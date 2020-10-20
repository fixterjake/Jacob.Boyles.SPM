using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SPM.Web.Data;
using SPM.Web.Models;
using SPM.Web.Services;
using ZDC.Web.Extensions;
using Task = SPM.Web.Models.Task;
using TaskStatus = SPM.Web.Models.TaskStatus;

namespace SPM.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// User manager constructor
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="userManager">User manager</param>
        public UserController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get user using the built in identity user manager
            var user = await _userManager.GetUserAsync(User);

            // Find teams that the user is apart of
            var userTeams = _context.UserTeams
                .Where(x => x.UserId == user.Id)
                .Select(x => x.TeamId)
                .ToList();

            // Get those teams
            var teams = _context.Teams
                .Where(x => userTeams.Contains(x.Id))
                .ToList();

            // Return view with teams
            return View(teams);
        }

        public IActionResult CreateTeam()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([Bind("Name,Description,FormImage")] Team team)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Get creator's user id for use in object creation
            var creator = int.Parse(_userManager.GetUserId(User));

            // Set other fields for created team
            team.CreatorId = creator;
            team.Created = DateTime.Now;

            // Create instance of the storage service
            var storageService = new BlobStorageService(_context);

            // Check if form image was null, only need to do checks
            // and upload if it is not null
            if (team.FormImage != null)
            {

                // Upload the image and get the url result and assign it to the team image field
                team.Image = await storageService.UploadFile(team.FormImage);

                // Check that image was uploaded successfully
                if (team.Image == null)
                {
                    // If not add error to model state
                    ModelState.AddModelError("FormImage", "Invalid image");

                    // Return back to form
                    return View();
                }
            }

            // Add team to database
            await _context.Teams.AddAsync(team);

            // Save database changes
            await _context.SaveChangesAsync();

            // Add creator to the team
            await _context.UserTeams.AddAsync(new UserTeam
            {
                TeamId = team.Id,
                UserId = creator,
                Created = DateTime.Now
            });

            // Save database changes
            await _context.SaveChangesAsync();

            // Redirect to team dashboard
            return Redirect("/user").WithSuccess("Success", "Team created.");
        }

        public IActionResult Team(int id)
        {
            // Try to find team
            var team = _context.Teams.FirstOrDefault(x => x.Id == id);

            // If team is null redirect back with error message
            if (team == null)
            {
                return Redirect("/user").WithDanger("Error", "Team not found.");
            }

            // Get user id
            var userId = int.Parse(_userManager.GetUserId(User));

            // Try to find user team object which means user can view the team
            var userTeam = _context.UserTeams
                .Where(x => x.TeamId == team.Id)
                .FirstOrDefault(x => x.UserId == userId);

            // If association was not found, redirect back with error message
            if (userTeam == null)
            {
                return Redirect("/user").WithDanger("Error", "Team not found.");
            }

            // Get number of users in team
            ViewBag.Users = _context.UserTeams.Count(x => x.TeamId == team.Id);

            // Get number of active sprints
            ViewBag.ActiveSprints = _context.Sprints
                .Where(x => x.TeamId == team.Id)
                .Count(x => x.Status == SprintStatus.Active);

            // Get number of inactive sprints
            ViewBag.InactiveSprints = _context.Sprints
                .Where(x => x.TeamId == team.Id)
                .Count(x => x.Status == SprintStatus.Inactive);

            // get number of extended sprints
            ViewBag.ExtendedSprints = _context.Sprints
                .Where(x => x.TeamId == team.Id)
                .Count(x => x.Status == SprintStatus.Extended);

            // Get number of completed sprints
            ViewBag.CompletedSprints = _context.Sprints
                .Where(x => x.TeamId == team.Id)
                .Count(x => x.Status == SprintStatus.Complete);

            // Get all sprints that are not completed
            ViewBag.Sprints = _context.Sprints
                .Where(x => x.TeamId == team.Id)
                .Where(x => x.Status == SprintStatus.Active
                                ||x.Status == SprintStatus.Inactive
                                ||x.Status == SprintStatus.Extended)
                .OrderBy(x => x.StartDate)
                .ToList();

            // Return view with the team
            return View(team);
        }

        public IActionResult CreateSprint(int id)
        {
            ViewBag.TeamId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSprint([Bind("Name,Description,Status,StartDate,EndDate")] Sprint sprint, int id)
        {
            // Check if returned data is valid
            if (!ModelState.IsValid)
            {
                ViewBag.TeamId = id;
                return View();
            }

            // Get creator's user id for use in object creation
            var creator = int.Parse(_userManager.GetUserId(User));

            // Set other sprint fields
            sprint.TeamId = id;
            sprint.Created = DateTime.Now;

            // Add sprint to database context
            await _context.Sprints.AddAsync(sprint);

            // Save database changes
            await _context.SaveChangesAsync();

            // Create user sprint object
            var userSprint = new UserSprint
            {
                UserId = creator,
                SprintId = sprint.Id,
                Created = DateTime.Now
            };

            // Add user sprint to database
            await _context.UserSprints.AddAsync(userSprint);

            // Save database changes
            await _context.SaveChangesAsync();

            // Redirect to sprint view with success message
            return Redirect($"/user/sprint/{sprint.Id}").WithSuccess("Success", "Sprint created.");
        }

        public IActionResult Sprint(int id)
        {
            // Try to get sprint
            var sprint = _context.Sprints.FirstOrDefault(x => x.Id == id);

            // If sprint not found redirect back with error
            if (sprint == null)
            {
                return Redirect("/user").WithDanger("Error", "Sprint not found");
            }

            // Get creator's user id to check if user can view sprint
            var user = int.Parse(_userManager.GetUserId(User));

            // Try to find user sprint object
            var userSprint = _context.UserSprints
                .Where(x => x.SprintId == sprint.Id)
                .FirstOrDefault(x => x.UserId == user);

            // If not found redirect back with error
            if (userSprint == null)
            {
                return Redirect("/user").WithDanger("Error", "Sprint not found");
            }

            // Get all items within this sprint
            ViewBag.Items = _context.Items
                .Where(x => x.SprintId == sprint.Id)
                .ToList();

            // Get number of all tasks within sprint
            ViewBag.Tasks = _context.Tasks
                .Count(x => x.SprintId == sprint.Id);

            // Get number of pending tasks
            ViewBag.PendingTasks = _context.Tasks
                .Where(x => x.SprintId == sprint.Id)
                .Count(x => x.Status == TaskStatus.Pending);

            // Get number of in progress tasks
            ViewBag.InProgressTasks = _context.Tasks
                .Where(x => x.SprintId == sprint.Id)
                .Count(x => x.Status == TaskStatus.InProgress);

            // Get number of blocked tasks
            ViewBag.BlockedTasks = _context.Tasks
                .Where(x => x.SprintId == sprint.Id)
                .Count(x => x.Status == TaskStatus.Blocked);

            // Get number of completed tasks
            ViewBag.CompletedTasks = _context.Tasks
                .Where(x => x.SprintId == sprint.Id)
                .Count(x => x.Status == TaskStatus.Complete);

            return View(sprint);
        }

        public IActionResult CreateItem(int id)
        {
            ViewBag.SprintId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([Bind("Name,Description,Status,StartDate,EndDate")] Item item, int id)
        {
            // Check if returned data is valid
            if (!ModelState.IsValid)
            {
                ViewBag.SprintId = id;
                return View();
            }

            // Get creator's user id for use in object creation
            var creator = int.Parse(_userManager.GetUserId(User));

            // Set other sprint fields
            item.SprintId = id;
            item.Created = DateTime.Now;

            // Add sprint to database context
            await _context.Items.AddAsync(item);

            // Save database changes
            await _context.SaveChangesAsync();

            // Create user sprint object
            var userItem = new UserItem
            {
                UserId = creator,
                ItemId = item.Id,
                Created = DateTime.Now
            };

            // Add user sprint to database
            await _context.UserItems.AddAsync(userItem);

            // Save database changes
            await _context.SaveChangesAsync();

            // Redirect to sprint view with success message
            return Redirect($"/user/item/{item.Id}").WithSuccess("Success", "Item created.");
        }

        public IActionResult Item(int id)
        {
            // Try to find item
            var item = _context.Items.FirstOrDefault(x => x.Id == id);

            // If item was not found redirect with error
            if (item == null)
            {
                return Redirect("/user").WithDanger("Error", "Item not found.");
            }

            // Get list of tasks
            ViewBag.Tasks = _context.Tasks
                .Where(x => x.ItemId == item.Id)
                .Where(x => x.Status == TaskStatus.InProgress ||
                            x.Status == TaskStatus.Pending ||
                            x.Status == TaskStatus.Blocked)
                .OrderByDescending(x => x.Status)
                .ToList();

            // Get pending tasks
            ViewBag.PendingTasks = _context.Tasks
                .Where(x => x.ItemId == item.Id)
                .Count(x => x.Status == TaskStatus.Pending);

            // Get number of blocked tasks
            ViewBag.BlockedTasks = _context.Tasks
                .Where(x => x.ItemId == item.Id)
                .Count(x => x.Status == TaskStatus.Blocked);

            // Get number of in progress tasks
            ViewBag.InProgressTasks = _context.Tasks
                .Where(x => x.ItemId == item.Id)
                .Count(x => x.Status == TaskStatus.InProgress);

            // Get number of completed tasks
            ViewBag.CompletedTasks = _context.Tasks
                .Where(x => x.ItemId == item.Id)
                .Count(x => x.Status == TaskStatus.Complete);

            // Return view with item data
            return View(item);
        }

        public IActionResult CreateTask(int id)
        {
            ViewBag.ItemId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([Bind("Name,Description,Status")] Task task, int id)
        {
            // If data was not valid return view with error
            if (!ModelState.IsValid)
            {
                ViewBag.ItemId = id;
                return View();
            }

            // Find item for use in task creation
            var item = _context.Items.FirstOrDefault(x => x.Id == id);

            // Set other fields of task from form
            if (item != null)
            {
                task.SprintId = item.SprintId;
            }
            task.ItemId = id;
            task.Created = DateTime.Now;

            // Add task to database
            await _context.Tasks.AddAsync(task);

            // Save database changes
            await _context.SaveChangesAsync();

            // Redirect back to item
            return Redirect($"/user/item/{id}").WithSuccess("Success", "Task created.");
        }
    }
}

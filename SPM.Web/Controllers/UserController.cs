using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore;
using SPM.Web.Data;
using SPM.Web.Models;
using SPM.Web.Models.ViewModels;
using SPM.Web.Services;
using ZDC.Web.Extensions;
using Task = SPM.Web.Models.Task;
using TaskStatus = SPM.Web.Models.TaskStatus;

namespace SPM.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        // Database context
        private readonly ApplicationDbContext _context;
        // User manager
        private readonly UserManager<User> _userManager;
        // Blob storage service
        private readonly BlobStorageService _storageService;

        /// <summary>
        /// User manager constructor
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="userManager">User manager</param>
        public UserController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _storageService = new BlobStorageService(_context);
        }

        /// <summary>
        /// Function to show index view
        /// </summary>
        /// <returns>Index view</returns>
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

        /// <summary>
        /// Function to show create team view
        /// </summary>
        /// <returns>Create team view</returns>
        public IActionResult CreateTeam()
        {
            return View();
        }

        /// <summary>
        /// Create team post endpoint to create a team
        /// </summary>
        /// <param name="team">Model bound team from form</param>
        /// <returns>Create team view if invalid data, or show the team that was created</returns>
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

        /// <summary>
        /// Function to show a specific team
        /// </summary>
        /// <param name="id">Id of team to show</param>
        /// <returns>Team view</returns>
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

        /// <summary>
        /// Function to show the edit team view
        /// </summary>
        /// <param name="id">Id of team to edit</param>
        [Authorize(Roles = "Administrator,Maintainer")]
        public IActionResult EditTeam(int id)
        {
            // Try to find sprint from id
            var team = _context.Teams.FirstOrDefault(x => x.Id == id);

            // if sprint is null redirect back, if it was found show edit view
            return team == null ? Redirect("/user").WithDanger("Error", "Team not found.") : View(team);
        }

        /// <summary>
        /// Edit team endpoint
        /// </summary>
        /// <param name="input">Model bound team from edit form</param>
        /// <param name="id">Id of team to edit</param>
        [HttpPost]
        [Authorize(Roles = "Administrator,Maintainer")]
        public async Task<IActionResult> EditTeam([Bind("Name,FormImage,Description")] Team input, int id)
        {
            // Ensure data is valid
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Get team
            var team = _context.Teams.FirstOrDefault(x => x.Id == id);

            // If team was not found, redirect with error
            if (team == null)
            {
                ModelState.AddModelError("Name", "Team not found.");
                return View();
            }

            // Update data
            // todo delete old image from azure
            team.Image = await _storageService.UploadFile(input.FormImage);
            team.Name = input.Name;
            team.Description = input.Description;

            // Save database changes
            await _context.SaveChangesAsync();

            // Redirect to team with success message
            return Redirect($"/user/team/{team.Id}").WithSuccess("Success", "Team edited.");
        }

        /// <summary>
        /// Function to show create sprint view
        /// </summary>
        /// <param name="id">Team id for use in form</param>
        /// <returns>Create sprint view</returns>
        public IActionResult CreateSprint(int id)
        {
            ViewBag.TeamId = id;
            return View();
        }

        /// <summary>
        /// Create sprint post endpoint for sprint creation
        /// </summary>
        /// <param name="sprint">Model bound sprint from form</param>
        /// <param name="id">Team id for use in sprint creation</param>
        /// <returns>Create sprint view if invalid data, or show the sprint</returns>
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

        /// <summary>
        /// Function to show a specific sprint view
        /// </summary>
        /// <param name="id">Id of sprint to view</param>
        /// <returns>Sprint view</returns>
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

            // Get all items within sprint
            var items = _context.Items
                .Where(x => x.SprintId == sprint.Id)
                .ToList();

            // Create list of item views to populate
            var itemViews = new List<ItemView>();

            // Populate item views
            foreach (var item in items)
            {
                itemViews.Add(new ItemView 
                    {
                        Item = item,
                        Tasks = _context.Tasks
                            .Where(x => x.ItemId == item.Id)
                            .ToList()
                    }
                );
            }

            // Create sprint view model
            var sprintView = new SprintView
            {
                Sprint = sprint,
                Items = itemViews
            };

            // Get all users in sprint
            var sprintUsers = _context.UserSprints
                .Where(x => x.SprintId == sprint.Id)
                .Select(x => x.UserId)
                .ToList();
            var users = _context.Users
                .Where(x => sprintUsers.Contains(x.Id))
                .ToList();

            // Add users to view bag
            ViewBag.Users = users;

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

            return View(sprintView);
        }

        /// <summary>
        /// Function to show the edit sprint view
        /// </summary>
        /// <param name="id">Id of sprint to edit</param>
        [Authorize(Roles = "Administrator,Maintainer")]
        public IActionResult EditSprint(int id)
        {
            // Try to find sprint from id
            var sprint = _context.Sprints.FirstOrDefault(x => x.Id == id);

            // if sprint is null redirect back, if it was found show edit view
            return sprint == null ? Redirect("/user").WithDanger("Error", "Sprint not found.") : View(sprint);
        }

        /// <summary>
        /// Endpoint to edit sprint
        /// </summary>
        /// <param name="input">Model bound sprint data from form</param>
        /// <param name="id">Id of sprint to edit</param>
        [HttpPost]
        [Authorize(Roles = "Administrator,Maintainer")]
        public async Task<IActionResult> EditSprint([Bind("Name,StartDate,EndDate,Status,Description")] Sprint input, int id)
        {
            // Ensure data is valid
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Get sprint
            var sprint = _context.Sprints.FirstOrDefault(x => x.Id == id);

            // If sprint was not found, redirect with error
            if (sprint == null)
            {
                ModelState.AddModelError("Name", "Sprint not found.");
                return View();
            }

            // Update data
            sprint.Name = input.Name;
            sprint.StartDate = input.StartDate;
            sprint.EndDate = input.EndDate;
            sprint.Status = input.Status;
            sprint.Description = input.Description;

            // Save database changes
            await _context.SaveChangesAsync();

            // Redirect back to sprint with success message
            return Redirect($"/user/sprint/{sprint.Id}").WithSuccess("Success", "Sprint edited");
        }

        /// <summary>
        /// Function to show create item view
        /// </summary>
        /// <param name="id">Id of sprint for use in form</param>
        /// <returns>Create item form</returns>
        public IActionResult CreateItem(int id)
        {
            ViewBag.SprintId = id;
            return View();
        }

        /// <summary>
        /// Create item post endpoint 
        /// </summary>
        /// <param name="item">Model bound item from form</param>
        /// <param name="id">Sprint id for use in item creation</param>
        /// <returns>Create item view if data is invalid, or show the item</returns>
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
            return Redirect($"/user/sprint/{item.SprintId}").WithSuccess("Success", "Item created.");
        }

        /// <summary>
        /// Function to show the edit item view
        /// </summary>
        /// <param name="id">Id of item to edit</param>
        [Authorize(Roles = "Administrator,Maintainer")]
        public IActionResult EditItem(int id)
        {
            // Find item
            var item = _context.Items.FirstOrDefault(x => x.Id == id);

            // if sprint is null redirect back, if it was found show edit view
            return item == null ? Redirect("/user").WithDanger("Error", "Item not found.") : View(item);
        }

        /// <summary>
        /// Endpoint to receive the edit item data
        /// </summary>
        /// <param name="input">Model bound item data from form</param>
        /// <param name="id">Id of item to edit</param>
        [HttpPost]
        [Authorize(Roles = "Administrator,Maintainer")]
        public async Task<IActionResult> EditItem([Bind("Name,Description")] Item input, int id)
        {
            // Ensure data is valid
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Find item
            var item = _context.Items.FirstOrDefault(x => x.Id == id);

            // Ensure item was found
            if (item == null)
            {
                ModelState.AddModelError("Name", "Item not found");
                return View();
            }

            // Update data
            item.Name = input.Name;
            item.Description = input.Description;

            // Save changed
            await _context.SaveChangesAsync();

            // Redirect to sprint view with success message
            return Redirect($"/user/sprint/{item.SprintId}").WithSuccess("Success", "Item edited.");
        }

        /// <summary>
        /// Function to show create task form
        /// </summary>
        /// <param name="id">Item id for use in form</param>
        /// <returns>Create task view</returns>
        public IActionResult CreateTask(int id)
        {
            ViewBag.ItemId = id;
            return View();
        }

        /// <summary>
        /// Create task endpoint
        /// </summary>
        /// <param name="task">Model bound task from form</param>
        /// <param name="id">Item id for use in task creation</param>
        /// <returns>Create task form if data is invalid, or refresh the item page to show the task</returns>
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
            return Redirect($"/user/sprint/{task.SprintId}").WithSuccess("Success", "Task created.");
        }

        /// <summary>
        /// Function to show the edit task view
        /// </summary>
        /// <param name="id">Id of task to edit</param>
        [Authorize(Roles = "Administrator,Maintainer")]
        public IActionResult EditTask(int id)
        {
            // Try to find task
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);

            // Ensure task exists and return view
            return task == null ? Redirect("/user").WithDanger("Error", "Task not found.") : View(task);
        }

        /// <summary>
        /// Endpoint to receive edited task data
        /// </summary>
        /// <param name="input">Model bound task from form</param>
        /// <param name="id">Id of task to edit</param>
        [HttpPost]
        [Authorize(Roles = "Administrator,Maintainer")]
        public async Task<IActionResult> EditTask([Bind("Name,Status,Description")] Task input, int id)
        {
            // Ensure data is valid
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Try to find task
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);

            // Ensure task was found
            if (task == null)
            {
                ModelState.AddModelError("Name", "Task not found.");
                return View();
            }

            // Update data
            task.Name = input.Name;
            task.Status = input.Status;
            task.Description = input.Description;

            // Save changes
            await _context.SaveChangesAsync();

            // Redirect to sprint view with success message
            return Redirect($"/user/sprint/{task.SprintId}").WithSuccess("Success", "Task edited.");
        }
    }
}

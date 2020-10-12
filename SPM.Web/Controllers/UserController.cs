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
                /*
                // Check if file is an image
                if (!team.FormImage.ContentType.Equals("image/jpg", StringComparison.OrdinalIgnoreCase) ||
                    !team.FormImage.ContentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                    !team.FormImage.ContentType.Equals("image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
                    !team.FormImage.ContentType.Equals("image/gif", StringComparison.OrdinalIgnoreCase) ||
                    !team.FormImage.ContentType.Equals("image/x-png", StringComparison.OrdinalIgnoreCase) ||
                    !team.FormImage.ContentType.Equals("image/png", StringComparison.OrdinalIgnoreCase))
                {
                    // If so add error to model state
                    ModelState.AddModelError("FormImage", "File must be an image");

                    // Return back to form
                    return View();
                }
                */

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

            // Redirect to team dahsboard
            return Redirect("/user").WithSuccess("Success", "Team created.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SPM.Web.Data;
using SPM.Web.Models;

namespace SPM.Web.Controllers
{
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SPM.Web.Data;
using SPM.Web.Models;
using SPM.Web.Models.ViewModels;
using ZDC.Web.Extensions;
using Role = SPM.Web.Models.ViewModels.Role;

namespace SPM.Web.Controllers
{
    [Authorize(Roles = "Administrator,Maintainer")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Admin controller constructor
        /// </summary>
        /// <param name="context">Database context</param>
        public AdminController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> EditRoles()
        {
            // Get all users
            var users = _context.Users
                .Where(x => x.EmailConfirmed)
                .ToList();

            // Add to viewbag
            ViewBag.users = users;

            // Get list of roles
            var roles = _context.Roles.ToList();

            // Add roles to viewbag
            ViewBag.roles = roles;

            // Get all user roles
            ViewBag.userRoles = await GetUserRoles();

            // Return view
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoles([Bind("UserId,RoleId")] Role data)
        {
            // Get all users
            var users = _context.Users
                .Where(x => x.EmailConfirmed)
                .ToList();

            // Add to viewbag
            ViewBag.users = users;

            // Get list of roles
            var roles = _context.Roles.ToList();

            // Add roles to viewbag
            ViewBag.roles = roles;

            // Ensure data is valid
            if (!ModelState.IsValid)
            {
                ViewBag.userRoles = GetUserRoles();
                return View();
            }

            // Get user
            var user = _context.Users
                .FirstOrDefault(x => x.Id == data.UserId);

            // Ensure user was found
            if (user == null)
            {
                ModelState.AddModelError("UserId", "User not found.");
                ViewBag.userRoles = GetUserRoles();
                return View();
            }

            // Get role
            var role = _context.Roles
                .FirstOrDefault(x => x.Id == data.RoleId);

            // Ensure role was found
            if (role == null)
            {
                ModelState.AddModelError("RoleId", "Role not found.");
                ViewBag.userRoles = GetUserRoles();
                return View();
            }

            // Add user to role
            await _userManager.AddToRoleAsync(user, role.Name);

            ViewBag.userRoles = await GetUserRoles();

            // Return with success message
            return View().WithSuccess("Success", "User added to role");
        }

        public async Task<List<UserRoles>> GetUserRoles()
        {
            // Create empty list
            var list = new List<UserRoles>();

            // Get current user
            var currentUser = await _userManager.GetUserAsync(User);

            // Get list of users to add or remove roles excluding the current user
            var users = _context.Users
                .Where(x => x.EmailConfirmed)
                .Where(x => x.Id != currentUser.Id)
                .ToList();

            // Iterate through users
            foreach (var user in users)
            {
                // Get if user has a role
                var userRole = _context.UserRoles
                    .FirstOrDefault(x => x.UserId == user.Id);

                if (userRole != null)
                {
                    // Get role
                    var role = _context.Roles
                        .FirstOrDefault(x => x.Id == userRole.RoleId);

                    // Ensure role existed
                    if (role != null)
                    {
                        // Add new item to list
                        list.Add(new UserRoles
                        {
                            User = user,
                            RoleId = role.Id,
                            RoleName = role.Name
                        });
                    }
                }
            }
            
            // Return list
            return list;
        }

        public async Task<IActionResult> RemoveFromRole(int roleId, int userId)
        {
            // Get role
            var role = _context.Roles
                .FirstOrDefault(x => x.Id == roleId);

            // Ensure role exists
            if (role == null)
            {
                return RedirectToAction("EditRoles").WithDanger("Error", "Role not found.");
            }

            // Get user
            var user = _context.Users
                .FirstOrDefault(x => x.Id == userId);

            // Ensure user exists
            if (user == null)
            {
                return RedirectToAction("EditRoles").WithDanger("Error", "User not found.");
            }

            // Remove from role
            await _userManager.RemoveFromRoleAsync(user, role.Name);

            // Return to edit view
            return RedirectToAction("EditRoles").WithSuccess("Success", "User removed from role.");
        }
    }
}

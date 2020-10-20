using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SPM.Web.Data;
using SPM.Web.Models;

namespace SPM.Web.Controllers
{
    public class HomeController : Controller
    {
        // Database context
        private readonly ApplicationDbContext _context;
        // Signin manager
        private readonly SignInManager<User> _signInManager;

        /// <summary>
        /// Constructor to init database context and signin manager
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="signInManager">Signin manager</param>
        public HomeController(ApplicationDbContext context, SignInManager<User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Function to show index view
        /// </summary>
        /// <returns>Index view</returns>
        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                Redirect("/user");
            }

            return View();
        }

        /// <summary>
        /// Function to show privacy view
        /// </summary>
        /// <returns>Privacy view</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Function to show error view
        /// </summary>
        /// <returns>Error view</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

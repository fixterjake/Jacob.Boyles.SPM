using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        ///     Constructor to init database context and signin manager
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="signInManager">Signin manager</param>
        public HomeController(ApplicationDbContext context, SignInManager<User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     Function to show index view
        /// </summary>
        /// <returns>Index view</returns>
        public IActionResult Index()
        {
            // If first time setup has not happened, display message to register
            ViewBag.setup = !_context.Users.Any();

            // If user is signed in direct them to the teams dashboard
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "User");
            }

            return View();
        }

        /// <summary>
        ///     Function to show error view
        /// </summary>
        /// <returns>Error view</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
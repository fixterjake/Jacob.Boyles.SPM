using Microsoft.AspNetCore.Mvc;

namespace ZDC.Web.Extensions
{
    public static class AlertExtensions
    {
        /// <summary>
        ///     Function to show success alert
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="title">Title</param>
        /// <param name="body">Body</param>
        /// <returns>Alert partial view</returns>
        public static IActionResult WithSuccess(this IActionResult result, string title, string body)
        {
            return Alert(result, "success", title, body);
        }

        /// <summary>
        ///     Function to show info alert
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="title">Title</param>
        /// <param name="body">Body</param>
        /// <returns>Alert partial view</returns>
        public static IActionResult WithInfo(this IActionResult result, string title, string body)
        {
            return Alert(result, "info", title, body);
        }

        /// <summary>
        ///     Function to show warning alert
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="title">Title</param>
        /// <param name="body">Body</param>
        /// <returns>Alert partial view</returns>
        public static IActionResult WithWarning(this IActionResult result, string title, string body)
        {
            return Alert(result, "warning", title, body);
        }

        /// <summary>
        ///     Function to show danger alert
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="title">Title</param>
        /// <param name="body">Body</param>
        /// <returns>Alert partial view</returns>
        public static IActionResult WithDanger(this IActionResult result, string title, string body)
        {
            return Alert(result, "danger", title, body);
        }

        /// <summary>
        ///     Function to show alert
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="type">type</param>
        /// <param name="title">Title</param>
        /// <param name="body">Body</param>
        /// <returns>An alert decorator result with the given type</returns>
        private static IActionResult Alert(IActionResult result, string type, string title, string body)
        {
            return new AlertDecoratorResult(result, type, title, body);
        }
    }
}
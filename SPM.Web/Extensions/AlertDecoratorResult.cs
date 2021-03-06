﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace ZDC.Web.Extensions
{
    public class AlertDecoratorResult : IActionResult
    {
        /// <summary>
        ///     Constructor for alert decorators
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="type">type</param>
        /// <param name="title">title</param>
        /// <param name="body">body</param>
        public AlertDecoratorResult(IActionResult result, string type, string title, string body)
        {
            Result = result;
            Type = type;
            Title = title;
            Body = body;
        }

        public IActionResult Result { get; }
        public string Type { get; }
        public string Title { get; }
        public string Body { get; }

        /// <summary>
        ///     Function to execute result
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>Executed result</returns>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            //NOTE: Be sure you add a using statement for Microsoft.Extensions.DependencyInjection, otherwise
            //      this overload of GetService won't be available!
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();

            var tempData = factory.GetTempData(context.HttpContext);
            tempData["_alert.type"] = Type;
            tempData["_alert.title"] = Title;
            tempData["_alert.body"] = Body;

            await Result.ExecuteResultAsync(context);
        }
    }
}
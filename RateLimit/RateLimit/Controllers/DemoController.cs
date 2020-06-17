using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RateLimit.Controllers
{
    /// <summary>
    /// This Demo controller will receive the http request from the requestor
    /// 
    /// </summary>
    [Route("api/[controller]")]
    public class DemoController : Controller
    {
        /// <summary>
        /// This Get method setup as default launch url, which is invoked by the requestor
        /// This method is not modified here, it is the default generic method
        /// Instead an action filter attribute is used which is called before GET method is executed."
        /// Set the number of hours and the limit of requests in that hours in this RequestRateLimit attribute 
        /// </summary>
        /// <returns>The method returns the default values</returns>
        [RateLimitAttribute.RequestRateLimit(Path ="demo", Hours = 1, Limit = 100)]
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { $"Requests are limited to 100 per user per hour" };
        }
    }
}

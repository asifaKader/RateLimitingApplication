using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace RateLimitAttribute
{
    /// <summary>
    /// This class will create custom action filter attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestRateLimitAttribute : ActionFilterAttribute
    {
        public string Path { get; set; }
        public int Hours { get; set; }
        public int Limit { get; set; }

        public static Dictionary<string, Dictionary<double, int>> Cache = new Dictionary<string, Dictionary<double, int>>();
        public static Dictionary<double, int> RequestCountTracker = new Dictionary<double, int>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
           var ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;

            var memoryCacheKey = $"{ip}-{Path}";
            
            var currentTimestamp = UnixTimeStamp(DateTime.Now);

            //Checking if this is a first request from the user
            //If it is the first request, cache the memoryCacheKey along with the time stamp
            if (!Cache.ContainsKey(memoryCacheKey))
            {
                CacheMemoryCacheKey(currentTimestamp,memoryCacheKey);
            } else
            {
                //If it is not the first request add the cache to the dictionary
                //This is done to store the number of requests in a particular timestamp
                RequestCountTracker = Cache[memoryCacheKey];
                //If there is no a request in this currentTimestamp, add one
                if (!RequestCountTracker.ContainsKey(currentTimestamp))
                {
                    RequestCountTracker.Add(currentTimestamp, 1);
                }
                //Calculate the total number of request in this currentTimestamp
                var totalRequests = CalculateTotalRequests(currentTimestamp, RequestCountTracker);
                //Check the request exceeded the limit or not
                CheckTotalRequestExceedsLimit(context, currentTimestamp,totalRequests);
                //Clear the requests from the RequestCountTracker after the number of Hours set
                var updatedRequestCount = ClearOldRequestCounts(currentTimestamp, RequestCountTracker);
                //Update the Cache with the latest RequestCountTracker
                Cache[memoryCacheKey] = updatedRequestCount;

            }
            base.OnActionExecuting(context);
        }
        /// <summary>
        /// This method will check whether number of request exceeded the limit or not
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentTimestamp"></param>
        /// <param name="totalRequests"></param>
        private void CheckTotalRequestExceedsLimit(ActionExecutingContext context, double currentTimestamp, int totalRequests)
        {
            if (totalRequests >= Limit)
            {
                context.Result = new ContentResult
                {
                    Content = $"Rate limit exceeded",
                };
                context.HttpContext.Response.StatusCode = 429;
            }
            else
            {
                RequestCountTracker[currentTimestamp] += 1;
            }
        }
        /// <summary>
        /// This method will calculate the total number of requests at a particulate time stamp
        /// </summary>
        /// <param name="currentTimestamp"></param>
        /// <param name="requestCount"></param>
        /// <returns></returns>
        public int CalculateTotalRequests(double currentTimestamp, Dictionary<double, int> requestCount)
        {
            var totalCount = 0;
            if (requestCount.ContainsKey(currentTimestamp))
            {
                var oneHourAgo = TimeStampAgo(currentTimestamp);
                var value = requestCount[currentTimestamp];

                totalCount = requestCount.Where(r => r.Key > oneHourAgo).Sum(x => x.Value);
            }
            return totalCount;
        }
        /// <summary>
        /// This method will convert the current datetime to unix datetime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private double UnixTimeStamp(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (dateTime.ToUniversalTime() - epoch).TotalSeconds;
        }
        /// <summary>
        /// This method will calculate the duration of the time(in Hours) set based on the current time stamp
        /// </summary>
        /// <param name="currentTimestamp"></param>
        /// <returns></returns>
        private double TimeStampAgo(double currentTimestamp)
        {
            var timeSpan = TimeSpan.FromSeconds(currentTimestamp);
            var setHoursAgo = new DateTime(timeSpan.Ticks).ToUniversalTime().Subtract(new TimeSpan(0, Hours, 0, 0));
            return UnixTimeStamp(setHoursAgo);   
        }
        /// <summary>
        /// This method will clear the requests from the RequestCountTacker after the set number of Hours
        /// </summary>
        /// <param name="currentTimeStamp"></param>
        /// <param name="requestCount"></param>
        /// <returns></returns>
        private Dictionary<double, int> ClearOldRequestCounts(double currentTimeStamp, Dictionary<double, int> requestCount)
        {
            var oneHourAgo = TimeStampAgo(currentTimeStamp);
            return requestCount.Where(r => r.Key > oneHourAgo).ToDictionary(x => x.Key, x => x.Value);
        }
        /// <summary>
        /// This method will Cache the request with memoryCacheKey and time stamp if the request is for the first time
        /// </summary>
        /// <param name="currentTimestamp"></param>
        /// <param name="memoryCacheKey"></param>
        private void CacheMemoryCacheKey(double currentTimestamp, string memoryCacheKey)
        {

            RequestCountTracker.Add(currentTimestamp, 1);
            Cache.Add(memoryCacheKey, RequestCountTracker);

        }
    }
}

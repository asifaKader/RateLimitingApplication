using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RateLimit.Tests
{
    /// <summary>
    /// This class is used to define and implement CurrentDateTime, RequestCount and  
    /// </summary>
    public class RequestCountHelper
    {
        public static Dictionary<double, int> RequestCount { get; set; }
        public static double CurrentTimestamp = UnixTimeStamp(DateTime.Now);
        /// <summary>
        /// This method will increment the request count for the current timestamp by 1
        /// </summary>
        /// <returns></returns>
        public static Dictionary<double, int> IncrementRequestCount()
        {
            Dictionary<double, int> requestCount = new Dictionary<double, int>();
            requestCount.Add(CurrentTimestamp, 1);
            return requestCount;
        }
        /// <summary>
        /// This property represents a read-only, generic collection of key/value pairs where key is the CurrentTimestamp and Value is the RequestCount dictionary
        /// You can pass as many parameters as you want in here , which can be consumed by the test methods
        /// </summary>
        public static readonly IReadOnlyDictionary<double, Dictionary<double, int>> CachedRequest = new Dictionary<double, Dictionary<double, int>>
        {
            { CurrentTimestamp,IncrementRequestCount()},
            { CurrentTimestamp+1,IncrementRequestCount()}
        }
        .ToImmutableDictionary();
        /// <summary>
        /// Converting CachedRequest to an Enumerable object
        /// </summary>
        public static IEnumerable<object[]> TestCases
        {
            get
            {
                var items = new List<object[]>();
                foreach (var item in CachedRequest)
                    items.Add(new object[] { item.Key });
                return items;
            }
        }
        /// <summary>
        /// This method will convert the current datetime to unix datetime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static double UnixTimeStamp(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (dateTime.ToUniversalTime() - epoch).TotalSeconds;
        }
    }
}

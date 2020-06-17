using RateLimitAttribute;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RateLimit.Tests
{
    public class RequestRateLimitAttributeTests
    {
        private readonly RequestRateLimitAttribute _requestRateLimitAttribute;

        public RequestRateLimitAttributeTests()
        {
            _requestRateLimitAttribute = new RequestRateLimitAttribute();
        }
        [Fact]
        public void CalculateTotalRequests_atSameRateLimitPass()
        {
            //Arrange
            var dateTime = RequestCountHelper.CurrentTimestamp;
            var requestCount = new Dictionary<double, int>();
            requestCount.Add(dateTime, 100);
            var expected = 100;
            //Act
            var actual = _requestRateLimitAttribute.CalculateTotalRequests(dateTime, requestCount);
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void CalculateTotalRequests_atSameRateLimitFail()
        {
            //Arrange
            var dateTime = RequestCountHelper.CurrentTimestamp;
            var requestCount = new Dictionary<double, int>();
            requestCount.Add(dateTime, 2);
            var expected = 100;
            //Act
            var actual = _requestRateLimitAttribute.CalculateTotalRequests(dateTime, requestCount);
            //Assert
            Assert.NotEqual(expected, actual);
        }
        [Theory]
        [MemberData(nameof(RequestCountHelper.TestCases), MemberType = typeof(RequestCountHelper))]
        public void CalculateTotalRequests_atSameRateLimitPass_MultipleTimes(double currentField)
        {
            //Arrange
            var dateTime = RequestCountHelper.CurrentTimestamp;
            var requestCount = RequestCountHelper.CachedRequest.FirstOrDefault().Value;
            var expected = 1;
            //Act
            var actual = _requestRateLimitAttribute.CalculateTotalRequests(dateTime, requestCount);
            //Assert
            Assert.Equal(expected, actual);
        }
        [Theory]
        [MemberData(nameof(RequestCountHelper.TestCases), MemberType = typeof(RequestCountHelper))]
        public void CalculateTotalRequests_atSameRateLimitFail_MultipleTimes(double currentField)
        {
            //Arrange
            var dateTime = currentField;
            var requestCount = RequestCountHelper.CachedRequest.FirstOrDefault().Value;
            var expected = 0;
            //Act
            var actual = _requestRateLimitAttribute.CalculateTotalRequests(dateTime, requestCount);
            //Assert
            Assert.Equal(expected, actual);
        }
    }
    
}

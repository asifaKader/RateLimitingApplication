using RestSharp;
using Xunit;

namespace RateLimit.Tests
{
    public class DemoControllerTests
    {
        private const string ApiPath = "http://localhost:52743/api/demo";

        [Fact]
        public void ApiGET_OneRequestInOneHourPerUserPass()
        {
            var client = new RestClient(ApiPath);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddParameter("application/json; charset=utf-8", "", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Assert.Equal(0, (int) response.StatusCode);
        }
        [Fact]
        public void ApiGET_OneRequestInOneHourPerUserFail()
        {
            var client = new RestClient(ApiPath);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddParameter("application/json; charset=utf-8", "", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Assert.NotEqual(429, (int)response.StatusCode);
        }
        [Theory]
        [InlineData(3,0)]
        public void ApiGET_MoreRequestInOneHourPerUserPass(int numberOfRequests, int expected)
        {
            var client = new RestClient(ApiPath);
            client.Timeout = -1;
            IRestResponse response = null;
            for (int i = 0; i < numberOfRequests; i++)
            {
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json; charset=utf-8");
                request.AddParameter("application/json; charset=utf-8", "", ParameterType.RequestBody);
                response = client.Execute(request);
            }
            Assert.Equal(expected, (int)response.StatusCode);
        }
    }
}
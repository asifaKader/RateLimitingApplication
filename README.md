# RateLimitingApplication
This is simple application written in C# ASP.Net Core  for limiting the number of times your users can request a particular endpoint. This application is designed to limit 100 requests from a single user in an hour.

An action filter attribute is used as an annotation which is called before GET method is executed. 

How to create this custome action filter attribute? Here it goes
We need to use a  class that inherits from ActionFilterAttribute. Lets call our action filter attribute as "RequestRateLimitAttribute". This class has the following properties - 
	1. Path - unique name, 
	2. Hour - the period of time 
	3. Limit - the number of requests passed in the set hour
	3. Cache - Dictionary to store the key-value pair(in this case the "ip-Path") for caching purpose. We can also use Microsoft memory cache or Redis for this purpose depending on how you want to cache. 
	4. RequestCountTracker - Dictionary which store the timespan as key and the number of requets in this span of time as value.
	Dictionary to Store the count of requests at a particular time stamp
	
We are overridding OnActionExecuting method from our inherited class to perform the following tasks; 

1) Obtaining the users ip address

2) Storing the ip address as key and the RequestCountTracker in the Cache, every time when a new user request the API

3) RequestCountTracker will keep track of the number of requests hitting at a particular timestamp

4) When a new request is received calculate the total number of requests from that user in the RequestCountTracker and check if it exceeds the rate limit 

3) Returning an error message and a relevant status code (HTTP 429), in the event that the user hits rate limit set for the API(that is the Hours and Limit config in RequestRateLimit Attribute)

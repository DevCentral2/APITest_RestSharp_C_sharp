using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestSharp_Demo
{

    // methods

    public partial class ApiTests
    {
        private static RestRequest CreateGETRequest(string requestString)
        {
            return new RestRequest(requestString, Method.Get);
        }


        private static RestRequest CreateRequest(string methodString, string requestString)
        {
            Method method = new Method();

            switch (methodString.ToUpper())
            {
                case "GET":
                    method = Method.Get;
                    break;
                case "POST":
                    method = Method.Post;
                    break;
                case "PUT":
                    method = Method.Put;
                    break;
                case "PATCH":
                    method = Method.Patch;
                    break;
                default:
                    Console.WriteLine($"Specified method {methodString} not found.");
                    break;
            }

            return new RestRequest(requestString, method);
        }

        // Execute the request
        RestResponse ExecuteRequest(RestRequest restRequest)
        {
            var response = _restClient.Execute(restRequest);
            return response;
        }

        void ValidateResponse(RestResponse response, HttpStatusCode httpStatusCode)
        {
            Assert.That(response.StatusCode, Is.EqualTo(httpStatusCode));

        }

        private void GetSessionTokenAndAddToClientHeader()
        {
            string userToken = GetSessionToken();
            AddSessionTokenHeaderToRestClient(userToken);
        }

        private void AddSessionTokenHeaderToRestClient(string userToken)
        {
            // this is needed for API tests that require a user session
            _restClient.AddDefaultHeader("User-Token", userToken);
        }
        private static void CheckResponseStatusCodeExpectedError(RestResponse response, int expectedError)
        {

            var responseBody = JsonConvert.DeserializeObject<ResponseBody>(response.Content);
            Assert.That(responseBody.error_code, Is.EqualTo(expectedError), $"Expected error code {expectedError} but got {responseBody.error_code}");

        }

        private void AddAuthorisationTokenHeader(RestRequest restRequest, string token)
        {
            restRequest.AddHeader("Authorization", $"Token token={token}");
        }

        private void AddContentTypeJsonHeader(RestRequest restRequest)
        {
            restRequest.AddHeader("Content-Type", "application/json");
        }

        private void AddApplicationJsonParamforRequestBody(string jsonBody)
        {
            _restRequest.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
        }

        private QuoteResponse GetSpecificQuote(int quoteId)
        {
            string restRequest = $"quotes/{quoteId}";
            RestRequest _restRequest = CreateRequest("GET", restRequest);

            AddContentTypeJsonHeader(_restRequest);
            AddAuthorisationTokenHeader(_restRequest, token);
            var response = ExecuteRequest(_restRequest);
            ValidateResponse(response, HttpStatusCode.OK);

            QuoteResponse quoteResponse = JsonConvert.DeserializeObject<QuoteResponse>(response.Content);
            return quoteResponse;
        }

        private static string GetSessionToken()
        {
            RestResponse response = CreateSession();

            if (response.StatusCode == HttpStatusCode.OK)
            {

                SessionResponse responseBody = JsonConvert.DeserializeObject<SessionResponse>(response.Content);

                // Extract the user token
                var userToken = responseBody.UserToken;

                // Print the user token
                Console.WriteLine($"User token: {userToken}");

                return userToken;

            }
            else
            {
                // Deserialize the error response body
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);


                Console.WriteLine($"Error {errorResponse.error_code}: {errorResponse.message}");
                return null;
            }
        }

        private static RestResponse CreateSession()
        {
            RestClient client = new RestClient(ApiClient);
            RestRequest request = CreateRequest("POST", "session");

            // Add the authorisation token
            request.AddHeader("Authorization", $"Token token={token}");


            // Define the request body
            var requestBody = new
            {
                user = new
                {
                    login = "30607",
                    password = "66cde56302f77e14"
                }
            };
            request.AddJsonBody(requestBody);

            RestResponse response = client.Execute(request);
            return response;
        }
        private static void ValidateResponseStatusAndErrorCodes(RestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseBody = JsonConvert.DeserializeObject<ResponseBody>(response.Content);

                if (responseBody.error_code != null)
                {
                    Assert.Fail($"Error: Error Code: {responseBody.error_code}: Error message: {responseBody.message}");
                }
                else
                {
                    Console.WriteLine("No error codes present.");
                }
            }
            else
            {
                Assert.Fail($"Error Status Code: {response.StatusCode}");
            }
        }
        private RestResponse CreateUser()
        {
           
            var requestBody = new
            {
                user = new
                {
                    login = "anonuser",
                    email = "anonymous@example.com",
                    password = "mysecretpswd"
                }
            };


            var jsonBody = JsonConvert.SerializeObject(requestBody);

            _restRequest = CreateRequest("POST", "users");

            // Set request headers
            AddContentTypeJsonHeader(_restRequest);
            AddAuthorisationTokenHeader(_restRequest, token);

            AddApplicationJsonParamforRequestBody(jsonBody);

            var response = ExecuteRequest(_restRequest);
            return response;
        }
    }
}

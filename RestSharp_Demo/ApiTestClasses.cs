using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharp_Demo
{
    public class QuoteResponse
    {
        public int id { get; set; }
        public bool dialogue { get; set; }
        public bool @private { get; set; }
        public List<string> tags { get; set; }
        public string? url { get; set; }
        public int favorites_count { get; set; }
        public int upvotes_count { get; set; }
        public int downvotes_count { get; set; }
        public string? author { get; set; }
        public string? author_permalink { get; set; }
        public string? body { get; set; }
        public UserDetails? user_details { get; set; }
    }

    public class UserDetails
    {
        public bool favorite { get; set; }
        public bool upvote { get; set; }
        public bool downvote { get; set; }
        public bool hidden { get; set; }
    }


    // this is for use with /api/quotes
    public class QuotesResponse
    {
        public int page { get; set; }
        public bool last_page { get; set; }
        public List<QuoteResponse>? quotes { get; set; }
    }


    public class SessionResponse
    {

        // The UserToken property should be mapped to the "User-Token" key in the JSON  of response.Content
        // otherwise it is unable to deserialse properly and returns an empty string for the UserToken

        [JsonProperty("User-Token")]
        public string UserToken { get; set; }
        public string? login { get; set; }
        public string? email { get; set; }
    }

    public class ErrorResponse
    {
        public int error_code { get; set; }
        public string? message { get; set; }
    }


    public class RequestBody
    {
        public User? User { get; set; }
    }

    public class User
    {
        public string? login { get; set; }
        public string? password { get; set; }
    }

    public class ResponseBody
    {
        public int? error_code { get; set; }
        public string? message { get; set; }
    }

    public enum ErrorCode
    {
        InvalidRequest = 10,
        PermissionDenied = 11,
        UserSessionNotFound = 20,
        InvalidLoginOrPassword = 21,
        LoginNotActive = 22,
        UserLoginOrPasswordMissing = 23,
        ProUserRequired = 24,
        UserNotFound = 30,
        UserSessionAlreadyPresent = 31,
        UserValidationErrors = 32,
        InvalidPasswordResetToken = 33,
        QuoteNotFound = 40,
        PrivateQuotesCannotBeUnfavd = 41,
        CouldNotCreateQuote = 42,
        AuthorNotFound = 50,
        TagNotFound = 60,
        ActivityNotFound = 70
    }

}

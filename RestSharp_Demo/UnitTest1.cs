//namespace RestSharp_Demo
//{
//    using Newtonsoft.Json;
//    using NUnit.Framework;
//    using RestSharp;
//    using System.Net;
//    using System.Xml.Linq;

//    public class Post
//    {
//        public int Id { get; set; }
//        public string Title { get; set; }
//        public string Body { get; set; }
//        public int UserId { get; set; }
//    }

//    public class User
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public string Email { get; set; }
//        public string Username { get; set; }
//        public Address Address { get; set; } // Assuming the User class has an Address property
//                                             // Add more properties based on the actual structure of your user data
//    }

//    public class Address
//    {
//        public string City { get; set; }
//        public string PostCode { get; set; }
//        // Add more properties based on the actual structure of your address data
//    }

//    public class Comment
//    {
//        public int Id { get; set; }
//        public int PostId { get; set; }
//        public string Name { get; set; }
//        public string Email { get; set; }
//        public string Body { get; set; }
//    }


//    [TestFixture]
//    public class ApiTests
//    {
//        private RestClient? _restClient;
//        private RestRequest? _restRequest;



//        // Execute the request
//        RestResponse ExecuteRequest(RestRequest restRequest)
//        {
//            var response = _restClient.Execute(restRequest);
//            return response;
//        }

//        void ValidateResponse(RestResponse response, HttpStatusCode httpStatusCode)
//        {
//            Assert.That(response.IsSuccessful, Is.True);
//            Assert.That(response.StatusCode, Is.EqualTo(httpStatusCode));
//            Assert.That(response.Content, Is.Not.Null.Or.Empty);
//        }

//        //void ValidateDeleted(RestResponse response, HttpStatusCode httpStatusCode)
//        //{       // Validate the response
//        //    Assert.That(response.StatusCode, Is.EqualTo(httpStatusCode));
//        //    Assert.That(response.Content, Is.Null.Or.Empty);
//        //}



//        [SetUp]
//        public void Setup()
//        {
//            // Set up the base API URL
//            _restClient = new RestClient("https://jsonplaceholder.typicode.com");
//        }


//        [Test]
//        public void GetPosts()
//        {
//            // Set up the API request for getting posts
//            _restRequest = new RestRequest("/posts", Method.Get);

//            // Execute the request
//            var response = ExecuteRequest(_restRequest);

//            // Validate the response
//            ValidateResponse(response, HttpStatusCode.OK);


//            // Deserialize the response content to a list of posts
//            List<Post> posts = JsonConvert.DeserializeObject<List<Post>>(response.Content);

//            // Validate specific aspects of the retrieved posts
//            Assert.That(posts, Is.Not.Null.And.Not.Empty);

//            // Check if there are at least a certain number of posts
//            Assert.That(posts.Count, Is.GreaterThan(0));


//            // This is a basic validation

//            // Validate the structure of each post
//            foreach (var post in posts)
//            {
//                Assert.That(post.Id, Is.GreaterThan(0));
//                Assert.That(post.Title, Is.Not.Null.Or.Empty);
//                Assert.That(post.Body, Is.Not.Null.Or.Empty);
//                Assert.That(post.UserId, Is.GreaterThan(0));

//            }
//        }

//        [Test]
//        public void GetSinglePost()
//        {
//            // Set up the API request for getting a single post
//            _restRequest = new RestRequest("/posts/1", Method.Get);

//            // Execute the request
//            var response = ExecuteRequest(_restRequest);

//            // Validate the response
//            ValidateResponse(response, HttpStatusCode.OK);

//            // Deserialize the response content to a single post
//            Post singlePost = JsonConvert.DeserializeObject<Post>(response.Content);

//            // Validate specific aspects of the retrieved single post
//            Assert.That(singlePost, Is.Not.Null);
//            Assert.That(singlePost.Id, Is.GreaterThan(0));
//            Assert.That(singlePost.Title, Is.Not.Null.Or.Empty);
//            Assert.That(singlePost.Body, Is.Not.Null.Or.Empty);
//            Assert.That(singlePost.UserId, Is.GreaterThan(0));
//        }

//        [Test]
//        public void GetUserById()
//        {
//            // Set up the API request for getting a user by ID
//            _restRequest = new RestRequest("/users/1", Method.Get);

//            // Execute the request
//            var response = ExecuteRequest(_restRequest);

//            // Validate the response
//            ValidateResponse(response, HttpStatusCode.OK);

//            // Deserialize the response content to a single user
//            User singleUser = JsonConvert.DeserializeObject<User>(response.Content);

//            // Validate specific aspects of the retrieved user
//            Assert.That(singleUser, Is.Not.Null);
//            Assert.That(singleUser.Id, Is.GreaterThan(0));
//            Assert.That(singleUser.Name, Is.Not.Null.Or.Empty);
//            Assert.That(singleUser.Email, Is.Not.Null.Or.Empty);
//            Assert.That(singleUser.Username, Is.Not.Null.Or.Empty);


//            //// Validate that the user's address is not null (assuming the User class has an Address property)
//            //Assert.That(singleUser.Address, Is.Not.Null);

//            //// Validate specific aspects of the user's address
//            //Assert.That(singleUser.Address.City, Is.Not.Null.Or.Empty);
//            //Assert.That(singleUser.Address.PostCode, Is.Not.Null.Or.Empty);
//        }


//        [Test]

//        public void CreatePost()
//        {

//            // Set up the API request for creating a post
//            var newPost = new
//            {
//                title = "New Post Title",
//                body = "This is the body of the new post.",
//                userId = 101    // Replace with a valid user ID from your system
//            };

//            _restRequest = new RestRequest("/posts", Method.Post)
//                .AddJsonBody(newPost);

//            // Execute the request
//            var response = ExecuteRequest(_restRequest);

//            // Validate the response
//            ValidateResponse(response, HttpStatusCode.Created);

//            Assert.That(response.Content, Is.SupersetOf(newPost.body));


//            // Deserialize the response content to the created post
//            Post createdPost = JsonConvert.DeserializeObject<Post>(response.Content);

//            // Validate specific aspects of the created post
//            Assert.That(createdPost, Is.Not.Null);
//            Assert.That(createdPost.Id, Is.GreaterThan(0));
//            Assert.That(createdPost.Id, Is.EqualTo(newPost.userId));
//            Assert.That(createdPost.Title, Is.EqualTo(newPost.title));
//            Assert.That(createdPost.Body, Is.EqualTo(newPost.body));
//            Assert.That(createdPost.UserId, Is.EqualTo(newPost.userId));
//        }

//        [Test]
//        public void UpdatePost()
//        {
//            var updatedPost = new
//            {
//                title = "Updated Post Title",
//                body = "This is the updated body of the post.",
//                userId = 1    // Replace with a valid user ID from your system
//            };

//            _restRequest = new RestRequest("/posts/1", Method.Put)
//                .AddJsonBody(updatedPost);

//            // Execute the request
//            var response = ExecuteRequest(_restRequest);

//            // Validate the response
//            ValidateResponse(response, HttpStatusCode.OK);

//            // Deserialize the response content to the updated post
//            Post updatedPostResponse = JsonConvert.DeserializeObject<Post>(response.Content);

//            // Validate specific aspects of the updated post
//            Assert.That(updatedPostResponse, Is.Not.Null);
//            Assert.That(updatedPostResponse.Id, Is.EqualTo(updatedPost.userId)); // Assuming the post ID remains the same
//            Assert.That(updatedPostResponse.Title, Is.EqualTo(updatedPost.title));
//            Assert.That(updatedPostResponse.Body, Is.EqualTo(updatedPost.body));


//        }

//        //[Test]
//        public void DeletePost()
//        {
//            // Set up the API request for deleting a post
//            _restRequest = new RestRequest("/posts/1", Method.Delete);

//            // Execute the request
//            var response = ExecuteRequest(_restRequest);

//            // Validate the response
//            ValidateResponse(response, HttpStatusCode.OK);

//            // Add a new API request to retrieve the post after deletion
//            _restRequest = new RestRequest("/posts/1", Method.Get);
//            var getResponse = _restClient.Execute(_restRequest);

//            //// Validate the response for the delete request
//            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//            //Assert.That(response.Content, Is.Not.Null.Or.Empty);
//        }

//        //[Test]
//        public void DeletePost2()
//        {
//            // Set up the API request for deleting a post (assuming post ID is 1)
//            _restRequest = new RestRequest("/posts/1", Method.Delete);

//            // Execute the request
//            var response = ExecuteRequest(_restRequest);

//            // Validate the response for the delete request
//            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
//            // Optionally, you can check for an empty response content or null
//            Assert.That(response.Content, Is.Null.Or.Empty);
//        }


//        [Test]
//        public void GetCommentsForPost()
//        {
//            // Set up the API request for retrieving comments for a post (assuming post ID is 1)
//            _restRequest = new RestRequest("/posts/1/comments", Method.Get);


//            // Execute the request
//            var response = ExecuteRequest(_restRequest);

//            // Validate the response
//            ValidateResponse(response, HttpStatusCode.OK);// Execute the request
      
//            // Validate the response for the get comments request
//            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//            Assert.That(response.Content, Is.Not.Null.Or.Empty);

//            // Deserialize the response content to a list of comments
//            List<Comment> comments = JsonConvert.DeserializeObject<List<Comment>>(response.Content);

//            // Validate specific aspects of the retrieved comments
//            Assert.That(comments, Is.Not.Null.And.Not.Empty);

//            foreach (var comment in comments)
//            {
//                // Validate each comment's properties
//                Assert.That(comment.Id, Is.GreaterThan(0));
//                Assert.That(comment.PostId, Is.EqualTo(1)); // Assuming the post ID is 1
//                Assert.That(comment.Name, Is.Not.Null.Or.Empty);
//                Assert.That(comment.Email, Is.Not.Null.Or.Empty);
//                Assert.That(comment.Email, Does.Contain("@")); // Basic email format check
//                Assert.That(comment.Body, Is.Not.Null.Or.Empty);
//                // Add more validations based on the actual structure of your comments
//            }
//        }
//    }


//[TestFixture]
//    public class GoogleBooksApiTests
//    {
//        private const string ApiKey = "YOUR_GOOGLE_BOOKS_API_KEY";
//        private const string BaseUrl = "https://www.googleapis.com/books/v1";

//        [Test]
//        public void SearchBooks_ReturnsResults()
//        {
//            // Arrange
//            var client = new RestClient(BaseUrl);
//            var request = new RestRequest("volumes", Method.Get);
//            request.AddParameter("q", "Harry Potter"); // Your search query

//            // Add API key to the request for authentication
//            request.AddParameter("key", ApiKey);

//            // Act
//            var response = client.Execute(request);

//            // Assert
//            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//            Assert.That(response.IsSuccessful, Is.True);
//            Assert.That(response.Content, Is.Not.Null);
//        }

//        [Test]
//        public void GetBookDetails_ReturnsDetails()
//        {
//            // Arrange
//            var client = new RestClient(BaseUrl);
//            var request = new RestRequest("volumes/{bookId}", Method.Get);
//            request.AddUrlSegment("bookId", "yourBookId"); // Replace with an actual book ID

//            // Add API key to the request for authentication
//            request.AddParameter("key", ApiKey);

//            // Act
//            var response = client.Execute(request);

//            // Assert
//            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//            Assert.That(response.IsSuccessful, Is.True);
//            Assert.That(response.Content, Is.Not.Null);
//        }

//            [Test]
//            public void SearchBooks_WithInvalidQuery_ReturnsBadRequest()
//            {
//                // Arrange
//                var client = new RestClient(BaseUrl);
//                var request = new RestRequest("volumes", Method.Get);
//                request.AddParameter("q", ""); // Invalid query

//                // Add API key to the request for authentication
//                request.AddParameter("key", ApiKey);

//                // Act
//                var response = client.Execute(request);

//                // Assert
//                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest)); //400
//                Assert.That(response.IsSuccessful, Is.False);
//            }

//            [Test]
//            public void GetBookDetails_WithInvalidBookId_ReturnsNotFound()
//            {
//                // Arrange
//                var client = new RestClient(BaseUrl);
//                var request = new RestRequest("volumes/{bookId}", Method.Get);
//                request.AddUrlSegment("bookId", "invalidBookId"); // Invalid book ID

//                // Add API key to the request for authentication
//                request.AddParameter("key", ApiKey);

//                // Act
//                var response = client.Execute(request);

//                // Assert
//                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound)); //404
//                Assert.That(response.IsSuccessful, Is.False);
//            }

//            [Test]
//            public void GetBookDetails_WithoutApiKey_ReturnsUnauthorized()
//            {
//                // Arrange
//                var client = new RestClient(BaseUrl);
//                var request = new RestRequest("volumes/{bookId}", Method.Get);
//                request.AddUrlSegment("bookId", "yourBookId"); // Replace with an actual book ID

//                // Act
//                var response = client.Execute(request);

//                // Assert
//                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized)); //401
//                Assert.That(response.IsSuccessful, Is.False);
//            }

//            [Test]
//            public void GetBookDetails_WithValidBookId_ReturnsDetails()
//            {
//                // Arrange
//                var client = new RestClient(BaseUrl);
//                var request = new RestRequest("volumes/{bookId}", Method.Get);
//                request.AddUrlSegment("bookId", "yourBookId"); // Replace with an actual book ID

//                // Add API key to the request for authentication
//                request.AddParameter("key", ApiKey);

//                // Act
//                var response = client.Execute(request);

//                // Assert
//                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//                Assert.That(response.IsSuccessful, Is.True);
//                Assert.That(response.Content, Is.Not.Null);
//            }
//        }



//}


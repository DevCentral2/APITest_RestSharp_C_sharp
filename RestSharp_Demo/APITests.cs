namespace RestSharp_Demo
{
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RestSharp;
    using System.Net;
    using System.Collections.Generic;



    [TestFixture]
    public partial class ApiTests
    {

        private RestClient? _restClient;
        private RestRequest? _restRequest;


        const string token = "7cc40bfb9e3312eeafa099fe6739a7c8";
        const string ApiClient = "https://favqs.com/api/";

        const int QuotesPerPage = 25;
        const int NumberOfCharsInSessionToken = 88;
      


        // data

        //string userName = "30607";
        //string pswd = "66cde56302f77e14";

        // sample quote
        int quoteId = 248;
        string quoteAuthor = "Mark Twain";
        string quoteBody = "Never let your schooling interfere with your education.";
        
        string quoteKeyword = "funny";

        List<Quote> quotesData = new List<Quote>
        {
            new Quote { Id = 248, Author = "Mark Twain", QuoteText = "Never let your schooling interfere with your education." },
            new Quote { Id = 396, Author = "Abraham Lincoln", QuoteText = "I never had a policy; I have just tried to do my very best each and every day.", },
            new Quote { Id = 434, Author = "Albert Einstein", QuoteText = "The formulation of a problem is often more essential than its solution, which may be merely a matter of mathematical or experimental skill." }
        };



        public class Quote
        {
            public int Id { get; set; }
            public string Author { get; set; }
            public string QuoteText { get; set; }
        }


        [SetUp]
        public void Setup()
        {
            _restClient = new RestClient(ApiClient);
        }

        [Test]
        public void GetQuoteOfTheDayResponse_RequestSuccessful()
        {
            _restRequest = CreateRequest("GET", "qotd");
            AddContentTypeJsonHeader(_restRequest);
            var response = ExecuteRequest(_restRequest);
            ValidateResponseStatusAndErrorCodes(response);
        }

        [Test]
        public void CheckAuthorisedRequest_RequestSuccessful()
        {
            _restRequest = CreateRequest("GET", "quotes");
            AddContentTypeJsonHeader(_restRequest);
            AddAuthorisationTokenHeader(_restRequest, token);
            var response = ExecuteRequest(_restRequest);
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Unauthorized));
            ValidateResponseStatusAndErrorCodes(response);
           }

        [Test]
        public void CheckUnauthorisedRequest_StatusIsUnauthorised()
        {
            _restRequest = CreateRequest("GET", "quotes");
            AddContentTypeJsonHeader(_restRequest);
            var response = ExecuteRequest(_restRequest);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetQuoteOfTheDay_AuthorAndQuoteReturned()
        {
            _restRequest = CreateRequest("GET", "qotd");
            AddContentTypeJsonHeader(_restRequest);
            var response = ExecuteRequest(_restRequest);
            ValidateResponse(response, HttpStatusCode.OK);
            dynamic? jsonResponse = JsonConvert.DeserializeObject(response.Content);
       
            Assert.That(jsonResponse,
                Is.Not.Null, "QuoteResponse is empty or Null");
            Assert.That(jsonResponse.quote.author.ToString(),
                Is.Not.Empty, "Author not found in response");
            Assert.That(jsonResponse.quote.body.ToString(),
                Is.Not.Empty, "Quote not found in response");
        }

        [Test]
        public void GetQuotes_CheckCorrectNumberRetrieved()
        {
            List<QuoteResponse> quotes = GetQuotes("");

            Assert.That(quotes, Is.Not.Null.And.Not.Empty);
            Assert.That(quotes.Count, Is.EqualTo(QuotesPerPage));
        }

        private List<QuoteResponse> GetQuotes(string addToRequestString)
        {
            _restRequest = new RestRequest("quotes"+ addToRequestString, Method.Get);
            AddContentTypeJsonHeader(_restRequest);
            AddAuthorisationTokenHeader(_restRequest, token);
            var response = ExecuteRequest(_restRequest);
            ValidateResponse(response, HttpStatusCode.OK);

            QuotesResponse quotesResponse = JsonConvert.DeserializeObject<QuotesResponse>(response.Content);
            List<QuoteResponse> quotes = quotesResponse.quotes;
            return quotes;
        }

        [Test]
        public void GetPrivateQuotes_CheckExpectedError()
        {
            int expectedError = (int)ErrorCode.UserSessionNotFound;

            _restRequest = CreateRequest("GET", "quotes/?private=1");
            AddContentTypeJsonHeader(_restRequest);
            AddAuthorisationTokenHeader(_restRequest, token);

            var response = ExecuteRequest(_restRequest);
            ValidateResponse(response, HttpStatusCode.OK);

            CheckResponseStatusCodeExpectedError(response, expectedError);
        }


        [Test]
        public void GetSpecificQuotesById_ValidateQuote()
        {
    

            // Access the quotes data
            foreach (var actualQuote in quotesData)
            {
                QuoteResponse quoteResponse = GetSpecificQuote(actualQuote.Id);
                Console.WriteLine($"Id: {actualQuote.Id}, Author: {actualQuote.Author}, Quote: {actualQuote.QuoteText}");
                VerifyQuoteData(quoteResponse, actualQuote);
            }


        }

        private void VerifyQuoteData(QuoteResponse quoteResponse, Quote quote)
        {
          
            Assert.That(quoteResponse, Is.Not.Null);
            Assert.That(quoteResponse.id, Is.EqualTo(quote.Id));
            Assert.That(quoteResponse.author, Is.EqualTo(quote.Author));
            Assert.That(quoteResponse.body, Is.EqualTo(quote.QuoteText));
        }

        [Test]
        public void MarkQuoteAsFavourite_CheckFavouriteIsTrue()
        {
            GetSessionTokenAndAddToClientHeader();
            MarkQuoteAsUnFavourite(quoteId);

            var jsonResponse = 
                MarkQuoteAsFavourite(quoteId);

            Assert.That((bool)jsonResponse.user_details.favorite, Is.True);
        }

        [Test]
        public void MarkQuoteAsUnFavourite_CheckFavouriteIsFalse()
        {
            GetSessionTokenAndAddToClientHeader();
            MarkQuoteAsFavourite(quoteId);
            var jsonResponse = 
                MarkQuoteAsUnFavourite(quoteId);

            Assert.That((bool)jsonResponse.user_details.favorite, Is.False);
        }

        private QuoteResponse MarkQuoteAsFavourite(int quoteId)
        {
            string restRequest = $"quotes/{quoteId}/fav";
            RestRequest _restRequest = CreateRequest("PUT", restRequest);

            AddContentTypeJsonHeader(_restRequest);
            AddAuthorisationTokenHeader(_restRequest, token);

            RestResponse response = ExecuteRequest(_restRequest);
            ValidateResponseStatusAndErrorCodes(response);

            QuoteResponse? quoteResponse = JsonConvert.DeserializeObject<QuoteResponse?>(response.Content);
            return quoteResponse;
        }

        private QuoteResponse MarkQuoteAsUnFavourite(int quoteId)
        { 
         
            string restRequest = $"quotes/{quoteId}/unfav";       // request to unfavourite a specific quote id
            RestRequest _restRequest = CreateRequest("PUT", restRequest);

            AddContentTypeJsonHeader(_restRequest);
            AddAuthorisationTokenHeader(_restRequest, token);

            RestResponse? response = ExecuteRequest(_restRequest);
            ValidateResponseStatusAndErrorCodes(response);

            QuoteResponse? quoteResponse = JsonConvert.DeserializeObject<QuoteResponse>(response.Content);
            return quoteResponse;

        }

        [Test]
        public void CheckFavouritesCountAfterMarkingAsFavourite()
        {

            GetSessionTokenAndAddToClientHeader();

            var jsonResponse = 
                MarkQuoteAsUnFavourite(quoteId);

            // Get favorites count after Un-favouriting a quote -  - this is our initial value
            int favoritesCountAfterUNFavouriting = jsonResponse.favorites_count;

            jsonResponse = 
                MarkQuoteAsFavourite(quoteId);

            // Get favorites count After Un-Favouriting a quote
            int favoritesCountAfterFavouriting = jsonResponse.favorites_count;

            // the Favourites count should be one more after Favouriting the quote
            Assert.That(favoritesCountAfterFavouriting, Is.EqualTo(favoritesCountAfterUNFavouriting + 1)); ;

        }

        [Test]
        public void CheckFavouritesCountAfterMarkingAsUnFavourite()
        {
            GetSessionTokenAndAddToClientHeader();

            var jsonResponse = 
                MarkQuoteAsFavourite(quoteId);
            // Get favorites count After Favouriting a quote - this is our initial value
            int favoritesCountAfterFavouriting = jsonResponse.favorites_count;

            jsonResponse = 
                MarkQuoteAsUnFavourite(quoteId);
            // Get favorites count after Un-favouriting a quote
            int favoritesCountAfterUNFavouriting = jsonResponse.favorites_count;

            // the Favourites count should be one fewer after unfavouriting the quote
            Assert.That(favoritesCountAfterUNFavouriting, Is.EqualTo(favoritesCountAfterFavouriting - 1)); ;

        }

        [Test]
        public void FilterQuotesByAuthor()
        {
            
            string filterAuthor = quoteAuthor.Replace(" ", "+");
      
            List<QuoteResponse> quotes = GetQuotes($"/?filter={filterAuthor}&type=author");

            Assert.That(quotes, Is.Not.Null.And.Not.Empty);
              
            // Check that each quote retrieved includes the author's name
            foreach (QuoteResponse quote in quotes)
            {
                Assert.That(quote.author, Is.Not.Null.And.Not.Empty);
                Assert.That(quote.author, Is.EqualTo(quoteAuthor), $"Incorrect author found in actualQuote list.  Author found is: {quote.body}");
            }
        }

        [Test]
        public void FilterQuotesByKeyword()
        {
            string filterKeyword = quoteKeyword.Replace(" ", "+");

            List<QuoteResponse> quotes = GetQuotes($"/?filter={filterKeyword}");

            Assert.That(quotes, Is.Not.Null.And.Not.Empty);

            // check that each quote retrieved includes the specified keyword
            foreach (QuoteResponse quote in quotes)
            {
                Assert.That(quote.body, Is.Not.Empty);
                Assert.That(quote.body.ToLower().Contains(quoteKeyword.ToLower()), $"Keyword {quoteKeyword} not found in actualQuote. Quote is: {quote.body}");
            }
        }


        [Test]
 
       public void CreateUser_UserExists_ExpectedError()
        {
            RestResponse restResponse;

            //we need to first create the user, to ensure that it exists
            restResponse = CreateUser();
            ValidateResponse(restResponse, HttpStatusCode.OK);

            restResponse = CreateUser();
            ValidateResponse(restResponse, HttpStatusCode.OK);

            // Check for the error code

            // we expect a UserValidation error since this user already exists
            CheckResponseStatusCodeExpectedError(restResponse, (int)ErrorCode.UserValidationErrors);
        }


        [Test]

        public void CreateSession_CheckResponse()
        {  
            RestResponse response = CreateSession();
            ValidateResponseStatusAndErrorCodes(response);

            SessionResponse responseBody = JsonConvert.DeserializeObject<SessionResponse>(response.Content);
            string sessionToken = responseBody.UserToken;

            // one way to identify a valid session token is by checking it contains the expected number of characters
            Assert.That(sessionToken.Count().Equals(NumberOfCharsInSessionToken));

        }


    }
}



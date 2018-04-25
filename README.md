# VPSAuthentication (Amazon S4  in  Web API 2 )


In this article, I will show you how to write an example implementation AWS Auhentification for ASP.NET Web API 2 with simple objects. The project will include both the server side and the client side (using the HttpClient bits of the Web API).

## HMAC AWS 

Hash-based message authentication code (HMAC) is a mechanism for calculating a message authentication code involving a hash function in combination with a secret key. This can be used to verify the integrity and authenticity of a a message.


The following example is from [Amazon S4 documentation](https://docs.aws.amazon.com/fr_fr/general/latest/gr/sigv4-create-canonical-request.html).


```
CanonicalRequest =
  HTTPRequestMethod + '\n' +       : Get/Post/Put/DELETE
  CanonicalURI + '\n' +            : Request Action
  CanonicalQueryString + '\n' +    : Request param
  CanonicalHeaders + '\n' +        : Value of header parameter
  SignedHeaders + '\n' +		   : Name of header parameter
  HexEncode(Hash(RequestPayload))  : Body request hash encoded en hex
  
```

Example canonical request

```
GET https://iam.amazonaws.com/?Action=ListUsers&Version=2010-05-08 HTTP/1.1
Host: iam.amazonaws.com
Content-Type: application/x-www-form-urlencoded; charset=utf-8
X-Amz-Date: 20150830T123600Z

```

```
GET
/
Action=ListUsers&Version=2010-05-08
content-type:application/x-www-form-urlencoded; charset=utf-8
host:iam.amazonaws.com
x-amz-date:20150830T123600Z
content-type;host;x-amz-date
e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855

```




## Web API 2 Common
 
we construct message by concatenating httpMethod,Uri,headers Value,headers Name


```
public interface ICreateCanonicalRequest
{
	 string CreateCanonicalRequest(HttpRequestMessage requestMessage);
}
		
public class CanonicalRequest : ICreateCanonicalRequest
{
	/// <summary>
	/// https://docs.aws.amazon.com/fr_fr/general/latest/gr/sigv4-create-canonical-request.html
	/// Exemple pseudo-code de la demande canonique :
	/// CanonicalRequest =
	/// HTTPRequestMethod + '\n' +
	/// CanonicalURI + '\n' +
	/// CanonicalQueryString + '\n' +
	/// CanonicalHeaders + '\n' +
	/// SignedHeaders + '\n' +
	/// HexEncode(Hash(RequestPayload))
	/// </summary>
	/// <returns></returns>
	public string CreateCanonicalRequest(HttpRequestMessage requestMessage)
	{
		bool valid = IsRequestValid(requestMessage);
		if (!valid)
		{
			return null;
		}

		//for more info about Headers.Date : https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Date
		if (!requestMessage.Headers.Date.HasValue)
		{
			return null;
		}
		DateTime date = requestMessage.Headers.Date.Value.UtcDateTime;

		if (!requestMessage.Headers.Contains(Configuration.HeaderSignatureName))
		{
				return null;
		}

		string email = requestMessage.Headers
					.GetValues(Configuration.HeaderSignatureName).First();

		string httpMethod = requestMessage.Method.Method + "\n";
		string CanonicalURI = requestMessage.RequestUri.AbsoluteUri + "\n";
		string CanonicalHeaders = date.ToString(CultureInfo.InvariantCulture) + ";" + email.ToLower() + "\n";
		string SignedHeaders = "date;" + Configuration.HeaderSignatureName.ToLower() + "\n";

		/// CanonicalRequest =
		/// httpMethod + '\n' +
		/// CanonicalURI + '\n' +
		/// CanonicalHeaders + '\n' +
		/// SignedHeaders + '\n' +
				
		string canonicalRequest = String.Join("\n", httpMethod,
					CanonicalURI,
					CanonicalHeaders, SignedHeaders);

			return canonicalRequest;
}

private bool IsRequestValid(HttpRequestMessage requestMessage)
{
	return true;
}

			
	}
}

```


Calculate signature  with HMACSHA256.

```
	public interface ICalculateSignature
	{
	  string Signature(string secret, string value);
	}
	public class CalculateSignature : ICalculateSignature
	{
		 public string Signature(string secret, string value)
		{
			var secretBytes = Encoding.UTF8.GetBytes(secret);
			var valueBytes = Encoding.UTF8.GetBytes(value);
			string signature;

			using (var hmac = new HMACSHA256(secretBytes))
			{
				var hash = hmac.ComputeHash(valueBytes);
				 signature = Convert.ToBase64String(hash);
			}
			return signature;
		}
	}


```

Configuration header .

```

    public class Configuration
    {
        /// <summary>
        /// Token = mail / hash ...
        /// </summary>
        public const string HeaderSignatureName = "VPS-Token";
       
        public const string Schema = "VPS-HMAC-SHA256";

        public const int PeriodeValiditeEnMinutes = 20;

        public const string ApplicationJson = "application/json";
    }
	

```
User Repository

```

    public interface IUserRepository : IDisposable
    {
        //Check if an User is exists
        bool IsExist(String EmailAdress, string Password);

        //create password hash
        string GenerateAccessTokenUser(string Email);
    }
	
	        ///Dictionary of user used 
        private readonly IDictionary<string, string> _passwords= new Dictionary<string, string>()
           {
                 {"vps@gmail.com","vps12345"},
                 {"nhmd@gmail.com","test123"}
           };

        private bool _disposed;

        /// <summary>
        /// check if user exit
        /// </summary>
        /// <param name="EmailAdress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsExist(String EmailAdress,string password)
        {
            string pass = null;
            return _passwords.TryGetValue(EmailAdress,out pass) && pass.Equals(password);
        }

        /// <summary>
        /// check if user has a password then Calculate Password Hash
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public string GenerateAccessTokenUser(string Email)
        {
            if (!_passwords.ContainsKey(Email))
            {
                return null;
            }

            var userPassword = _passwords[Email];
            var hashed = CalculateHash(userPassword, new SHA1CryptoServiceProvider());
            return hashed;
        }

        /// <summary>
        /// Calculate Password Hash
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        private string CalculateHash(string inputData, HashAlgorithm algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            byte[] hashed = algorithm.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashed);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _passwords.Clear();

                }
                this._disposed = true;
            }
        }
    }
	

```

The Signature will be calculated and attached to the request in a custom message handler.


```

    public class HttpClientRequest : HttpClientHandler
    {

        private readonly IUserRepository _authRepository;
        private readonly ICreateCanonicalRequest _canonicalRequest;
        private readonly ICalculateSignature _calculSignature;

        //Use for Client and UnitTest projects to test ConfidentialsController  
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"> Repository user</param>
        /// <param name="canonicalRequest">allows the creation of the CanonicalRequest </param>
        /// <param name="signatureCalculator"></param>
        public HttpClientRequest(IUserRepository userRepository,
                              ICreateCanonicalRequest canonicalRequest,
                              ICalculateSignature signatureCalculator)
        {
            _authRepository = userRepository;
            _canonicalRequest = canonicalRequest;
            _calculSignature = signatureCalculator;
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                     CancellationToken cancellationToken)
        {
            //add email header 
            if (!request.Headers.Contains(Configuration.HeaderSignatureName))
            {
                request.Headers.Add(Configuration.HeaderSignatureName, Email);
            }

            //set DateTime request header 
            request.Headers.Date = new DateTimeOffset(DateTime.Now, DateTime.Now - DateTime.UtcNow);

            //create CanonicalRequest
            var canonicalRequest = _canonicalRequest.CreateCanonicalRequest(request);

            //Generate accessToken(password hash) with mail
            var accessToken = _authRepository.GenerateAccessTokenUser(Email);

            ///Calculate Signature HMACSHA256
            string signature = _calculSignature.Signature(accessToken, canonicalRequest);

            //create  AuthenticationHeaderValue with shema and signature Authorization = VPS-HMAC-SHA256 signature
            var header = new AuthenticationHeaderValue(Configuration.Schema, signature);

            //Authorization = VPS-HMAC-SHA256 signature
            request.Headers.Authorization = header;

            //send request
            return base.SendAsync(request, cancellationToken);
      
	          }
    }
	
```

The general logic will be that we will want to authenticate each incoming request. The authentication code for each request will be calculated using the same ICreateCanonicalRequest and ICalculateSignature implementations. If the signature does not match, we will immediately return a false answer.

```	
    /// <summary>
    /// 
    /// </summary>
    public class RestAuthentication
    {

        private readonly IUserRepository _authRepository;
        private readonly ICreateCanonicalRequest _canonicalRequest;
        private readonly ICalculateSignature _calculSignature;


        public RestAuthentication(IUserRepository authRepository,
            ICreateCanonicalRequest canonicalRequest,
            ICalculateSignature calculSignature)
        {
            _authRepository = authRepository;
            _canonicalRequest = canonicalRequest;
            _calculSignature = calculSignature;
        }

        public bool IsAuthenticated(HttpRequestMessage requestMessage)
        {

            if (!requestMessage.Headers.Contains(Configuration.HeaderSignatureName) 
                || !requestMessage.Headers.Date.HasValue
                || requestMessage.Headers.Authorization == null
                || requestMessage.Headers.Authorization.Scheme!= Configuration.Schema
                )
            {
                return false;
            }


            //AccessToken= mail passed in the header
            string mail = requestMessage.Headers.GetValues(Configuration.HeaderSignatureName)
                                    .First();
            var accessToken = _authRepository.GenerateAccessTokenUser(mail);
            if (accessToken == null)
            {
                return false;
            }

            //test if the date is valid
            if(!IsDateOk(requestMessage))
            {
                return false;
            }

            //create CanonicalRequest 
            var canonicalRequest = _canonicalRequest.CreateCanonicalRequest(requestMessage);
            if (canonicalRequest == null)
            {
                return false;
            }


            var signature = _calculSignature.Signature(accessToken, canonicalRequest);

            //check if the signature is the same as signature calculated
            return requestMessage.Headers.Authorization.Parameter == signature;
        }

        private bool IsContentType(HttpRequestMessage requestMessage)
        {
            var ContentTypeHeader = requestMessage.Content.Headers.ContentType.MediaType;

            return ContentTypeHeader == Configuration.ApplicationJson;

        }

    private bool IsDateOk(HttpRequestMessage requestMessage)
    {
        var utcNow = DateTime.UtcNow;
        var date = requestMessage.Headers.Date.Value.UtcDateTime;
        if (date >= utcNow.AddMinutes(Configuration.PeriodeValiditeEnMinutes)
            || date <= utcNow.AddMinutes(-Configuration.PeriodeValiditeEnMinutes))
        {
            xreturn false;
        }
        return true;

    }
}
```
## Web API 2 Web


```

 public class ConfidentialsController : ApiController
    {
        // GET api/Confidentials : EMail in header : VPS-Token:email
        //  
        [HttpGet, Route("api/confidentials")]
        public bool Confidentials()
        {
            RestAuthentication restAuthentication = new RestAuthentication(new UserRepository(),
                               new CanonicalRequest(), new CalculateSignature());
            return restAuthentication.IsAuthenticated(this.Request);
        }
    }
	
```


## Web API 2 Client for Test

Here is how we would make a request from console App:

```
        [TestMethod]
        public void TestAccessOK()
        {
            HttpClientRequest httpClientRequest = new HttpClientRequest(new UserRepository(),
                                       new CanonicalRequest(), new CalculateSignature());
            httpClientRequest.Email = "vps@gmail.com";

            var client = new HttpClient(httpClientRequest);

            bool result = client.GetStringAsync("http://localhost:53085/api/confidentials").GetAwaiter().GetResult()== "true" ? true:false;
            Assert.Equals(result, true);
        }
		

```		

	

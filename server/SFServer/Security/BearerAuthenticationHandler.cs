using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Security.Claims;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using SFServer.Models.DB;
using SFServer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SFServer.Services;
using Microsoft.AspNetCore.Http.Extensions;
using SFServer.Utility;
using Chumpware.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.IO;

namespace SFServer.Security
{
    public class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string BearerSchemeName = "Bearer";

        private readonly PostgreSqlContext _db;
        private TokenService _tokenService;
        private ILogger<BearerAuthenticationHandler> _logger;

        private static Dictionary<string, DateTimeOffset> cachedTokens = new Dictionary<string, DateTimeOffset>();

        private bool IsTokenFresh(string token)
        {
            string md5 = CreateMD5(token);

            bool fresh = !cachedTokens.ContainsKey(md5);

            if(fresh)
                cachedTokens.Add(md5, DateTimeOffset.Now);

            List<string> expiredTokens = cachedTokens.Where(x => x.Value < DateTimeOffset.Now.AddMinutes(-2)).Select(x => x.Key).ToList();
            expiredTokens.ForEach(x =>
            {
                cachedTokens.Remove(x);
            });

            return fresh;
        }

        public BearerAuthenticationHandler(PostgreSqlContext db, IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, TokenService tokenService) : base(options, logger, encoder, clock)
        {
            _db = db;
            _tokenService = tokenService;
            _logger = logger.CreateLogger<BearerAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                //Console.WriteLine($"Authorization header not in request: {Request.GetEncodedUrl()}");
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                Console.WriteLine($"Invalid Authorization header: {Request.GetEncodedUrl()}");
                _logger.LogInformation($"Invalid Authorization header: {Request.GetEncodedUrl()}");
                return AuthenticateResult.NoResult();
            }

            if (!BearerSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Not Bearer authentication header: {Request.GetEncodedUrl()}");
                _logger.LogInformation($"Not Bearer authentication header: {Request.GetEncodedUrl()}");
                return AuthenticateResult.NoResult();
            }

            string json = StringCipher.Decrypt(headerValue.Parameter, "P1*Vc9otb0%uJo%rMU2KWU3!tsI4CP0U@&*jlgsEoePnw*ej7GCI^Nb2a5L7To#h", 128);

            JObject jObject = JsonConvert.DeserializeObject<JObject>(json);

            if(jObject == null)
            {
                Console.WriteLine($"jObject could not be decoded!:\n{json}");
                _logger.LogInformation($"jObject could not be decoded!:\n{json}");
                return AuthenticateResult.Fail("Invalid authentication header: Token could not be decoded!");
            }
            
            if (!jObject.TryGetValue("token", out JToken tokenJToken))
            {
                Console.WriteLine($"token not found in jObject!:\n{json}");
                _logger.LogInformation($"token not found in jObject!:\n{json}");
                return AuthenticateResult.Fail("Invalid authentication header: Token could not be decoded!");
            }

            if (!jObject.TryGetValue("checksum", out JToken checksumJToken))
            {
                Console.WriteLine($"checksum not found in jObject!:\n{json}");
                _logger.LogInformation($"checksum not found in jObject!:\n{json}");
                return AuthenticateResult.Fail("Invalid authentication header: Token could not be decoded!");
            }

            Token token = _tokenService.DecodeToken(tokenJToken.ToObject<string>());

            if(token == null)
            {
                Console.WriteLine($"Token could not be decoded!: {Request.GetEncodedUrl()}");
                _logger.LogInformation($"Token could not be decoded!: {Request.GetEncodedUrl()}");
                return AuthenticateResult.Fail("Invalid authentication header: Token could not be decoded!");
            }

            if (!jObject.TryGetValue("tokenTime", out JToken tokenTimeJToken))
            {
                Console.WriteLine($"tokenTime not found in jObject!:\n{json}");
                _logger.LogInformation($"tokenTime not found in jObject!:\n{json}");
                return AuthenticateResult.Fail("Invalid authentication header: Token could not be decoded!");
            }

            Client client = await _db.Clients.SingleOrDefaultAsync(x => x.ClientId == token.ClientId);
            if (client == null)
            {
                Console.WriteLine("Client not found. User does probably not exsist.");
                _logger.LogInformation("Client not found. User does probably not exsist.");
                return AuthenticateResult.Fail("User does not exsist.");
            }

            if (client.Banned)
            {
                Console.WriteLine("Client Banned");
                _logger.LogInformation("Client Banned");
                return AuthenticateResult.Fail("Client Banned");
            }

            var user = await _db.Users.Include(x => x.Tokens).SingleOrDefaultAsync(x => x.UserId == token.UserId);

            if (user == null)
            {
                Console.WriteLine("User not found");
                _logger.LogInformation("User not found");
                return AuthenticateResult.Fail("User not found");
            }

            if (!user.Tokens.Any(x => x.TokenString == token.TokenString))
            {
                Console.WriteLine("Invalid token");
                _logger.LogInformation("Invalid token");
                return AuthenticateResult.Fail("Invalid token");
            }
                
            if(user.Banned)
            {
                Console.WriteLine("User Banned");
                _logger.LogInformation("User Banned");
                return AuthenticateResult.Fail("User Banned");
            }

            DateTimeOffset tokenTime = tokenTimeJToken.ToObject<DateTimeOffset>();
            DateTimeOffset serverTime = DateTimeOffset.Now.ToUniversalTime();

            if (tokenTime < serverTime.AddMinutes(-1) || tokenTime > serverTime.AddMinutes(1)) //Tokens need to be created +- a minute
            {
                Console.WriteLine($"Outdated token:\nServer Time: {serverTime}\nTokenTime: {tokenTime}");
                _logger.LogInformation($"Outdated token:\nServer Time: {serverTime}\nTokenTime: {tokenTime}");
                return AuthenticateResult.Fail("Invalid token");
            }

            if(!IsTokenFresh(headerValue.Parameter))
            {
                Console.WriteLine($"token has been used before");
                _logger.LogInformation($"token has been used before");
                return AuthenticateResult.Fail("Invalid token");
            }

            string jsonString = "";
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
            {
                jsonString = await reader.ReadToEndAsync();
                Request.Body.Seek(0, SeekOrigin.Begin);
            }

            string checksum = CreateMD5(jsonString);
            string tokenChecksum = checksumJToken.ToObject<string>();

            if (!checksum.Equals(tokenChecksum, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"token checksum mismatch");
                _logger.LogInformation($"token checksum mismatch");
                return AuthenticateResult.Fail("Invalid token");
            }

            string version = Request.Headers["ClientVersion"];
            if(ClientVersion.Parse(version, out ClientVersion clientVersion))
            {
                Request.HttpContext.Items["clientVersion"] = clientVersion;
            }

            Request.HttpContext.Items["user"] = user;
            Request.HttpContext.Items["token"] = token;
            
            client.UserId = token.UserId; //Temp migration can be removed later.

            //Save user activity
            _db.UpdateUser(user);
            _db.UpdateClient(client);
            //user.LastActive = DateTimeOffset.Now;
            //_db.Users.Update(user);
            await _db.SaveChangesAsync();

            var claims = new[] { new Claim(ClaimTypes.Name, user.UserId), new Claim(ClaimTypes.Role, user.Role.ToString()) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}

using Jose;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFServer.Models;
using SFServer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DB;

namespace SFServer.Services
{
    public class TokenService
    {
        static ILogger _logger;

        public TokenService(ILogger<TokenService> logger)
        {
            _logger = logger;
        }

        public Token GenerateToken(User user, string clientId)
        {
            var payload = new Dictionary<string, object>
            {
                { "UserId", user.UserId },
                { "ClientId", clientId }
            };

            var tokenString = JWT.Encode(payload, Startup.Configuration["AUTHENTICATION_TOKEN_KEY"], JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512, JweCompression.DEF);
            var newToken = new Token(user.UserId, clientId, tokenString);

            return newToken;
        }

        public Token DecodeToken(string tokenString)
        {
            var token = new Token();
            string json = null;

            try
            {
                json = JWT.Decode(tokenString, Startup.Configuration["AUTHENTICATION_TOKEN_KEY"]);
            }
            catch
            {
                _logger.LogError("Decoding token failed!");
                return null;
            }
            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            if (!payload.ContainsKey("UserId"))
            {
                _logger.LogError("Missing UserId Key");
                return null;
            }

            token.UserId = Convert.ToString(payload["UserId"]);
            token.ClientId = Convert.ToString(payload["ClientId"]);
            token.TokenString = tokenString;

            if (string.IsNullOrWhiteSpace(token.UserId))
            {
                _logger.LogError("UserId NullOrWhiteSpace");
                return null;
            }

            return token;
        }
    }
}

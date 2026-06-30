using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFServer.Contexts;
using SFServer.Models;
using SFServer.Security;
using Microsoft.AspNetCore.Authorization;
using SFServer.Utility;
using SFServer.Services;
using SFServer.Filters;
using Microsoft.EntityFrameworkCore;
using SFServer.Models.DTO;
using SFServer.Models.Notifications;
using SFServer.Models.Requests.UserRequests;
using SFServer.Models.Requests.Misc;
using SFServer.Models.Responses.UserResponses;
using SFServer.Models.Enums;
using SFServer.Models.DB;
using SFServer.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Hosting;
using RazorLight;
using System.IO;
using Microsoft.Extensions.Options;
using SFServer.Models.Configs;
using SFServer.Configs;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly PostgreSqlContext _db;
        private ClientSecretAuthenticator _clientSecretAuthenticator;
        private NotificationService _notificationService;
        private TokenService _tokenService;
        private IWebHostEnvironment _env;
        private RazorLightEngine _razorEngine;

        private readonly VersionConfig _versionConfig;
        private readonly GameConfig _gameConfig;

        private RewardService _rewardService;
        private LevelStatsService _levelStatsService;
        private EndlessService _endlessService;

        public UserController(PostgreSqlContext db, ClientSecretAuthenticator clientSecretAuthenticator, IWebHostEnvironment env, NotificationService notificationService, EndlessService endlessService, TokenService tokenService, IOptions<VersionConfig> versionConfig, IOptions<GameConfig> gameConfig, RewardService rewardService, LevelStatsService levelStatsService)
        {
            _db = db;
            _env = env;
            _versionConfig = versionConfig.Value;
            _gameConfig = gameConfig.Value;
            _clientSecretAuthenticator = clientSecretAuthenticator;
            _notificationService = notificationService;
            _tokenService = tokenService;
            _rewardService = rewardService;
            _levelStatsService = levelStatsService;
            _endlessService = endlessService;

            _razorEngine = new RazorLightEngineBuilder()
               .UseFilesystemProject(Path.Combine(env.WebRootPath, "Templates"))
               .UseMemoryCachingProvider()
               .Build();
        }

        private string GetShareUrl(string Nickname, int Identifier)
        {
            Startup.WebHostEnvironment.IsDevelopment();
            string Scheme = _env.IsDevelopment() ? "http" : "https";
            return $"{Scheme}://{this.Request.Host}/{Nickname}/{Identifier}";
        }

        [ModelValidation]
        [ServiceFilter(typeof(ClientSecretValidationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] NewUserRequest newUserRequest)
        {
            if (newUserRequest.Nickname.Length < 2 || newUserRequest.Nickname.Length > 12)
                return BadRequest("Nicknames are limited to 2-12 characters.");
             
            string randomId = IdGenerator.Generate();
            while (await _db.Users.FindAsync(randomId) != null)
                randomId = IdGenerator.Generate();

            //Find all other users with same name
            List<int> otherIdentifiers = await _db.Users.Where(x => x.Nickname == newUserRequest.Nickname).Select(x => x.Identifier).ToListAsync<int>();
            int randomIdentifier;
            Random random = new Random();
            int maxValue = (int)Math.Max(1000, Math.Pow(10, otherIdentifiers.Count.ToString().Length));
            randomIdentifier = random.Next(maxValue);

            while (otherIdentifiers.Contains(randomIdentifier))
                randomIdentifier = random.Next(maxValue);

            User newUser = new User
            {
                UserId = randomId,
                Nickname = newUserRequest.Nickname,
                Identifier = randomIdentifier,
                Color = ColorUtility.GetRandomHexColor(),
                Role = UserRole.Player,
                RestoreToken = RandomTokenGenerator.Generate(),
                AgreedToTermsOfService = DateTimeOffset.Now, //remove this once 0.90 is out.
                SaveData = new UserSaveData(randomId)
            };

            _db.TermsOfServiceAgreement.Add(
                new TermsOfServiceAgreement
                {
                    UserId = newUser.UserId,
                    Agreed = true
                });

            _db.PrivacyPolicyAgreement.Add(
                new PrivacyPolicyAgreement
                {
                    UserId = newUser.UserId,
                    Agreed = true
                });

            //ClientSecretValidationAttribute garantees this headers presence.
            string ClientId = this.Request.Headers["ClientId"];

            //Create a new token for user
            Token token = _tokenService.GenerateToken(newUser, ClientId);
            newUser.Tokens.Add(token);

            _db.Users.Add(newUser);

            if (await _db.Clients.FindAsync(ClientId) == null)
                _db.Clients.Add(new Client(ClientId, newUser.UserId));

            string title = $"Hey {newUserRequest.Nickname}! (Tap to read)";
            string body = "Welcome to Safari Forever! If you haven't had the chance yet, check out our Discord server!";
            await _notificationService.CreateNotification(newUser.UserId, title, body, NotificationType.DontSendPushNotification, "", new List<NotificationLink>() { new NotificationLink("Join Discord", "https://discord.gg/AVwnQgM") });



            await _db.SaveChangesAsync();

            var response = new NewUserResponse(newUser, token.TokenString);

            return Ok(response);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPatch("update-nickname")]
        public async Task<IActionResult> UpdateNickname([FromBody] ChangeNicknameRequest changeNicknameRequest)
        {
            if (changeNicknameRequest.Nickname.Length < 2 || changeNicknameRequest.Nickname.Length > 12)
                return BadRequest("Nicknames are limited to 2-12 characters.");

            User user = (User)Request.HttpContext.Items["user"];

            //Find all other users with same name
            List<int> otherIdentifiers = await _db.Users.Where(x => x.Nickname == changeNicknameRequest.Nickname).Select(x => x.Identifier).ToListAsync<int>();

            int randomIdentifier;
            Random random = new Random();
            int maxValue = (int)Math.Max(1000, Math.Pow(10, otherIdentifiers.Count.ToString().Length));
            randomIdentifier = random.Next(maxValue);
            while (otherIdentifiers.Contains(randomIdentifier))
                randomIdentifier = random.Next(maxValue);

            user.Nickname = changeNicknameRequest.Nickname;
            user.Identifier = randomIdentifier;

            _db.UpdateUser(user);
            await _db.SaveChangesAsync();

            var response = new ChangeNicknameResponse(user.Nickname, user.Identifier);
            return Ok(response);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPatch("update-color")]
        public async Task<IActionResult> UpdateColor([FromBody] ChangeColorRequest changeColorRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            user.Color = changeColorRequest.Color;

            _db.UpdateUser(user);
            await _db.SaveChangesAsync();

            return Ok(new ChangeColorResponse(user.Color));
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPatch("update-terms-of-service-agreement")]
        public async Task<IActionResult> UpdateTermsOfServiceAgreement([FromBody] UpdateTermsOfServiceAgreementRequest request)
        {
            User user = (User)Request.HttpContext.Items["user"];

            TermsOfServiceAgreement tOS = await _db.TermsOfServiceAgreement.Where(x => x.UserId == user.UserId).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();

            _db.TermsOfServiceAgreement.Add(
                new TermsOfServiceAgreement
                {
                    UserId = user.UserId,
                    Agreed = request.Accept
                });

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPatch("update-privacy-policy-agreement")]
        public async Task<IActionResult> UpdatePrivacyPolicyAgreement([FromBody] UpdatePrivacyPolicyAgreementRequest request)
        {
            User user = (User)Request.HttpContext.Items["user"];

            PrivacyPolicyAgreement pPA = await _db.PrivacyPolicyAgreement.Where(x => x.UserId == user.UserId).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();

            _db.PrivacyPolicyAgreement.Add(
                new PrivacyPolicyAgreement
                {
                    UserId = user.UserId,
                    Agreed = request.Accept
                });

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("get-terms-of-service-agreement-status")]
        public async Task<IActionResult> GetTermsOfServiceAgreementStatus()
        {
            User user = (User)Request.HttpContext.Items["user"];
            ClientVersion version = (ClientVersion)Request.HttpContext.Items["clientVersion"];

            if(version.IsNewerThanOrEqual(new ClientVersion(0, 90)))
            {
                TermsOfServiceAgreement tOS = await _db.TermsOfServiceAgreement.Where(x => x.UserId == user.UserId).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();

                if (tOS == null)
                    return Ok(false);

                return Ok(tOS.CreatedOn > _versionConfig.TermsOfServiceAgreementUpdated && tOS.Agreed == true);
            }
            else
            {
                return Ok(user.AgreedToTermsOfService.HasValue && user.AgreedToTermsOfService.Value > _versionConfig.TermsOfServiceAgreementUpdated);
            }
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [HttpGet("get-privacy-policy-agreement-status")]
        public async Task<IActionResult> GetPrivacyPolicyAgreementStatus()
        {
            User user = (User)Request.HttpContext.Items["user"];

            PrivacyPolicyAgreement pPA = await _db.PrivacyPolicyAgreement.Where(x => x.UserId == user.UserId).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();

            if (pPA == null)
                return Ok(false);

            return Ok(pPA.CreatedOn > _versionConfig.PrivacyPolicyUpdated && pPA.Agreed == true);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [HttpGet("rightToAccessRequest")]
        public async Task<IActionResult> RequestRightToAccess()
        {
            User user = (User)Request.HttpContext.Items["user"];

            List<LevelUserStats> userStats = await _db.LevelUserStats.Where(x => x.UserId == user.UserId).ToListAsync();

            await _db.Entry(user).Collection(p => p.Levels).LoadAsync();

            await _db.Entry(user).Collection(p => p.Following).LoadAsync();

            List<TermsOfServiceAgreement> tOSStatus = await _db.TermsOfServiceAgreement.Where(x => x.UserId == user.UserId).ToListAsync();

            List<PrivacyPolicyAgreement> pPStatus = await _db.PrivacyPolicyAgreement.Where(x => x.UserId == user.UserId).ToListAsync();

            List<Transaction> transactions = await _db.Transactions.Where(x => x.UserId == user.UserId).ToListAsync();

            return Ok(new RightToAccessRequest(user, userStats, tOSStatus, pPStatus, transactions));
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("authorized")]
        public IActionResult IsAuthorized()
        {
            User user = (User)Request.HttpContext.Items["user"];
            return Ok(user.UserId);
        }

        [ModelValidation]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("restore")]
        public async Task<IActionResult> RestoreProfile([FromBody] RestoreUserRequest restoreUserRequest)
        {
            User userToRestore = await _db.Users.FirstOrDefaultAsync(x => x.RestoreToken == restoreUserRequest.RestoreToken);

            if (userToRestore == null)
                return BadRequest("Invalid Restore Token");

            //Create a new token for user
            Token newToken = _tokenService.GenerateToken(userToRestore, restoreUserRequest.ClientId);
            userToRestore.Tokens.Add(newToken);

            if (await _db.Clients.FindAsync(restoreUserRequest.ClientId) == null)
                _db.Clients.Add(new Client(restoreUserRequest.ClientId, userToRestore.UserId));

            _db.UpdateUser(userToRestore);
            await _db.SaveChangesAsync();

            var response = new ProfileRestoreResponse(userToRestore.UserId, newToken.TokenString, userToRestore.Nickname, userToRestore.Identifier, userToRestore.Color, userToRestore.Coins);
            return Ok(response);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("enable-restore")]
        public async Task<IActionResult> EnableCloudRestore()
        {
            User user = (User)Request.HttpContext.Items["user"];

            if (!string.IsNullOrWhiteSpace(user.RestoreToken))
            {
                return Ok(user.RestoreToken);
            }

            user.RestoreToken = RandomTokenGenerator.Generate();
            _db.UpdateUser(user);

            await _db.SaveChangesAsync();

            return Ok(user.RestoreToken);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("upload-backup")]
        public async Task<IActionResult> BackupSaveData([FromBody] SaveDataDTO saveDataBackUpDTO)
        {
            User user = (User)Request.HttpContext.Items["user"];

            await _db.Entry(user).Reference(x => x.SaveData).LoadAsync();

            if (user.SaveData == null)
                user.SaveData = new UserSaveData(user.UserId);

            user.SaveData.Merge(saveDataBackUpDTO);

            _db.UpdateSaveData(user, user.SaveData);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        //[ServiceFilter(typeof(LegalVerificationAttribute))] Dont require legal verification to download data.
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("download-backup")]
        public async Task<IActionResult> DownloadBackupSaveData()
        {
            User user = (User)Request.HttpContext.Items["user"];

            await _db.Entry(user).Reference(x => x.SaveData).LoadAsync();

            if (user.SaveData == null)
            {
                user.SaveData = new UserSaveData(user.UserId);
                _db.UpdateUser(user);
                await _db.SaveChangesAsync();
            }

            await _db.Entry(user.SaveData).Collection(x => x.CampaignLevelData).LoadAsync();

            List<string> playedLevels = await _db.LevelUserStats.Where(x => x.UserId == user.UserId && (x.Seconds >= 0 || x.Milliseconds >= 0)).Select(x => x.LevelId).ToListAsync();

            return Ok(new SaveDataDTO(user) { LevelsPlayed = playedLevels });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("{userId}/profile")]
        public async Task<IActionResult> GetProfile(string userId)
        {
            User userToFind = await _db.Users.FindAsync(userId);

            if (userToFind == null)
                return NotFound();

            User user = (User)Request.HttpContext.Items["user"];

            bool isFollowed = false;
            if (userToFind.UserId != user.UserId)
            {
                Following follow = await _db.Followings.FirstOrDefaultAsync(x => x.UserFollowingId == user.UserId && x.UserBeingFollowedId == userToFind.UserId);
                isFollowed = follow != null && follow.Active;
            }

            var result = await _db.Levels
                .Where(x => x.Blacklisted == false && x.VerifiedUpload)
                .Select(x => new LevelInfoDTO()
                {
                    LevelId = x.LevelId,
                    Name = x.Name,
                    CreatorUserId = x.CreatorUserId,
                    Creator = userToFind.Nickname,
                    GameVersion = x.GameVersion,
                    CreatedOn = x.CreatedOn,
                    Thumbnail = x.Thumbnail,
                    MiniThumbnail = x.MiniThumbnail,
                    CoinsInvested = x.CoinsInvested,
                    Likes = x.Likes,
                    Plays = x.Plays,
                    CanBeBoosted = x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                    HasGraduated = x.Plays >= _gameConfig.MaxPlaysInNewCategory,
                    RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                    Difficulty = Difficulty.Unrated
                })
                .Where(x => x.CreatorUserId == userId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(0)
                .Take(4)
                .ToListAsync();

            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            if (user.UserId != userToFind.UserId)
            {
                //Player requests someone else.
                userToFind.Coins = -1;
                userToFind.LifetimeCoins = -1;
            }

            int endlessRank = await _endlessService.GetRank(userToFind);

            return Ok(new ProfileResponse(userToFind, result, isFollowed, GetShareUrl(userToFind).Replace("api2", "user"), endlessRank));
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [ModelValidation]
        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromBody] FollowRequest followRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            if (user.UserId == followRequest.UserToFollow)
                return StatusCode(403, "You cant follow yourself!");

            User userToFollow = await _db.Users.FindAsync(followRequest.UserToFollow);

            if (userToFollow == null)
                return BadRequest("Cant find user with that Id!");

            //Check if user is already followed!
            Following follow = await _db.Followings.FirstOrDefaultAsync(x => x.UserFollowingId == user.UserId && x.UserBeingFollowedId == userToFollow.UserId);

            if (follow != null)
            {
                //User previously followed!

                if(follow.Active)
                    return BadRequest("User already followed!");

                follow.Active = true;
                _db.UpdateFollowing(follow);

                await _db.SaveChangesAsync();

                userToFollow.FollowerCount = await _db.Followings.Where(x => x.UserBeingFollowedId == userToFollow.UserId && x.Active).CountAsync();
                _db.UpdateUser(userToFollow);
            }
            else
            {
                _db.Followings.Add(new Following()
                {
                    UserBeingFollowedId = userToFollow.UserId,
                    UserFollowingId = user.UserId,
                    Active = true
                });

                await _db.SaveChangesAsync();

                userToFollow.FollowerCount = await _db.Followings.Where(x => x.UserBeingFollowedId == userToFollow.UserId && x.Active).CountAsync();
                _db.UpdateUser(userToFollow);

                string title = $"New follower";
                string body = $"{user.Nickname} started following you";
                string deeplinkUrl = $"https://user.safariforever.com/{user.Nickname}/{user.Identifier}"; 
                await _notificationService.CreateNotification(userToFollow.UserId, title, body, NotificationType.NewFollower, deeplinkUrl, new List<NotificationLink>() { new NotificationLink("View Profile", deeplinkUrl) });
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [ModelValidation]
        [HttpPost("unfollow")]
        public async Task<IActionResult> Unfollow([FromBody] UnfollowRequest unfollowRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            if (user.UserId == unfollowRequest.UserToUnfollow)
                return StatusCode(403, "You cant follow yourself!");

            User userToUnfollow = await _db.Users.FindAsync(unfollowRequest.UserToUnfollow);

            if (userToUnfollow == null)
                return BadRequest("Cant find user with that Id!");

            //Check if user is already followed!
            Following follow = await _db.Followings.FirstOrDefaultAsync(x => x.UserFollowingId == user.UserId && x.UserBeingFollowedId == userToUnfollow.UserId);

            if (follow == null)
                return BadRequest("You dont follow this user");

            if (!follow.Active)
                return BadRequest("You dont follow this user");

            follow.Active = false;
            _db.UpdateFollowing(follow);

            await _db.SaveChangesAsync();

            userToUnfollow.FollowerCount = await _db.Followings.Where(x => x.UserBeingFollowedId == userToUnfollow.UserId && x.Active).CountAsync();
            _db.UpdateUser(userToUnfollow);

            await _db.SaveChangesAsync();

            return Ok();
        }


        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [ModelValidation]
        [HttpPost("followings")]
        public async Task<IActionResult> Followings([FromBody] FollowedByUserRequest followedByUserRequest)
        {
            if (followedByUserRequest.FromIndex < 0)
                return BadRequest("FromIndex outside of scope");

            User user = (User)Request.HttpContext.Items["user"];

            if (user == null)
                return NotFound();

            var following = await _db.Followings
                .Where(x => x.UserFollowingId == user.UserId && x.Active)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(followedByUserRequest.FromIndex)
                .Take(_gameConfig.UsersPrPage)
                .ToListAsync();

            FollowedUsersResponse response = new FollowedUsersResponse();
            response.UsersPrPage = _gameConfig.UsersPrPage;
            foreach (Following follow in following)
            {
                User followedUser = await _db.Users.FindAsync(follow.UserBeingFollowedId);

                if (followedUser == null)
                    continue;

                response.followedUsers.Add(new FollowedUserDTO()
                {
                    UserId = followedUser.UserId,
                    Nickname = followedUser.Nickname,
                    Identifier = followedUser.Identifier,
                    Color = followedUser.Color,
                    LastActive = followedUser.UpdatedOn
                });
            }

            return Ok(response);
        }


        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("follower-count")]
        public IActionResult FollowerCount()
        {
            User user = (User)Request.HttpContext.Items["user"];

            return Ok(new FollowerCountResponse()
            {
                Count = user.FollowerCount
            });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [ModelValidation]
        [HttpPost("register-fcm-token")]
        public async Task<IActionResult> RegisterFCMToken([FromBody] RegisterFCMTokenRequest registerFCMTokenRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];
            Token token = (Token)Request.HttpContext.Items["token"];

            Client client = await _db.Clients.FirstOrDefaultAsync(x => x.ClientId == token.ClientId);

            if (client == null)
                return NotFound();

            client.UserId = user.UserId;
            client.FCMToken = registerFCMTokenRequest.FCMToken;

            _db.UpdateClient(client);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [ModelValidation]
        [HttpPost("register-client")]
        public async Task<IActionResult> RegisterClient()
        {
            User user = (User)Request.HttpContext.Items["user"];
            Token token = (Token)Request.HttpContext.Items["token"];

            Client client = await _db.Clients.FirstOrDefaultAsync(x => x.ClientId == token.ClientId);

            if(client == null)
            {
                client = new Client(token.ClientId, user.UserId);
                _db.Clients.Add(client);
            }
            else
            {
                //Client already exsist update with userId Instead.
                client.UserId = user.UserId;
                _db.UpdateClient(client);
            }  

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("bananas")]
        public async Task<IActionResult> GetBananaCount(string nick, int id)
        {
            if (string.IsNullOrWhiteSpace(nick))
                return BadRequest();

            if (id == 0)
                return BadRequest();

            User user = await _db.Users.Where(x => x.Nickname.ToLower() == nick && x.Identifier == id).FirstOrDefaultAsync();

            if (user == null)
                return StatusCode(404, "User not found");

            return Ok(user.Coins);
        }

        [NoCache]
        [HttpGet("~/{nickname}/{identifier}")]
        public async Task<ActionResult> PreviewProfile(string nickname, int identifier)
        {
            User user = await _db.Users.Where(x => x.Nickname == nickname && x.Identifier == identifier).FirstOrDefaultAsync();
            if (user == null)
                return StatusCode(404, $"User not found {nickname}#{identifier}");

            List<string> levels = await _db.Levels
                .Where(x => x.CreatorUserId == user.UserId && x.Blacklisted == false && x.VerifiedUpload)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(0)
                .Take(4)
                .Select(x => x.LevelId)
                .ToListAsync();

            string displayUrl = Request.GetEncodedPathAndQuery();
            string playButtonUrl = displayUrl.Contains("api2.safariforever.com", StringComparison.OrdinalIgnoreCase) ? "https://safariforever.com" : GetShareUrl(user.Nickname, user.Identifier).Replace("user", "api2");

            var cacheResult = _razorEngine.TemplateCache.RetrieveTemplate("UserCodePreview.cshtml");

            string result = string.Empty;
            if (cacheResult.Success)
                result = await _razorEngine.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), new UserViewDTO(user, levels, playButtonUrl));
            else
                result = await _razorEngine.CompileRenderAsync("UserCodePreview.cshtml", new UserViewDTO(user, levels, playButtonUrl));

            return new ContentResult()
            {
                Content = result,
                ContentType = "text/html",
            };
        }


        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("{nickname}/{identifier}/profile")]
        public async Task<IActionResult> GetProfile(string nickname, int identifier)
        {
            User userToFind = await _db.Users.Where(x => x.Nickname == nickname && x.Identifier == identifier).FirstOrDefaultAsync();
            if (userToFind == null)
                return StatusCode(404, $"User not found {nickname}#{identifier}");

            User user = (User)Request.HttpContext.Items["user"];

            bool isFollowed = false;
            if (userToFind.UserId != user.UserId)
            {
                Following follow = await _db.Followings.FirstOrDefaultAsync(x => x.UserFollowingId == user.UserId && x.UserBeingFollowedId == userToFind.UserId);
                isFollowed = follow != null && follow.Active;
            }

            var result = await _db.Levels
                .Where(x => x.Blacklisted == false && x.VerifiedUpload)
                .Select(x => new LevelInfoDTO()
                {
                    LevelId = x.LevelId,
                    Name = x.Name,
                    CreatorUserId = x.CreatorUserId,
                    Creator = userToFind.Nickname,
                    GameVersion = x.GameVersion,
                    CreatedOn = x.CreatedOn,
                    Thumbnail = x.Thumbnail,
                    MiniThumbnail = x.MiniThumbnail,
                    CoinsInvested = x.CoinsInvested,
                    Likes = x.Likes,
                    Plays = x.Plays,
                    CanBeBoosted = x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && x.VerifiedUpload && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                    HasGraduated = x.Plays >= _gameConfig.MaxPlaysInNewCategory,
                    RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                    Difficulty = Difficulty.Unrated
                })
                .Where(x => x.CreatorUserId == userToFind.UserId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(0)
                .Take(4)
                .ToListAsync();

            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            if (user.UserId != userToFind.UserId)
            {
                //Player requests someone else.
                userToFind.Coins = -1;
                userToFind.LifetimeCoins = -1;
            }

            int endlessRank = await _endlessService.GetRank(userToFind);

            return Ok(new ProfileResponse(userToFind, result, isFollowed, GetShareUrl(userToFind).Replace("api2", "user"), endlessRank));
        }

        private string GetShareUrl(User user)
        {
            string Scheme = _env.IsDevelopment() ? "http" : "https";
            return $"{Scheme}://{this.Request.Host}/{user.Nickname}/{user.Identifier.ToString().PadLeft(4,'0')}";
        }

        private string GetShortShareUrl(User user)
        {
            if (_env.IsDevelopment())
                return GetShareUrl(user);
            else
                return $"https://sf4.life/u/{user.Nickname}/{user.Identifier.ToString().PadLeft(4, '0')}";
        }
    }
}

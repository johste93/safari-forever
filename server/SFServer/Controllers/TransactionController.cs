using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFServer.Contexts;
using SFServer.Filters;
using SFServer.Models.DB;
using SFServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.Configs;
using SFServer.Models.Enums;
using SFServer.Models.Responses.TransactionResponses;
using SFServer.Services.iAP;
using SFServer.Services.iAP.Apple;
using SFServer.Services.iAP.Google;
using SFServer.Models.Requests.TransactionRequests;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class TransactionController : Controller
    {
        private PostgreSqlContext _db;
        private TransactionService _transactionService;
        private PriceConfig _priceConfig;
        private AppleAppStoreService _appleAppStoreService;
        private GooglePlayStoreService _googlePlayStoreService;
        private ILogger<TransactionController> _logger;

        public TransactionController(PostgreSqlContext db, TransactionService transactionService, IOptions<PriceConfig> priceConfig, AppleAppStoreService appleAppStoreService, GooglePlayStoreService googlePlayStoreService, ILogger<TransactionController> logger)
        {
            _db = db;
            _transactionService = transactionService;
            _priceConfig = priceConfig.Value;
            _appleAppStoreService = appleAppStoreService;
            _googlePlayStoreService = googlePlayStoreService;
            _logger = logger;
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("unrecived-rewards")]
        public async Task<IActionResult> GetUnrecivedRewards()
        {
            User user = (User)Request.HttpContext.Items["user"];
            return Ok(await _transactionService.GetUnrecivedRewards(user.UserId, true));
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("prices")]
        public IActionResult GetPrices()
        {
            return Ok(_priceConfig.Prices);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("unlock-character")]
        public async Task<IActionResult> UnlockCharacter([FromBody] UnlockCharacterRequest request)
        {
            User user = (User)Request.HttpContext.Items["user"];

            int price = _priceConfig.Prices[request.Animal.ToString()];

            if (price < 0)
                return Ok(new UnlockCharacterResponse(false, user.Coins, PurchaseError.NotAvailable));

            if (price > user.Coins)
                return Ok(new UnlockCharacterResponse(false, user.Coins, PurchaseError.CantAfford));

            await _db.Entry(user).Reference(p => p.SaveData).LoadAsync();

            if (user.SaveData.IsUnlocked(request.Animal))
                return Ok(new UnlockCharacterResponse(false, user.Coins, PurchaseError.AlreadyPurchased));

            await _transactionService.SpendCoins(user, price, $"Adopted: {request.Animal.ToString()}");

            user.SaveData.UnlockAnimal(request.Animal);
            _db.UpdateSaveData(user, user.SaveData);
            await _db.SaveChangesAsync();

            return Ok(new UnlockCharacterResponse(true, user.Coins));
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("unlock-character-with-apple-iap")]
        public async Task<IActionResult> UnlockCharacterWithAppleIAP([FromBody] UnlockCharacterWithAppleIAPRequest unlockCharacterWithAppleIAPRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            AppleReceiptResponse response = await _appleAppStoreService.VerifyReceipt(unlockCharacterWithAppleIAPRequest.base64Receipt);

            if (response == null)
            {
                _logger.LogError($"UnlockCharacterWithAppleIAP(): {500}: Could not understand Apple's response.");
                return StatusCode(500, "Could not understand Apple's response.");
            }

            if (response.status != 0)
            {
                switch(response.status)
                {
                    case 21002:
                        _logger.LogError($"UnlockCharacterWithAppleIAP(): {400}: The data in the receipt - data property was malformed or the service experienced a temporary issue.Try again.");
                        return StatusCode(400, "The data in the receipt-data property was malformed or the service experienced a temporary issue. Try again.");
                    case 21003:
                        _logger.LogError($"UnlockCharacterWithAppleIAP(): {401}: The receipt could not be authenticated.");
                        return StatusCode(401, "The receipt could not be authenticated.");
                    case 21005:
                        _logger.LogError($"UnlockCharacterWithAppleIAP(): {401}: The receipt server was temporarily unable to provide the receipt. Try again.");
                        return StatusCode(401, "The receipt server was temporarily unable to provide the receipt. Try again.");
                    case 21009:
                        _logger.LogError($"UnlockCharacterWithAppleIAP(): {500}: Internal data access error.Try again later.");
                        return StatusCode(500, "Internal data access error. Try again later.");
                }

                _logger.LogError($"UnlockCharacterWithAppleIAP(): {500}: {AppStoreResponseStatus.GetExplaination(response.status)}");
                return StatusCode(500, AppStoreResponseStatus.GetExplaination(response.status));
            }

            Animal? animal = IAPIds.GetCharacterByIAPId(response.receipt.in_app[0].product_id);

            if(!animal.HasValue)
            {
                _logger.LogError($"UnlockCharacterWithAppleIAP(): {500}: No iap with Id found.");
                return StatusCode(500, "No iap with Id found.");
            }

            await _db.Entry(user).Reference(p => p.SaveData).LoadAsync();

            if (user.SaveData.IsUnlocked(animal.Value))
                return Ok(new UnlockCharacterWithIAPResponse(false, animal.Value, PurchaseError.AlreadyPurchased));

            if (response.environment == "Sandbox")
            {
                return Ok(new UnlockCharacterWithIAPResponse(true, animal.Value));
            }
            else
            {
                user.SaveData.UnlockAnimal(animal.Value);
                _db.UpdateSaveData(user, user.SaveData);
                await _db.SaveChangesAsync();
            }
            
            return Ok(new UnlockCharacterWithIAPResponse(true, animal.Value));
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("unlock-character-with-google-iap")]
        public async Task<IActionResult> UnlockCharacterWithGoogleIAP([FromBody] UnlockCharacterWithGoogleIAPRequest unlockCharacterWithGoogleIAPRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            GoogleReceiptResponse response = await _googlePlayStoreService.VerifyReceipt(unlockCharacterWithGoogleIAPRequest.productId, unlockCharacterWithGoogleIAPRequest.purchaseToken);

            if (response == null)
            {
                _logger.LogError($"UnlockCharacterWithGoogleIAP(): {500}: Could not understand Google's response.");
                return StatusCode(500, "Could not understand Google's response.");
            }

            if( response.error != null )
            {
                _logger.LogError($"UnlockCharacterWithGoogleIAP(): {response.error.code}: {response.error.message}");
                return StatusCode(response.error.code, response.error.message);
            }

            Animal? animal = IAPIds.GetCharacterByIAPId(unlockCharacterWithGoogleIAPRequest.productId);

            if (!animal.HasValue)
            {
                _logger.LogError($"UnlockCharacterWithGoogleIAP(): {400}: No iap with Id found.");
                return StatusCode(400, "No iap with Id found.");
            }

            await _db.Entry(user).Reference(p => p.SaveData).LoadAsync();

            if (user.SaveData.IsUnlocked(animal.Value))
                return Ok(new UnlockCharacterWithIAPResponse(false, animal.Value, PurchaseError.AlreadyPurchased));

            if (response.purchaseType == PurchaseType.Test)
            {
                return Ok(new UnlockCharacterWithIAPResponse(true, animal.Value));
            }
            else
            {
                user.SaveData.UnlockAnimal(animal.Value);
                _db.UpdateSaveData(user, user.SaveData);
                await _db.SaveChangesAsync();
            }

            return Ok(new UnlockCharacterWithIAPResponse(true, animal.Value));
        }
    }
}

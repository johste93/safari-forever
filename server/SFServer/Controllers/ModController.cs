using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFServer.Contexts;
using SFServer.Filters;
using SFServer.Models.DB;
using SFServer.Models.Requests.Mod;
using SFServer.Models.Responses.Mod;
using SFServer.Services;
using SFServer.Models.Transaction;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Moderator")]
    public class ModController : Controller
    {
        private readonly PostgreSqlContext _db;

        private TransactionService _transactionService;

        private DailyChallengeService _dailyChallengeService;

        public ModController(PostgreSqlContext db, TransactionService transactionService, DailyChallengeService dailyChallengeService)
        {
            _db = db;

            _transactionService = transactionService;

            _dailyChallengeService = dailyChallengeService;
        }

        [ModelValidation]
        [HttpPost("blacklist-level")]
        public async Task<ActionResult> BlacklistLevel([FromBody] BlacklistRequest request)
        {
            Level level = await _db.Levels.FindAsync(request.LevelId);

            if (level == null)
                return BadRequest("Level not found");

            level.Blacklisted = request.Blacklisted;

            _db.UpdateLevel(level);

            await _db.SaveChangesAsync();

            return Ok(new BlacklistResponse() { 
                LevelId = request.LevelId, 
                Blacklisted = level.Blacklisted 
            });
        }

        [ModelValidation]
        [HttpPost("ban-player")]
        public async Task<ActionResult> BanPlayer([FromBody] BanRequest request)
        {
            User user = await _db.Users.FindAsync(request.UserId);
            if (user == null)
                return StatusCode(404, $"User not found {request.UserId}");

            user.Banned = request.Banned;

            _db.UpdateUser(user);

            await _db.SaveChangesAsync();

            return Ok(new BanResponse() { 
                UserId = user.UserId, 
                NicknamePlusIdentifier = $"{user.Nickname}#{user.Identifier.ToString("0000")}", 
                Banned = user.Banned 
            });
        }

        [ModelValidation]
        [HttpPost("refund")]
        public async Task<ActionResult> Refund([FromBody] RefundRequest request)
        {
            Transaction transaction = await _db.Transactions.FindAsync(request.TransactionId);
            if (transaction == null)
                return StatusCode(404, $"transaction not found {request.TransactionId}");

            if (transaction.Refunded)
                return BadRequest("Transaction already refunded!");

            User user = await _db.Users.FindAsync(transaction.UserId);
            if (user == null)
                return StatusCode(404, $"User not found {transaction.UserId}");

            Transaction refundTransaction;

            if (transaction.ChangeInBalance > 0)
                refundTransaction = await _transactionService.SpendCoins(user, transaction.ChangeInBalance, $"Refund: {transaction.Description}", TransactionType.Refund);
            else
                refundTransaction = await _transactionService.AddCoins(user, transaction.ChangeInBalance, $"Refund: {transaction.Description}", TransactionType.Refund, transaction.LevelName);
            

            transaction.Refunded = true;

            _db.UpdateTransaction(transaction);

            await _db.SaveChangesAsync();

            return Ok(new RefundResponse() { 
                TransactionId = transaction.TransactionId, 
                UserId = user.UserId, 
                Refunded = true,
                BalanceBefore = refundTransaction.BalanceBefore,
                BalanceAfter = refundTransaction.BalanceAfter,
                ChangeInBalance = refundTransaction.ChangeInBalance,
            });
        }

        [ModelValidation]
        [HttpPost("gift")]
        public async Task<ActionResult> Gift([FromBody] GiftRequest request)
        {
            User user = await _db.Users.FindAsync(request.UserId);
            if (user == null)
                return StatusCode(404, $"User not found {request.UserId}");

            if (request.Amount <= 0)
                return BadRequest("Cant gift 0 or less coins.");

            Transaction giftTransaction = await _transactionService.AddCoins(user, Math.Abs(request.Amount), $"Gift: {request.Description}", TransactionType.Gift, string.Empty);

            return Ok(new GiftResponse() {
                UserId = user.UserId, 
                TransactionId = giftTransaction.TransactionId, 
                Amount = request.Amount,
                BalanceBefore = giftTransaction.BalanceBefore,
                BalanceAfter = giftTransaction.BalanceAfter,
                ChangeInBalance = giftTransaction.ChangeInBalance,
            });
        }

        [HttpGet("new-daily")]
        public async Task<ActionResult> NewDailyChallenge()
        {
            await _dailyChallengeService.GenerateNewDailyChallenge();

            return Ok();
        }
    }
}
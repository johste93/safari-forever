using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFServer.Contexts;
using SFServer.Models.Configs;
using SFServer.Models.DB;
using SFServer.Models.Transaction;

namespace SFServer.Services
{
    public class TransactionService
    {
        private PostgreSqlContext _db;
        private readonly IOptions<RewardConfig> _rewardConfig;

        public TransactionService(PostgreSqlContext db, IOptions<RewardConfig> rewardConfig)
        {
            _db = db;
            _rewardConfig = rewardConfig;
        }

        public async Task<Transaction> AddCoins(User user, int amount, string description, TransactionType transactionType, string levelName)
        {
            //Force positive number.
            amount = Math.Abs(amount);

            Transaction transaction = new Transaction()
            {
                UserId = user.UserId,
                ChangeInBalance = amount,
                BalanceBefore = user.Coins,
                BalanceAfter = user.Coins + amount,
                Description = description,
                RecivedByPlayer = false,
                TransactionType = transactionType,
                LevelName = levelName
            };

            _db.Add(transaction);

            user.Coins += amount;
            user.LifetimeCoins += amount;

            _db.UpdateUser(user);

            await _db.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> SpendCoins(User user, int amount, string description, TransactionType transactionType = TransactionType.Purchase)
        {
            //Force negative number.
            amount = -Math.Abs(amount);

            Transaction transaction = new Transaction()
            {
                UserId = user.UserId,
                ChangeInBalance = amount,
                BalanceBefore = user.Coins,
                BalanceAfter = user.Coins + amount,
                Description = description,
                TransactionType = transactionType
            };

            _db.Add(transaction);

            user.Coins += amount;
            _db.UpdateUser(user);

            await _db.SaveChangesAsync();

            return transaction;
        }

        public async Task<List<UnrecivedReward>> GetUnrecivedRewards(string userId, bool markAsRecived)
        {
            List<Transaction> transactions = await _db.Transactions.Where(x => x.UserId == userId && x.RecivedByPlayer == false && x.BalanceBefore < x.BalanceAfter).ToListAsync();
            
            if(markAsRecived)
            {
                foreach (Transaction transaction in transactions)
                {
                    transaction.RecivedByPlayer = true;
                    _db.Update(transaction);
                }

                await _db.SaveChangesAsync();
            }

            List<UnrecivedReward> rewards = transactions.Select(x => new UnrecivedReward
            {
                TransactionId = x.TransactionId,
                ChangeInBalance = x.ChangeInBalance,
                BalanceBefore = x.BalanceBefore,
                BalanceAfter = x.BalanceAfter,
                TransactionType = x.TransactionType,
                LevelName = x.LevelName,
                RecivedOn = x.CreatedOn,
                Description = x.Description,
            }).ToList();

            return rewards;
        }
    }
}

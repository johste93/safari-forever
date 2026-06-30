using System;
using Microsoft.Extensions.Options;
using SFServer.Configs;
using SFServer.Models.DB;
using SFServer.Contexts;
using SFServer.Models.Enums;
using SFServer.Models.Transaction;
using System.Threading.Tasks;

namespace SFServer.Services
{
    public class SlotMachineService
    {
        private PostgreSqlContext _db;
        private readonly SlotMachineConfig _slotMachineConfig;

        private TransactionService _transactionService;

        public SlotMachineService(PostgreSqlContext db, TransactionService transactionService, IOptions<SlotMachineConfig> slotMachineConfig)
        {
            _db = db;
            _slotMachineConfig = slotMachineConfig.Value;
            _transactionService = transactionService;
        }

        public async Task<SlotMachineResult> Spinn(User user, int coinsSpent)
        {
            SlotMachineResult result = new SlotMachineResult();

            await _db.Entry(user).Reference(x => x.SaveData).LoadAsync();

            if (user.SaveData == null)
            {
                user.SaveData = new UserSaveData(user.UserId);
                _db.UpdateUser(user);
                await _db.SaveChangesAsync();
            }

            int totalNumberOfHats = System.Enum.GetNames(typeof(Hat)).Length - 1;

            if (totalNumberOfHats - user.SaveData.NumberOfHatsUnlocked() <= 0)
            {
                result.Error = SlotMachineError.NoMoreRewards;
                return result;
            }

            if( coinsSpent < _slotMachineConfig.MinimumBet )
            {
                result.Error = SlotMachineError.LessThanMinimumBet;
                return result;
            }

            if( coinsSpent > user.Coins)
            {
                result.Error = SlotMachineError.CantAfford;
                return result;
            }

            if (coinsSpent > GetMaxBet())
            {
                result.Error = SlotMachineError.MoreThanMaximumBet;
                return result;
            }

            user.CoinsSpentOnLottery += coinsSpent;
            await _transactionService.SpendCoins(user, coinsSpent, "Used the slot machine", TransactionType.SlotMachine);

            int baseCost = GetbaseCost(user.SaveData.NumberOfHatsUnlocked()); //Get the base cost of this bet.

            double chanceBasedOnNumberOfHatsOwned = (1d-((double)user.SaveData.NumberOfHatsUnlocked() / (double)totalNumberOfHats)) * (0.5 - _slotMachineConfig.LossChance);
            double chanceBasedOnMoneySpent = Math.Clamp((double)(coinsSpent-_slotMachineConfig.MinimumBet) / (double)baseCost, 0f, 1f) * 0.5;

            double winChance = chanceBasedOnNumberOfHatsOwned + chanceBasedOnMoneySpent;

            Random r = new Random();
            double roll = r.NextDouble();

            result.DidWin = roll <= winChance;
            if (result.DidWin)
            {
                //Player won! Give them a random hat they don't have!
                result.Reward = GetRandomLockedHat(user);
                user.SaveData.UnlockHat(result.Reward);
                _db.UpdateSaveData(user, user.SaveData);
            }

            await _db.SaveChangesAsync();

            return result;
        }

        public int GetMinBet()
        {
            return _slotMachineConfig.MinimumBet;
        }

        public int GetMaxBet()
        {
            int totalNumberOfHats = System.Enum.GetNames(typeof(Hat)).Length - 1;
            return GetbaseCost(totalNumberOfHats);
        }

        private int GetbaseCost(int ownedHats)
        {
            return _slotMachineConfig.BaseCost + (ownedHats * _slotMachineConfig.CostIncrement);
        }

        private Hat GetRandomLockedHat(User user)
        {
            Random random = new Random();
            Hat reward = (Hat)random.Next(1, System.Enum.GetNames(typeof(Hat)).Length);
            while(user.SaveData.IsUnlocked(reward))
            {
                reward = (Hat)random.Next(1, System.Enum.GetNames(typeof(Hat)).Length);
            }

            return reward;
        }
    }
}

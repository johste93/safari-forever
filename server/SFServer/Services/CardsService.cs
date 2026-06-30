using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFServer.Configs;
using SFServer.Models.DB;
using SFServer.Models.Enums;
using SFServer.Models.Responses.CardsResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Contexts;
using SFServer.Models.Transaction;
using Microsoft.Extensions.Logging;

namespace SFServer.Services
{
    public class CardsService
    {
        private PostgreSqlContext _db;
        private TransactionService _transactionService;
        private readonly CardsConfig _cardsConfig;
        private ILogger<CardsService> _logger;

        public CardsService(PostgreSqlContext db, TransactionService transactionService, IOptions<CardsConfig> cardsConfig, ILogger<CardsService> logger)
        {
            _db = db;
            _transactionService = transactionService;
            _cardsConfig = cardsConfig.Value;
            _logger = logger;
        }

        public async Task<CardsSelectionResponse> GetCardsSelection(User user)
        {
            //If all cards are turned, refill cards.
            if (GetBools(user.CardTable).Where(x => x == true).Count() == 0)
                await DealCards(user);

            CardsSelectionResponse result = new CardsSelectionResponse()
            {
                CardPrice = _cardsConfig.Cost,
                Cards = GetBools(user.CardTable)
            };

            await _db.Entry(user).Reference(x => x.SaveData).LoadAsync();
            int totalNumberOfHats = System.Enum.GetNames(typeof(Hat)).Length - 1;
            if (totalNumberOfHats - user.SaveData.NumberOfHatsUnlocked() <= 0)
            {
                result.Error = CardError.NoMoreRewards;
            }

            return result;
        }

        public async Task<PickCardResponse> PickCard(User user, int cardIndex)
        {
            PickCardResponse result = new PickCardResponse();

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
                result.Error = CardError.NoMoreRewards;
                return result;
            }
            
            if (_cardsConfig.Cost > user.Coins)
            {
                result.Error = CardError.CantAfford;
                return result;
            }

            bool[] cardTable = GetBools(user.CardTable);

            if(cardIndex > cardTable.Length)
            {
                result.Error = CardError.CardIndexOutOfBounds;
                return result;
            }

            if(!cardTable[cardIndex])
            {
                result.Error = CardError.CardAlreadyTurned;
                return result;
            }

            int remainingCards = cardTable.Where(x => x == true).Count();
            if (remainingCards == 0)
            {
                result.Error = CardError.NoMoreCardsToTurn;
                return result;
            }

            cardTable[cardIndex] = false;
            user.CardTable = GetBitmask(cardTable);

            user.CoinsSpentOnCards += _cardsConfig.Cost;
            await _transactionService.SpendCoins(user, _cardsConfig.Cost, "Used the slot machine", TransactionType.Cards);

            double winChance = 1d/(double)remainingCards;
            Random r = new Random();
            double roll = r.NextDouble();

            result.DidWin = roll <= winChance;
            if (result.DidWin)
            {
                //Player won! Give them a random hat they don't have!
                result.Reward = GetRandomLockedHat(user);
                user.SaveData.UnlockHat(result.Reward);
                await DealCards(user);
            }

            _db.UpdateSaveData(user, user.SaveData);
            await _db.SaveChangesAsync();

            return result;
        }

        private async Task DealCards(User user)
        {
            user.CardTable = GetBitmask(new bool[] { true, true, true, true, true, true, true, true, true});
            _db.UpdateUser(user);
            await _db.SaveChangesAsync();
        }

        private Hat GetRandomLockedHat(User user)
        {
            Random random = new Random();
            Hat reward = (Hat)random.Next(1, System.Enum.GetNames(typeof(Hat)).Length);
            while (user.SaveData.IsUnlocked(reward))
            {
                reward = (Hat)random.Next(1, System.Enum.GetNames(typeof(Hat)).Length);
            }

            return reward;
        }

        private int GetBitmask(params bool[] bits)
        {
            return bits.Select((b, i) => b ? 1 << i : 0).Aggregate((a, b) => a | b);
        }

        private bool[] GetBools(int mask)
        {
            return Enumerable.Range(0, 9).Select(b => (mask & (1 << b)) != 0).ToArray();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFServer.Configs;
using SFServer.Contexts;
using SFServer.Models.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services
{
    public class BoostService
    {
        private PostgreSqlContext _db;
        private TransactionService _transactionService;
        private readonly GameConfig _gameConfig;
        private readonly VersionConfig _versionConfig;

        private static List<Tuple<int, long>> cachedList;

        public BoostService(PostgreSqlContext db, TransactionService transactionService, IOptions<GameConfig> gameConfig, IOptions<VersionConfig> versionConfig)
        {
            _db = db;
            _transactionService = transactionService;
            _gameConfig = gameConfig.Value;
            _versionConfig = versionConfig.Value;
        }

        public async Task<int> FindRank(int amountInvested, long createdOn)
        {
            if (cachedList == null)
            {
                cachedList = await _db.Levels
                   .Where(x => x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)) && x.Plays < _gameConfig.MaxPlaysInNewCategory)
                   .OrderByDescending(x => x.CoinsInvested)
                   .ThenBy(x => x.CreatedOn)
                   .Select(x => new Tuple<int, long>(x.CoinsInvested, x.CreatedOn.Ticks))
                   .ToListAsync();
            }

            //Copy list incase its cleared.
            List<Tuple<int, long>> list = new List<Tuple<int, long>>(cachedList);

            for (int i = 0; i < list.Count; i++)
            {
                //Starting at the beginning:
                //Skip every level thats boosted more than the one we are looking for.
                if (amountInvested < list[i].Item1)
                    continue;

                //If the amount invested is the same as the one we are looking for.
                if (amountInvested == list[i].Item1)
                {
                    //If this level or identical to this level.
                    if (list[i].Item2 == createdOn)
                    {
                        return i + 1;
                    }

                    //Skip every level older than this one.
                    if (list[i].Item2 < createdOn)
                        continue;
                }

                return i + 1;
            }

            return list.Count + 1;
        }

        public void ClearCache()
        {
            cachedList = null;
        }
    }
}

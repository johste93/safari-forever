using Microsoft.Extensions.Options;
using SFServer.Configs;
using SFServer.Models.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services
{
    public class RewardService
    {
        private readonly GameConfig _gameConfig;
        private readonly RewardConfig _rewardConfig;

        public RewardService(IOptions<GameConfig> gameConfig, IOptions<RewardConfig> rewardConfig)
        {
            _gameConfig = gameConfig.Value;
            _rewardConfig = rewardConfig.Value;
        }

        public int GetRewardMultiplier(int numberOfPlays)
        {
            return numberOfPlays < _gameConfig.MaxPlaysInNewCategory ? _rewardConfig.CompletedNewUserLevelMultipler : 1;
        }
    }
}

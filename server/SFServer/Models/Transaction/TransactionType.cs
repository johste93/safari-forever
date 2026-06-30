using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Transaction
{
    public enum TransactionType
    {
        Purchase,
        RecivedLike,
        BeatLevel,
        BeatNewLevel,
        BeatPromotedLevel,
        DailyChallengeParticipation,
        DailyChallengeThirdPlace,
        DailyChallengeSecondPlace,
        DailyChallengeFirstPlace,
        Gift,
        Refund,
        SlotMachine,
        Cards
    }
}

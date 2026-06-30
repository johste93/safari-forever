using SFServer.Models.Enums;
using SFServer.Utility;
using System;

namespace SFServer.Models.Responses.LevelResponses
{
    public class LevelStatsResponse
    {
        public float ClearRate { get; set; }
        public int Plays { get; set; }
        public int Likes { get; set; }
        public int Wins { get; set; }
        public int AvgJumps { get; set; }
        public int AvgDeaths { get; set; }
        public int TotalJumps { get; set; }
        public int TotalDeaths { get; set; }
        public string Record { get; set; }
        public string RecordHolderId { get; set; }
        public string RecordHolderNicknameAndIdentifier { get; set; }
        public Difficulty Difficulty { get; set; }

        public LevelStatsResponse(int Plays, int Wins, int AvgDeaths, int AvgJumps, int TotalDeaths, int TotalJumps, int Likes, int Seconds, int Milliseconds, string RecordHolderId, string RecordHolderNicknameAndIdentifier, Difficulty difficulty)
        {
            Plays = Math.Max(1, Plays);

            if (Wins > Plays)
                Wins = Plays;

            ClearRate = (float)Wins / (float)Plays;

            this.Plays = Plays;
            this.Likes = Likes;
            this.Wins = Wins;
            this.AvgDeaths = AvgDeaths;
            this.AvgJumps = AvgJumps;
            this.TotalDeaths = TotalDeaths;
            this.TotalJumps = TotalJumps;
            this.Record = new Highscore(Seconds, Milliseconds).ToString();
            this.RecordHolderId = RecordHolderId;
            this.RecordHolderNicknameAndIdentifier = RecordHolderNicknameAndIdentifier;
            this.Difficulty = difficulty;

            if(Seconds < 0 && Milliseconds < 0)
            {
                this.Record = "--:--";
                this.RecordHolderId = "";
                this.RecordHolderNicknameAndIdentifier = "None :)";
            }
        }

        public string GetClearPercentage()
        {
            ClearRate *= 100f;
            return ClearRate + "%";
        }
    }
}


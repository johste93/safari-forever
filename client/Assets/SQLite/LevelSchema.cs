using SQLite4Unity3d;

[System.Serializable]
public class Levels
{
    [PrimaryKey]
    public string LevelId{ get; set; }
    public string Name{ get; set; }
    public string SerializedLevel{ get; set; }
    public int Plays{ get; set; }
    public int Deaths{ get; set; }
    public int Wins{ get; set; }
    public int Likes{ get; set; }
    public int Dislikes{ get; set; }
    public double Record{ get; set; }
    public string GameVersion{ get; set; }
    public string CreatorUserId{ get; set; }
    public byte[] Thumbnail{ get; set; }
    public string MainColor{ get; set; }
    public string SubColor{ get; set; }
    public string WallColor{ get; set; }
    public string PatternColor{ get; set; }
    public string UpdatedOn{ get; set; }
    public string CreatedOn{ get; set; }
    public string UserId{ get; set; }
    public string RecordHolder{ get; set; }
    public bool Blacklisted{ get; set; }
    public string DailyChallengeOn{ get; set; }
    public int MajorGameVersion{ get; set; }
    public int MinorGameVersion{ get; set; }
    public int LifetimeLikes{ get; set; }
    public int CoinsInvested{ get; set; }
    public byte[] MiniThumbnail{ get; set; }
    public int Record_Milliseconds{ get; set; }
    public int Record_Seconds{ get; set; }
    public bool VerifiedUpload{ get; set; }
}
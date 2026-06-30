using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFServer.Models;
using SFServer.Models.Notifications;
using SFServer.Security;
using SFServer.Models.DailyChallenge;
using SFServer.Models.DB;
using SFServer.Models.Discord;

namespace SFServer.Contexts
{
    public class PostgreSqlContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Following> Followings { get; set; }
        public virtual DbSet<LevelUserStats> LevelUserStats { get; set; }
        public virtual DbSet<CampaignTime> CampaignTimes { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationLink> NotificationLinks { get; set; }
        public virtual DbSet<DailyChallenge> DailyChallenges { get; set; }
        public virtual DbSet<DailyChallengeParticipant> DailyChallengeParticipants { get; set; }
        public virtual DbSet<UserSaveData> UserSaveData { get; set; }
        public virtual DbSet<UserCampaignLevelData> UserCampaignLevelData { get; set; }
        public virtual DbSet<RecentPlay> RecentPlays { get; set; }
        public virtual DbSet<TrendingLevel> TrendingLevels { get; set; }
        public virtual DbSet<LevelOfTheWeek> LevelsOfTheWeek { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<PrivacyPolicyAgreement> PrivacyPolicyAgreement { get; set; }
        public virtual DbSet<TermsOfServiceAgreement> TermsOfServiceAgreement { get; set; }
        public virtual DbSet<EndlessChallenge> EndlessChallenge { get; set; }
        public virtual DbSet<PersonalDiscordInvite> DiscordInvites { get; set; }

        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : base(options)
        { }

        public void UpdateUser(User user)
        {
            user.LastActive = DateTimeOffset.Now;
            user.UpdatedOn = DateTimeOffset.Now;
            Users.Update(user);
        }

        public void UpdateFollowing(Following following)
        {
            following.UpdatedOn = DateTimeOffset.Now;
            Followings.Update(following);
        }

        public void UpdateDiscordInvite(PersonalDiscordInvite invite)
        {
            invite.UpdatedOn = DateTimeOffset.Now;
            DiscordInvites.Update(invite);
        }

        public void UpdateClient(Client client)
        {
            client.UpdatedOn = DateTimeOffset.Now;
            Clients.Update(client);
        }

        public void UpdateTransaction(Transaction transaction)
        {
            transaction.UpdatedOn = DateTimeOffset.Now;
            Transactions.Update(transaction);
        }

        public void UpdateSaveData(User user, UserSaveData saveData)
        {
            user.UpdatedOn = DateTimeOffset.Now;
            saveData.UpdatedOn = DateTimeOffset.Now;
            user.SaveData = saveData;
            Users.Update(user);
            UserSaveData.Update(saveData);
        }

        public void UpdateLevel(Level level)
        {
            level.UpdatedOn = DateTimeOffset.Now;
            Levels.Update(level);
        }

        public void UpdateLevelUserStats(LevelUserStats levelUserStats)
        {
            levelUserStats.UpdatedOn = DateTimeOffset.Now;
            LevelUserStats.Update(levelUserStats);
        }

        public void UpdateCampaignTime(CampaignTime campaignTime)
        {
            campaignTime.UpdatedOn = DateTimeOffset.Now;
            CampaignTimes.Update(campaignTime);
        }

        public void UpdateDailyChallengeParticipant(DailyChallengeParticipant dailyChallengeParticipant)
        {
            dailyChallengeParticipant.UpdatedOn = DateTimeOffset.Now;
            DailyChallengeParticipants.Update(dailyChallengeParticipant);
        }

        public void UpdateRecentPlay(RecentPlay recentPlay)
        {
            recentPlay.UpdatedOn = DateTimeOffset.Now;
            RecentPlays.Update(recentPlay);
        }

        public void UpdateEndlessChallenge(EndlessChallenge endlessChallenge)
        {
            endlessChallenge.UpdatedOn = DateTimeOffset.Now;
            EndlessChallenge.Update(endlessChallenge);
        }
    }
}

using SFServer.Models.DTO;
using SFServer.Models.Enums;
using SFServer.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DB
{
    public class UserSaveData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserSaveDataId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public bool PepeUnlocked { get; set; } = true;
        public bool PatchyUnlocked { get; set; }
        public bool JawsUnlocked { get; set; }
        public bool OlipherUnlocked { get; set; }
        public bool KokoUnlocked { get; set; }
        public bool LeonUnlocked { get; set; }
        public bool DebraUnlocked { get; set; }
        public bool NuggetUnlocked { get; set; }
        public bool PerryUnlocked { get; set; }
        public bool RexUnlocked { get; set; }
        public bool PingoUnlocked { get; set; }
        public bool HonkyUnlocked { get; set; }
        public bool SpeedyUnlocked { get; set; }
        public bool BrineUnlocked { get; set; }
        public bool AxolUnlocked { get; set; }

        public bool SantaHatUnlocked { get; set; }
        public bool ShadesHatUnlocked { get; set; }
        public bool ThinfoilHatUnlocked { get; set; }
        public bool WizzardHatUnlocked { get; set; }
        public bool WitchHatUnlocked { get; set; }
        public bool PirateHatUnlocked { get; set; }
        public bool ShowbizHatUnlocked { get; set; }
        public bool HaloHatUnlocked { get; set; }
        public bool TopHatHatUnlocked { get; set; }
        public bool VikingHatUnlocked { get; set; }
        public bool HornsHatUnlocked { get; set; }
        public bool SombreroHatUnlocked { get; set; }
        public bool ConicalHatUnlocked { get; set; }
        public bool BootHatUnlocked { get; set; }
        public bool ComradeHatUnlocked { get; set; }
        public bool CrownHatUnlocked { get; set; }
        public bool MustacheHatUnlocked { get; set; }
        public bool BeanieHatUnlocked { get; set; }
        public bool SouWesterHatUnlocked { get; set; }
        public bool PrivateHatUnlocked { get; set; }    

        public List<UserCampaignLevelData> CampaignLevelData { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        public UserSaveData(string userId)
        {
            this.UserId = userId;
            CampaignLevelData = new List<UserCampaignLevelData>();
        }

        public void Merge(SaveDataDTO dataToMerge)
        {
            foreach(UserCampaignLevelDataDTO dto in dataToMerge.UserCampaignLevelData)
            {
                UserCampaignLevelData campaingLevel = CampaignLevelData.FirstOrDefault(x => x.World == dto.World && x.Index == dto.Index);
                if(campaingLevel == null)
                {
                    CampaignLevelData.Add(
                        new UserCampaignLevelData()
                        {
                            UserSaveDataId = UserSaveDataId,
                            World = dto.World,
                            Index = dto.Index,
                            Beaten = dto.Beaten,
                            Seconds = dto.Seconds,
                            Milliseconds = dto.Milliseconds
                        }
                    );
                    continue;
                }

                campaingLevel.Beaten = campaingLevel.Beaten || dto.Beaten;

                Highscore oldHighScore = new Highscore(campaingLevel.Seconds, campaingLevel.Milliseconds);
                Highscore newHighScore = new Highscore(dto.Seconds, dto.Milliseconds);

                if(newHighScore.IsLowerThan(oldHighScore))
                {
                    campaingLevel.Seconds = dto.Seconds;
                    campaingLevel.Milliseconds = dto.Milliseconds;
                }

                campaingLevel.UpdatedOn = DateTimeOffset.Now;
            }
        }

        public bool IsUnlocked(Animal animal)
        {
            switch (animal)
            {
                case Animal.Pepe:
                    return PepeUnlocked;
                case Animal.Patchy:
                    return PatchyUnlocked;
                case Animal.Jaws:
                    return JawsUnlocked;
                case Animal.Olipher:
                    return OlipherUnlocked;
                case Animal.Koko:
                    return KokoUnlocked;
                case Animal.Leon:
                    return LeonUnlocked;
                case Animal.Debra:
                    return DebraUnlocked;
                case Animal.Nugget:
                    return NuggetUnlocked;
                case Animal.Perry:
                    return PerryUnlocked;
                case Animal.Rex:
                    return RexUnlocked;
                case Animal.Pingo:
                    return PingoUnlocked;
                case Animal.Honky:
                    return HonkyUnlocked;
                case Animal.Speedy:
                    return SpeedyUnlocked;
                case Animal.Brine:
                    return BrineUnlocked;
                case Animal.Axol:
                    return AxolUnlocked;
                default:
                    return false;
            }
        }

        public void UnlockAnimal(Animal animal)
        {
            switch (animal)
            {
                case Animal.Pepe:
                    PepeUnlocked = true;
                    break;
                case Animal.Patchy:
                    PatchyUnlocked = true;
                    break;
                case Animal.Jaws:
                    JawsUnlocked = true;
                    break;
                case Animal.Olipher:
                    OlipherUnlocked = true;
                    break;
                case Animal.Koko:
                    KokoUnlocked = true;
                    break;
                case Animal.Leon:
                    LeonUnlocked = true;
                    break;
                case Animal.Debra:
                    DebraUnlocked = true;
                    break;
                case Animal.Nugget:
                    NuggetUnlocked = true;
                    break;
                case Animal.Perry:
                    PerryUnlocked = true;
                    break;
                case Animal.Rex:
                    RexUnlocked = true;
                    break;
                case Animal.Pingo:
                    PingoUnlocked = true;
                    break;
                case Animal.Honky:
                    HonkyUnlocked = true;
                    break;
                case Animal.Speedy:
                    SpeedyUnlocked = true;
                    break;
                case Animal.Brine:
                    BrineUnlocked = true;
                    break;
                case Animal.Axol:
                    AxolUnlocked = true;
                    break;
            }
        }

        public bool IsUnlocked(Hat hat)
        {
            switch (hat)
            {
                case Hat.Santa:
                    return SantaHatUnlocked;
                case Hat.Shades:
                    return ShadesHatUnlocked;
                case Hat.Thinfoil:
                    return ThinfoilHatUnlocked;
                case Hat.Wizzard:
                    return WizzardHatUnlocked;
                case Hat.Witch:
                    return WitchHatUnlocked;
                case Hat.Pirate:
                    return PirateHatUnlocked;
                case Hat.Showbiz:
                    return ShowbizHatUnlocked;
                case Hat.Halo:
                    return HaloHatUnlocked;
                case Hat.TopHat:
                    return TopHatHatUnlocked;
                case Hat.Viking:
                    return VikingHatUnlocked;
                case Hat.Horns:
                    return HornsHatUnlocked;
                case Hat.Sombrero:
                    return SombreroHatUnlocked;
                case Hat.Conical:
                    return ConicalHatUnlocked;
                case Hat.Boot:
                    return BootHatUnlocked;
                case Hat.Comrade:
                    return ComradeHatUnlocked;
                case Hat.Crown:
                    return CrownHatUnlocked;
                case Hat.Mustache:
                    return MustacheHatUnlocked;
                case Hat.Beanie:
                    return BeanieHatUnlocked;
                case Hat.SouWester:
                    return SouWesterHatUnlocked;
                case Hat.Private:
                    return PrivateHatUnlocked;
                default:
                    return false;
            }
        }

        public void UnlockHat(Hat hat)
        {
            switch (hat)
            {
                case Hat.Santa:
                    SantaHatUnlocked = true;
                    break;
                case Hat.Shades:
                    ShadesHatUnlocked = true;
                    break;
                case Hat.Thinfoil:
                    ThinfoilHatUnlocked = true;
                    break;
                case Hat.Wizzard:
                    WizzardHatUnlocked = true;
                    break;
                case Hat.Witch:
                    WitchHatUnlocked = true;
                    break;
                case Hat.Pirate:
                    PirateHatUnlocked = true;
                    break;
                case Hat.Showbiz:
                    ShowbizHatUnlocked = true;
                    break;
                case Hat.Halo:
                    HaloHatUnlocked = true;
                    break;
                case Hat.TopHat:
                    TopHatHatUnlocked = true;
                    break;
                case Hat.Viking:
                    VikingHatUnlocked = true;
                    break;
                case Hat.Horns:
                    HornsHatUnlocked = true;
                    break;
                case Hat.Sombrero:
                    SombreroHatUnlocked = true;
                    break;
                case Hat.Conical:
                    ConicalHatUnlocked = true;
                    break;
                case Hat.Boot:
                    BootHatUnlocked = true;
                    break;
                case Hat.Comrade:
                    ComradeHatUnlocked = true;
                    break;
                case Hat.Crown:
                    CrownHatUnlocked = true;
                    break;
                case Hat.Mustache:
                    MustacheHatUnlocked = true;
                    break;
                case Hat.Beanie:
                    BeanieHatUnlocked = true;
                    break;
                case Hat.SouWester:
                    SouWesterHatUnlocked = true;
                    break;
                case Hat.Private:
                    PrivateHatUnlocked = true;
                    break;
            }
        }

        public int NumberOfHatsUnlocked()
        {
            int count = 0;

            if (SantaHatUnlocked) count++;
            if (ShadesHatUnlocked) count++;
            if (ThinfoilHatUnlocked) count++;
            if (WizzardHatUnlocked) count++;
            if (WitchHatUnlocked) count++;
            if (PirateHatUnlocked) count++;
            if (ShowbizHatUnlocked) count++;
            if (HaloHatUnlocked) count++;
            if (TopHatHatUnlocked) count++;
            if (VikingHatUnlocked) count++;
            if (HornsHatUnlocked) count++;
            if (SombreroHatUnlocked) count++;
            if (ConicalHatUnlocked) count++;
            if (BootHatUnlocked) count++;
            if (ComradeHatUnlocked) count++;
            if (CrownHatUnlocked) count++;
            if (MustacheHatUnlocked) count++;
            if (BeanieHatUnlocked) count++;
            if (SouWesterHatUnlocked) count++;
            if (PrivateHatUnlocked) count++;

            return count;
        }
    }
}
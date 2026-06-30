using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DB;

namespace SFServer.Models.DTO
{
    public class SaveDataDTO
    {
        public string Nickname { get; set; }
        public int Identifier { get; set; }
        public string Color { get; set; }
        public int Coins { get; set; }
        public List<string> LevelsPlayed { get; set; } = new List<string>();

        public bool PepeUnlocked { get; set; }
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

        public List<UserCampaignLevelDataDTO> UserCampaignLevelData { get; set; }


        public SaveDataDTO()
        {
        }

        public SaveDataDTO(User user)
        {
            Nickname = user.Nickname;
            Identifier = user.Identifier;
            Color = user.Color;
            Coins = user.Coins;

            PepeUnlocked = user.SaveData.PepeUnlocked;
            PatchyUnlocked = user.SaveData.PatchyUnlocked;
            JawsUnlocked = user.SaveData.JawsUnlocked;
            OlipherUnlocked = user.SaveData.OlipherUnlocked;
            KokoUnlocked = user.SaveData.KokoUnlocked;
            LeonUnlocked = user.SaveData.LeonUnlocked;
            DebraUnlocked = user.SaveData.DebraUnlocked;
            NuggetUnlocked = user.SaveData.NuggetUnlocked;
            PerryUnlocked = user.SaveData.PerryUnlocked;
            RexUnlocked = user.SaveData.RexUnlocked;
            PingoUnlocked = user.SaveData.PingoUnlocked;
            HonkyUnlocked = user.SaveData.HonkyUnlocked;
            SpeedyUnlocked = user.SaveData.SpeedyUnlocked;
            BrineUnlocked = user.SaveData.BrineUnlocked;
            AxolUnlocked = user.SaveData.AxolUnlocked;

            SantaHatUnlocked = user.SaveData.SantaHatUnlocked;
            ShadesHatUnlocked = user.SaveData.ShadesHatUnlocked;
            ThinfoilHatUnlocked = user.SaveData.ThinfoilHatUnlocked;
            WizzardHatUnlocked = user.SaveData.WizzardHatUnlocked;
            WitchHatUnlocked = user.SaveData.WitchHatUnlocked;
            PirateHatUnlocked = user.SaveData.PirateHatUnlocked;
            ShowbizHatUnlocked = user.SaveData.ShowbizHatUnlocked;
            HaloHatUnlocked = user.SaveData.HaloHatUnlocked;
            TopHatHatUnlocked = user.SaveData.TopHatHatUnlocked;
            VikingHatUnlocked = user.SaveData.VikingHatUnlocked;
            HornsHatUnlocked = user.SaveData.HornsHatUnlocked;
            SombreroHatUnlocked = user.SaveData.SombreroHatUnlocked;
            ConicalHatUnlocked = user.SaveData.ConicalHatUnlocked;
            BootHatUnlocked = user.SaveData.BootHatUnlocked;
            ComradeHatUnlocked = user.SaveData.ComradeHatUnlocked;
            CrownHatUnlocked = user.SaveData.CrownHatUnlocked;
            MustacheHatUnlocked = user.SaveData.MustacheHatUnlocked;
            BeanieHatUnlocked = user.SaveData.BeanieHatUnlocked;
            SouWesterHatUnlocked = user.SaveData.SouWesterHatUnlocked;
            PrivateHatUnlocked = user.SaveData.PrivateHatUnlocked;

            UserCampaignLevelData = new List<UserCampaignLevelDataDTO>();

            foreach(UserCampaignLevelData userCampaignLevelData in user.SaveData.CampaignLevelData)
            {
                UserCampaignLevelData.Add(new UserCampaignLevelDataDTO(userCampaignLevelData));
            }
        }
    }
}

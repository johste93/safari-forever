using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataDTO
{
	public string GameVersion {get; set;}
    public string Nickname { get; set; }
    public int Identifier { get; set; }
    public string Color { get; set; }
    public int Coins { get; set; }
    public List<string> LevelsPlayed {get; set; }

    public bool PepeUnlocked { get; set; }
    public bool PatchyUnlocked { get; set; }
    public bool JawsUnlocked { get; set; }
    public bool OlipherUnlocked {get; set;}
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
}

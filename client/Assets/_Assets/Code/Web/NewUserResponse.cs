using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewUserResponse
{
    public string userId;
    public string token;
    public string restoreToken;
    public SaveDataDTO saveData;

    public override string ToString()
    {
        return $"{userId}\n{saveData.Identifier}\n{saveData.Color}\n{token}\n{saveData.Nickname}\n{restoreToken}\n{saveData.Coins}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using FSG.iOSKeychain;

public class Profile
{
    public string userId;
    public string nickname;
    public string color;
    public int identifier;
    public string token;
    
    public int coins;

    public Profile(string userId, string token, string nickname, int identifier, string color, int coins)
    {
        this.userId = userId;
        this.token = token;
        this.nickname = nickname;
        this.identifier = identifier;
        this.color = color;
        this.coins = coins;
    }
}

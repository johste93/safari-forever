using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileRestoreResponse
{
    public string UserId { get; set; }
    public string Token { get; set; }
    public string Nickname { get; set; }
    public int Identifier { get; set; }
    public string Color { get; set; }
    public int Coins { get; set; }
}

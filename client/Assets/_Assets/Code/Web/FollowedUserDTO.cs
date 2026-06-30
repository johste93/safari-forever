using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FollowedUserDTO
{
    public string UserId { get; set; }
    public string Nickname { get; set; }
    public int Identifier { get; set; }
    public string Color { get; set; }
    public DateTimeOffset LastActive { get; set; }
}

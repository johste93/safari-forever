using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowedUsersResponse
{
    public int UsersPrPage { get; set; }
    public List<FollowedUserDTO> followedUsers { get; set; }
}

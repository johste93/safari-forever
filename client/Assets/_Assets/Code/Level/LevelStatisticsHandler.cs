using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatisticsHandler : MonoBehaviour
{
    private void On_PlayerDied(FSM_CharacterController2D.FSM_CharacterController controller)
    {
        if(GlobalSingleton.mode != GameMode.FreePlay)
            return;

        //If we are currently playing a level that has been published.
        Level level = GameMaster.instance.currentlyPlayingLevel;
        if(level == null || level.PublishedLevelMeta == null)
            return;

       LevelAPI.CountDeath(level.PublishedLevelMeta.LevelId, level.PublishedLevelMeta.CreatorUserId, null);
    }
    
    private void OnEnable()
    {
        GameMaster.On_PlayerDied += On_PlayerDied;
        //GameMaster.On_PlayerWon += On_PlayerWon;
    }

    private void Unsubscribe()
    {
        GameMaster.On_PlayerDied -= On_PlayerDied;
        //GameMaster.On_PlayerWon -= On_PlayerWon;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}

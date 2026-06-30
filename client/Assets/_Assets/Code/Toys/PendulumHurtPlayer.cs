using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumHurtPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.enabled == false)
            return;

        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;

        Hurt(other.gameObject.GetComponent<Character>());
    }

    private void Hurt(Character character)
    {
        if(!character.IsDead())
        {
            Audio.Play( SFX.instance.level.slice.slice, Channel.Game );
            character.Hurt();
        }
    }
}
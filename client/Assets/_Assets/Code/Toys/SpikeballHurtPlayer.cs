using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StandardCharacterController2D.Spikeball;

public class SpikeballHurtPlayer : MonoBehaviour
{
    public Spikeball spikeball;

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
            Audio.Play( SFX.instance.level.spikeball.splat, Channel.Game );
            character.Hurt();
        }
    }
}

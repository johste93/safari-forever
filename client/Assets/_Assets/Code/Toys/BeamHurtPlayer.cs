using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamHurtPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;

        if (other.enabled == false)
            return;

        Hurt(other.gameObject.GetComponent<Character>());
    }

    private void Hurt(Character character)
    {
        if(!character.IsDead())
        {
            Audio.Play( SFX.instance.level.fire.burn, Channel.Game );
            character.Hurt();
        }
    }
}

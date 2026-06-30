using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTrigger : MonoBehaviour
{
    public MetalBlock parentBlock;

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerStay2D(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.enabled == false)
            return;

        if (!GameMaster.instance.IsPlaying())
			return;

        if(!parentBlock.IsPowered())
            return;

        if (!parentBlock.PlayerHasStartedToRun())
            return;

        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;

        Hurt(other.gameObject.GetComponent<Character>());
    }

    private void Hurt(Character character)
    {
        if(!character.IsDead())
        {
            Audio.Play( SFX.instance.level.electric.zap, Channel.Game );
            character.Hurt();
        }
    }
}

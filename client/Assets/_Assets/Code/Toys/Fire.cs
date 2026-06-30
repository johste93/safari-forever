using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{   
    public SpriteRenderer spriteRenderer;

    public CircleCollider2D circleCollider2D;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;

        if (other.enabled == false)
            return;

        HurtPlayer(other.gameObject.GetComponent<Character>());
    }

    private void HurtPlayer(Character character)
    {
        if(!character.IsDead())
        {
            Audio.Play( SFX.instance.level.fire.burn, Channel.Game);
            character.Hurt();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class SlimeBubble : MonoBehaviour
{
    public Transform graphic;
    
    private float duration = 0.5f;
    private float currentTime;
    private Vector2 direction;
    private float scale = 0.75f;
    private float maxDistance = 1.4f;

    public void Bounce(Transform target)
    {
        this.direction = (transform.position - target.position);
        float distance = direction.magnitude;
        float amount = 1f - Mathf.Clamp01(distance / maxDistance);

        currentTime = duration;
        this.direction = direction.normalized * ((Vector2)graphic.localScale).magnitude * scale * amount;
    }

    private void Update()
    {
        if(currentTime > 0)
        {
            float t =  1f - (currentTime/duration);
            graphic.localPosition = Spring(direction, Vector3.zero, t);

            currentTime -= Time.deltaTime * SaveManager.currentSave.gameSpeed;
        }
        else
        {
            graphic.localPosition = Vector3.zero;
        }
    }

    private Vector3 Spring(Vector3 from, Vector3 to, float time)
    {
        return new Vector3(Spring(from.x, to.x, time), Spring(from.y, to.y, time), 0f);
    }

    private float Spring(float from, float to, float time)
    {
        time = Mathf.Clamp01(time);
        time = (Mathf.Sin(time * Mathf.PI * (.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time) * (1f + (1.2f * (1f - time)));
        return from + (to - from) * time;
    }

    private void Reset()
    {
        direction = Vector2.zero;
        currentTime = 0;
    }

    private void On_PlayerDied(FSM_CharacterController controller) => Reset();

    private void On_ExitPlayMode() => Reset();
    
    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_PlayerDied += On_PlayerDied;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_PlayerDied -= On_PlayerDied;
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

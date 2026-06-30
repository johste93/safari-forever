using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventCooler : MonoBehaviour
{
    [SerializeField]
    public UnityEvent OnClickEvent;

    public List<UnityEventCooler> shareCooldown;

    private float cooldown;

    public void OnClick()
    {
        if(cooldown > 0f)
            return;
        
        OnClickEvent.Invoke();

        PutOnCooldown();
        foreach (UnityEventCooler cooler in shareCooldown)
        {
            cooler.PutOnCooldown();
        }
    }

    public void PutOnCooldown()
    {
        cooldown = 1f;
    }

    private void Update()
    {
        if(cooldown > 0f)
            cooldown -= Time.deltaTime;
    }

    private void OnEnable()
    {
        cooldown = 0f;
    }

    private void OnDisable()
    {
        cooldown = 0f;
    }
}
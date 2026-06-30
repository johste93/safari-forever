using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class HatController : MonoBehaviour
{
    private float gravity = -50f;
    private float force = 10f;
    private SpriteMask spriteMask;

    public GameObject Santa,
        Shades,
        Thinfoil,
        Wizzard,
        Witch,
        Pirate,
        Showbiz,
        Halo,
        TopHat,
        Viking,
        Horns,
        Sombrero,
        Conical,
        Boot,
        Comrade,
        Crown,
        Mustache,
        Beanie,
        SouWester,
        Private;


    private Vector3 velocity;
    private bool detached;
    private Transform parent;

    private static Hat? previousHat;

    private void Start()
    {
        parent = transform.parent;
        
        Hat currentHat = (Hat) SaveManager.currentSave.currentHat;
        SetHat(currentHat);
    }

    public void SetHat(Hat hat)
    {
        if(SaveManager.currentSave.useRandomHat && GameMaster.instance != null)
        {
            hat = GetRandomHat();
        }

        previousHat = hat;

        Santa.SetActive( hat == Hat.Santa );
        Shades.SetActive( hat == Hat.Shades );
        Thinfoil.SetActive( hat == Hat.Thinfoil );
        Wizzard.SetActive( hat == Hat.Wizzard );
        Witch.SetActive( hat == Hat.Witch );
        Pirate.SetActive( hat == Hat.Pirate );
        Showbiz.SetActive( hat == Hat.Showbiz );
        Halo.SetActive( hat == Hat.Halo );
        TopHat.SetActive( hat == Hat.TopHat );
        Viking.SetActive( hat == Hat.Viking );
        Horns.SetActive( hat == Hat.Horns );
        Sombrero.SetActive( hat == Hat.Sombrero );
        Conical.SetActive( hat == Hat.Conical );
        Boot.SetActive( hat == Hat.Boot );
        Comrade.SetActive( hat == Hat.Comrade );
        Crown.SetActive( hat == Hat.Crown );
        Mustache.SetActive( hat == Hat.Mustache );
        Beanie.SetActive( hat == Hat.Beanie );
        SouWester.SetActive( hat == Hat.SouWester );
        Private.SetActive( hat == Hat.Private );
    }

    private Hat GetRandomHat()
    {
        List<Hat> unlockedHats = new List<Hat>();

        for(int i = 1; i < SaveManager.currentSave.unlockedHats.Length; i++)
        {
            if(SaveManager.currentSave.unlockedHats[i])
            {
                unlockedHats.Add((Hat) (i));
            }
        }

        if(unlockedHats.Count <= 1)
            return (Hat) SaveManager.currentSave.currentHat;

        if(previousHat.HasValue)
            unlockedHats.Remove(previousHat.Value);
        else
            unlockedHats.Remove((Hat) SaveManager.currentSave.currentHat);

        return unlockedHats[Random.Range(0, unlockedHats.Count)];
    }

    public void FallOff()
    {
        spriteMask = GetComponentInChildren<SpriteMask>();
        if(spriteMask != null)
            spriteMask.gameObject.SetActive(false);

        transform.SetParent(null);
        detached = true;
        velocity = new Vector3(Random.Range(-1f, 1f), 2f, 0) * force;

        
    }

    public void Reset()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if(!detached)
            return;

        transform.eulerAngles += new Vector3(0,0, 360f * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
        velocity += new Vector3(0, gravity * Time.deltaTime, 0);
    }

    private void On_PlayerDied(FSM_CharacterController controller) => FallOff();

    private void On_LevelReset(bool manualReset) => Reset();

    

    private void OnEnable()
    {
        GameMaster.On_PlayerDied += On_PlayerDied;
        GameMaster.On_LevelReset += On_LevelReset;
    }

    private void Unsubscribe()
    {
        GameMaster.On_PlayerDied -= On_PlayerDied;
        GameMaster.On_LevelReset -= On_LevelReset;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;
using DG.Tweening;

public class PlayerSpawnPoint : MonoBehaviour
{   
    public GameObject readySignPrefab;
    public GameObject oneWayPlatform;

    private FSM_CharacterController characterController;
    private ReadySign lastReadySign;

    private Direction4Gizmo directionGizmo;

    private LevelEntity entity;
    private Tween delayTween;

    private bool waitingLogicEvent;
    private Animal? previousAnimal;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        directionGizmo = (Direction4Gizmo) entity.gizmo;
    }
    
    private void OnEnable()
    {
        Spawn();
        Subscribe();
    }

    private void Update()
	{
        if(!GameMaster.instance.IsPlaying())
            return;

        DoLogicUpdate();
    }

    private void DoLogicUpdate()
    {
        if(waitingLogicEvent)
        {
            entity.outputNode.EmitPower(true);
            waitingLogicEvent = false;
        }
        else
        {
            entity.outputNode.EmitPower(false);
        } 
    }

    private Animal GetRandomCharacter()
    {
        List<Animal> unlockedAnimals = new List<Animal>();

        for(int i = 0; i < SaveManager.currentSave.unlockedCharacter.Length; i++)
        {
            if(SaveManager.currentSave.unlockedCharacter[i])
            {
                unlockedAnimals.Add((Animal) i);
            }
        }

        if(unlockedAnimals.Count <= 1)
            return (Animal) SaveManager.currentSave.currentCharacter;

        if(previousAnimal.HasValue)
            unlockedAnimals.Remove(previousAnimal.Value);
        else
            unlockedAnimals.Remove((Animal) SaveManager.currentSave.currentCharacter);

        return unlockedAnimals[Random.Range(0, unlockedAnimals.Count)];
    }

    public void Spawn()
    {
        Animal character = (Animal) SaveManager.currentSave.currentCharacter;

        if(SaveManager.currentSave.useRandomCharacter)
            character = GetRandomCharacter();

        previousAnimal = character;

        GameObject prefab = Globals.gameConstants.GetCharacterPrefab(character);
        GameObject spawnedPlayer = Instantiate(prefab, transform);
        spawnedPlayer.transform.localPosition = new Vector3(0f, -0.5f, 0f);

        characterController = spawnedPlayer.GetComponent<FSM_CharacterController>();
        characterController.boxCollider2D.enabled = false;

        int runningDirection = directionGizmo.direction == Direction4.Left ? -1 : 1;
        characterController.motion.runningDirection = runningDirection;

        characterController.Suspend(true);
        characterController.stateController.OnStateChanged += OnStateChanged;
    }

    private void OnStateChanged(State previousState, State newState)
    {
        //On the first sign of input we unsubscribe!
        if(lastReadySign != null)
            lastReadySign.ShrinkSign();

        characterController.stateController.OnStateChanged -= OnStateChanged;

        waitingLogicEvent = true;
        oneWayPlatform.SetActive(false);
    }

    private void On_EnterPlayMode()
    {
        //Attach sign
        GameObject spawnedSign = Instantiate(readySignPrefab, characterController.transform);
        lastReadySign = spawnedSign.GetComponent<ReadySign>();
        
        characterController.Suspend(false);
        characterController.character.ReadyPlayer();
        characterController.inputInfo = new InputInfo();

        ValidateSpawn(()=>{
            if (LevelBuilder.instance.GetCurrentRoomIndex() == 0)
                lastReadySign.EnlargeSign();
        });
    }

    private void ValidateSpawn(System.Action onComplete)
    {
        if (SpawnValid())
        {
            float delay = 0f;

            if (TransitionSingleton.instance.IsTransitioning() || PopupCanvas.instance.IsShowing())
            {
                delay = 0.2f;
            }

            delayTween = DOVirtual.DelayedCall(delay, ()=> onComplete?.Invoke() );
        }
        else
        {
            characterController.character.Hurt(true);
        }
    }

    public bool SpawnValid()
    {
        Vector2 startPos = (Vector2)FindObjectOfType<PlayerSpawnPoint>().transform.position;
        LayerMask mask = characterController.properties.whatIsPlatform.CombineLayerMask(characterController.properties.whatIsSlope);

        return !Physics2D.OverlapCircle(startPos, 0.1f, mask);
    }

    private void On_LevelReset(bool manual)
    {
        Reset();
    }

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
	{
		delayTween?.Kill();
        ValidateSpawn(null);
	}

    private void On_ExitPlayMode()
    {
        Reset();
    }

    private void Respawn()
    {
        Destroy(characterController.gameObject);
        Spawn();
    }

    private void Reset()
    {
        Respawn();

        characterController.character.ReadyPlayer();
        characterController.Suspend(false);
        characterController.inputInfo = new InputInfo();

        if (lastReadySign != null)
            lastReadySign.ShrinkSign();

        waitingLogicEvent = false;
        oneWayPlatform.SetActive(true);
    }

    private void Subscribe()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;

		GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_LevelReset -= On_LevelReset;

		GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		delayTween?.Kill();
	}
}

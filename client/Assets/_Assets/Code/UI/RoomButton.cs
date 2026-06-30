using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{   
    public RectTransform rectTransform;
    public TextMeshProUGUI textmesh;
    public Image icon;
    public Sprite[] symbols;

    private void Start()
    {
        int index = LevelBuilder.instance.GetCurrentRoomIndex();
        textmesh.text = (index+1).ToString();
        //icon.sprite = symbols[index];
    }

    public void OnClick()
    {
        TouchInput.CancelTouch();
        
        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(UnityEngine.Random.Range(0.9f, 1.1f));
        rectTransform.DOComplete();
        rectTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);

        LevelBuilder.instance.SaveRoom();

        int index = LevelBuilder.instance.GetCurrentRoomIndex();
        index++;
        if(index >= LevelBuilder.instance.GetNumberOfRooms())
        {
            if(index >= Globals.gameConstants.maxNumberOfRooms)
            {
                index = 0;
                LevelBuilder.instance.SetRoomIndex(index);
            }
            else
                LevelBuilder.instance.AddRoom();
        }
        else
        {
            LevelBuilder.instance.SetRoomIndex(index);
        }
    }

    private void UpdateRoomIndex()
    {
        int index = LevelBuilder.instance.GetCurrentRoomIndex();
        textmesh.text = (index+1).ToString();
        //icon.sprite = symbols[index];
    }

    private void OnEnable()
    {
        LevelBuilder.On_ChangedRoom += UpdateRoomIndex;
    }

    private void Unsubscribe()
    {
        LevelBuilder.On_ChangedRoom -= UpdateRoomIndex;
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
		rectTransform.DOKill();
	}

    /* 
    private void ChangeRoom()
    {
        bool roomEmpty = LevelBuilder.instance.IsRoomEmpty();
        int filledRooms = LevelBuilder.instance.GetFilledRooms();
        int availableRooms = 4 - filledRooms;

        Debug.Log(filledRooms);
        Debug.Log(availableRooms);

        int currentRoomIndex = LevelBuilder.instance.GetCurrentRoomIndex();

        if(roomEmpty && LevelBuilder.instance.GetNumberOfRooms() > 1)
        {
            //Destroy current room.
            LevelBuilder.instance.DeleteRoom();

            //Go To Next Filled Room.
            return;
        }

        currentRoomIndex++;


        if(currentRoomIndex >= filledRooms)
        {
            if(availableRooms > 0)
            {
                //Create new room
                LevelBuilder.instance.AddRoom();
                return;
            }
            else
            {
                //Loop back
                currentRoomIndex = 0;
            }
        }

        Debug.Log(currentRoomIndex);
        LevelBuilder.instance.SetRoomIndex(currentRoomIndex);
    }
    */
}

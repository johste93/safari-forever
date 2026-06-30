using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RoomNavigator : MonoBehaviour
{
    public GameObject prefab;

    public Sprite selected;
    public Sprite unselected;

    private List<RoomNavigatorElement> dots = new List<RoomNavigatorElement>();

    public void UpdateRoomCount(bool instant = false)
    {
        if(LevelBuilder.instance.GetNumberOfRooms() > dots.Count)
        {
            //Spawn roooms
            int currentIndex = LevelBuilder.instance.GetCurrentRoomIndex();
            int roomsToSpawn = LevelBuilder.instance.GetNumberOfRooms()  - dots.Count;
            for(int i = 0; i < roomsToSpawn; i++)
            {
                GameObject go = Instantiate(prefab, transform);
                RoomNavigatorElement element = go.GetComponent<RoomNavigatorElement>();
                dots.Add(element);

                element.rectTransform.sizeDelta = Vector2.zero;
            }
        }

        if(LevelBuilder.instance.GetNumberOfRooms() < dots.Count)
        {
            //Destroy rooms
            int roomsToDestroy = dots.Count - LevelBuilder.instance.GetNumberOfRooms();
            for(int i = roomsToDestroy-1; i >= 0; i--)
            {
                Destroy(dots[i].gameObject);
                dots.RemoveAt(i);
            }
        }

        UpdateRoomIndex(instant);
    }

    public void UpdateRoomIndex(bool instant = false)
    { 
        int currentIndex = LevelBuilder.instance.GetCurrentRoomIndex();

        for(int i = 0; i < dots.Count; i++)
        {
            dots[i].DOKill();
            if(i < LevelBuilder.instance.GetNumberOfRooms())
                dots[i].rectTransform.DOSizeDelta((i == currentIndex ? 7.5f : 2.5f) * Vector2.one, instant ? 0f : 0.15f);
            else
                dots[i].rectTransform.DOSizeDelta(Vector2.zero, instant ? 0f : 0.15f);
                
            dots[i].image.sprite = i == currentIndex ? selected : unselected;
        }
    }

	private void KillAllTweens()
	{
		for(int i = 0; i < dots.Count; i++)
        {
            dots[i].DOKill();
			dots[i].rectTransform.DOKill();
		}
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}
}

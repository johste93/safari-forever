using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInfo
{
    public int fingerIndex;

    public TouchPhase phase;

    public float duration;

    public Vector2 screenPosition;
    
    public Vector2 viewportStartPosition;
    public Vector2 viewportPosition;

    public GameObject pickedGameObject;
    public GameObject pickedUIElement;

    private GameObject firstPickedGameObject;

    private Vector3? touchToWorldPoint;
    private Collider2D[] allPickedColliders;

    public TouchInfo(int finderIndex)
    {
        this.fingerIndex = finderIndex;
    }

    public void ResetCache()
    {
        touchToWorldPoint = null;
        allPickedColliders = null;
    }

    public Vector3 GetTouchToWorldPoint(float distanceFromCamera, Camera camera)
    {
        if(!touchToWorldPoint.HasValue)
            touchToWorldPoint = camera.ViewportToWorldPoint( new Vector3( viewportPosition.x, viewportPosition.y, distanceFromCamera));

        return touchToWorldPoint.Value;
	}

    public GameObject GetFirstPickedGameObject(Camera camera)
    {
        Vector3 origin = GetTouchToWorldPoint(0, camera);

        RaycastHit2D hit = Physics2D.Raycast(origin, camera.transform.forward, Mathf.Infinity, TouchInput.WhatIsPickable);
        if(hit.collider != null && firstPickedGameObject == null)
            firstPickedGameObject = hit.collider.gameObject;

        return firstPickedGameObject;
    }

    public Collider2D[] GetAllPickedColliders(Camera camera)
    {
        Vector3 origin = GetTouchToWorldPoint(0, camera);
        
        if(allPickedColliders == null)
            allPickedColliders = Physics2D.OverlapPointAll(origin, TouchInput.WhatIsPickable);

        return allPickedColliders;
    }

    public GameObject GetFirstPickedUIElement()
    {
        if (IsScreenPositionOverUI( screenPosition )){
			return GetFirstUIElementFromCache();
		}
		else{
			return null;
		}
    }

    private EventSystem uiEventSystem;
    private List<RaycastResult> uiRaycastResultCache= new List<RaycastResult>();

    private bool IsScreenPositionOverUI( Vector2 position){

		uiEventSystem = EventSystem.current;
		if (uiEventSystem != null){

			PointerEventData uiPointerEventData = new PointerEventData( uiEventSystem);
			uiPointerEventData.position = position;

			uiEventSystem.RaycastAll( uiPointerEventData, uiRaycastResultCache);
			if (uiRaycastResultCache.Count>0){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			return false;
		}
	}

    private GameObject GetFirstUIElementFromCache(){

		if (uiRaycastResultCache.Count>0){
			return uiRaycastResultCache[0].gameObject;
		}
		else{
			return null;
		}
	}
}

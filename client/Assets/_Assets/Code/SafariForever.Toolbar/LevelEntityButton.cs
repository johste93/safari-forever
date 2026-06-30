using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

namespace SafariForever.Toolbar
{
    public class LevelEntityButton : MonoBehaviour
    {
        public GameObject prefab;
        private ToolbarButton toolbarButton;

        public RectTransform dragMe;

        public Image icon;
        public TextMeshProUGUI text;

        private LevelEntity lastSpawnedEntity;
        private bool fingerIndexAssigned = false;
        private Toolbar toolbar;

		private TouchInfo touchStartCache;

        private void Awake()
        {
            toolbar = GetComponentInParent<Toolbar>();
            toolbarButton = GetComponentInParent<ToolbarGrid>().toolbarButton;

            EventTrigger eventTrigger = GetComponent<EventTrigger>();
            EventTrigger.Entry OnDragBeginEntry = new EventTrigger.Entry();
            OnDragBeginEntry.eventID = EventTriggerType.BeginDrag;
            OnDragBeginEntry.callback.AddListener((eventData) => 
            {
                OnBeginDrag((PointerEventData)eventData);
            });

            eventTrigger.triggers.Add(OnDragBeginEntry);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //toolbar.Close();
            toolbarButton.SetShortcut(this);
            bool unique = prefab.GetComponent<LevelEntity>().unique;
            if(unique)
            {
                string id = prefab.GetComponent<LevelEntity>().id;

                LevelEntity uniqueSpawned = LevelBuilder.instance.GetLevelEntities().FirstOrDefault(x => x.id == id);
                if(uniqueSpawned != null)
                {
                    fingerIndexAssigned = false;
                    lastSpawnedEntity = uniqueSpawned;
                    return;
                }
            }
            
            GameObject lastSpawn = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            lastSpawnedEntity = lastSpawn.GetComponent<LevelEntity>();
            fingerIndexAssigned = false;

			Vector3 touchPos = eventData.pointerCurrentRaycast.worldPosition;
			touchPos.z = 0;
        	lastSpawnedEntity.SetPositon(touchPos);
        }

        public void OnClick()
        {
            //Select.
            toolbarButton.SetShortcut(this);

            dragMe.DOKill();
            dragMe.localScale = new Vector3(0,0,1);
            dragMe.gameObject.SetActive(true);
            dragMe.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        }

        private void On_TouchStart(TouchInfo touch)
        {
			touchStartCache = touch;

            if(dragMe.gameObject.activeInHierarchy)
                dragMe.DOScale(0f, 0.2f).SetEase(Ease.OutQuad).OnComplete(()=>{
                    dragMe.gameObject.SetActive(false);
                });
        }

        private void On_TouchMaintained(TouchInfo touch)
        {
            if(lastSpawnedEntity != null && !fingerIndexAssigned)
            {
                lastSpawnedEntity.GetDragHandle().AssignFinger(touch.fingerIndex);
                lastSpawnedEntity.OnTouchStart(touch);
                fingerIndexAssigned = true;
            }	
        }

        private void On_TouchEnd(TouchInfo touch)
        {
            if(lastSpawnedEntity == null)
                return;

            lastSpawnedEntity.GetDragHandle().On_TouchEnd(touch);
            
            lastSpawnedEntity = null;
        }

        private void OnEnable()
        {
            TouchInput.On_TouchStart += On_TouchStart;

            TouchInput.On_TouchMaintained += On_TouchMaintained;
            TouchInput.On_TouchEnd += On_TouchEnd;
        }

        private void Unsubscribe()
        {
            TouchInput.On_TouchStart -= On_TouchStart;

            TouchInput.On_TouchMaintained -= On_TouchMaintained;
            TouchInput.On_TouchEnd -= On_TouchEnd;
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
			dragMe.DOKill();
		}
    }
}
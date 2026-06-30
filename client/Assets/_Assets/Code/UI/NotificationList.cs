using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;

namespace SafariForever.Notifications
{
    public class NotificationList : MonoBehaviour
    {
        public GameObject messagePrefab;
        public GameObject separatorPrefab;
        public Transform container;
        public Image scrollBarHandle;
        public CustomScrollRect scrollRect;
        public RectTransform scrollRectTransform;
		public ListLoadAnimation listLoadAnimation;

        private Color scrollBarHandleColor;
        private bool loadLock;

        private bool waitingForPlayerToEndTouch;

        private int currentIndex;
        private int notificationsPrPage;
		private float overDrag;

		private List<GameObject> listElements = new List<GameObject>();

        private void Awake()
        {
            scrollBarHandleColor = scrollBarHandle.color;
        }

        public void SpawnMessage(Notification notification)
        {
            if(container.childCount > 0)
                listElements.Add(Instantiate(separatorPrefab, container));
                
            GameObject last = Instantiate(messagePrefab, container);
			listElements.Add(last);
            NotificationElement element = last.GetComponent<NotificationElement>();
            element.SetMessage(notification);
        }

        private void Update()
        {
			float windowHeight = scrollRectTransform.rect.height;
			float contentHeight = scrollRect.content.sizeDelta.y;
			
			float position = (scrollRect.content.anchoredPosition.y + windowHeight);
			
			overDrag = Mathf.Max(0f, position - contentHeight);

			listLoadAnimation?.SetValue(contentHeight < windowHeight ? 0f : Mathf.Clamp01(overDrag/75f));

            if(scrollRect.m_Dragging)
                scrollBarHandleColor.a = Mathf.Clamp(scrollBarHandleColor.a + (Time.deltaTime*2f), 0f, 0.5f);
            else
                scrollBarHandleColor.a = Mathf.Clamp01(scrollBarHandleColor.a - Time.deltaTime);

            scrollBarHandle.color = scrollBarHandleColor;

            if (loadLock)
                if (!scrollRect.m_Dragging && scrollRect.normalizedPosition.y > -0.15f)
                    loadLock = false;

            if(waitingForPlayerToEndTouch && TouchInput.GetTouchCount() == 0)
            {
                scrollRect.enabled = true;
            }
        }

        public void LoadNotifications()
        {
            ClearMessages();
            FetchNotifications(0, null);
        }

        private void FetchNotifications(int fromIndex, System.Action onComplete)
        {
            NotificationAPI.GetNotifications(fromIndex, (success, response) =>
            {
                if (!success)
                {
                    Debug.LogError("Unable to get notifications");
                    onComplete?.Invoke();
                    return;
                }

                notificationsPrPage = response.NotifcationsPrPage;

                foreach (Notification n in response.Notifications)
                {
                    SpawnMessage(n);
                    currentIndex++;
                }

				listLoadAnimation.rectTransform.SetAsLastSibling();

                if (response.Notifications.Count > 0)
                {
                    List<Notification> unreadNotifications = response.Notifications.Where(x => x.Read == false).ToList();
                    if (unreadNotifications.Count > 0)
                        NotificationAPI.MarkAsRead(unreadNotifications);
                }

                onComplete?.Invoke();
            });
        }

		public void ClearMessages()
		{
			currentIndex = 0;

			foreach(GameObject go in listElements.ToArray())
				Destroy(go);

			listElements = new List<GameObject>();
		}

        public void OnValueChanged()
        {
            if(loadLock)
                return;

            if(!scrollRect.m_Dragging)
                return;
			
            //if contentlist is shorter than window height
			if(scrollRect.content.sizeDelta.y < scrollRectTransform.rect.height)
				return;

            if (overDrag > 75f)
				LoadMore();
        }

        public void LoadMore()
        {
            if (loadLock)
                return;

            loadLock = true;
            scrollRect.enabled = false;
            scrollRect.EndDrag();

            if (currentIndex % notificationsPrPage != 0)
            {
                Debug.Log("No More notifications to load!");
                waitingForPlayerToEndTouch = true;
                return;
            }

            FetchNotifications(currentIndex, ()=>{
                waitingForPlayerToEndTouch = true;
            });
        }
    }
}
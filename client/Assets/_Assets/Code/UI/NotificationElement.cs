using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace SafariForever.Notifications
{
    public class NotificationElement : MonoBehaviour
    {
        public Image UnreadImg;
        public TextMeshProUGUI TitleTextMesh;
        public TextMeshProUGUI BodyTextMesh;
        public Image Background;

        private Notification notification;
        private Tween thisTween;

        public void SetMessage(Notification notification)
        {
            this.notification = notification;
            TitleTextMesh.text = notification.Title.Replace("<mspace=0.4em>","");
            BodyTextMesh.text = notification.Body.Replace("<mspace=0.4em>","");

            UpdateColor(notification.Read);
        }

        public void UpdateColor(bool read)
        {
            UnreadImg.enabled = !read;
            //TitleTextMesh.color = new Color(1,1,1, read ? 0.5f : 1f);
            //Background.color = new Color(0,0,0, read ? 0.5f : 1f);
        }
        
        public void OnClick()
        {
            if(thisTween != null)
                thisTween.Complete();
            
            thisTween = transform.DOPunchScale(new Vector3(-0.2f, -0.3f, 0), 0.3f, 5);
            UpdateColor(true);

            if (!string.IsNullOrEmpty(notification.Body))
            {
                thisTween = DOVirtual.DelayedCall(0.4f, ()=>{
                    Dialog d = new Dialog(TitleTextMesh.text, TitleTextMesh.isRightToLeftText, Globals.localizationConstants.defaultLanguage, BodyTextMesh.text, BodyTextMesh.isRightToLeftText, Globals.localizationConstants.defaultLanguage)
                    .AddNeutralButton(TranslationKey.Generic_Ok, null, true);

                    if(notification.Links != null && notification.Links.Length > 0)
                    {
                        foreach(NotificationLink link in notification.Links)
                        {
                            d.AddNeutralButton(link.ButtonText, Globals.localizationConstants.defaultLanguage, ()=>
                            {
                                if(!DeepLinkSingleton.instance.ParseDeeplink(link.Url))
                                    Application.OpenURL(link.Url);

                            }, false, false);
                        }
                    }

                    d.Show();

                    if(!notification.Read)
                    {
                        UpdateColor(true);
                        NotificationAPI.MarkAsRead(new List<Notification>(){notification});
                    }
                });
            }
        }

        private void KillAllTweens()
        {
            thisTween?.Kill();
        }

        private void OnDestroy()
        {
            KillAllTweens();
        }	
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace SafariForever.Toolbar
{
    public class ToolbarButton : MonoBehaviour
    {
        public LayoutElement layoutElement;
        public Image image;

        public Image icon;
        public TextMeshProUGUI text;

        public LevelEntityButton shortcut;

        private Toolbar toolbar;

        private string defaultName;

        private void Awake()
        {
            toolbar = GetComponentInParent<Toolbar>();

            EventTrigger eventTrigger = GetComponent<EventTrigger>();
            EventTrigger.Entry OnDragBeginEntry = new EventTrigger.Entry();
            OnDragBeginEntry.eventID = EventTriggerType.BeginDrag;
            OnDragBeginEntry.callback.AddListener((eventData) => 
            {
                OnBeginDrag((PointerEventData)eventData);
            });
            eventTrigger.triggers.Add(OnDragBeginEntry);

            SetShortcut(shortcut);

            shortcut = null;
        }

        private void Start()
        {
            defaultName = this.text.text;
        }

        public void OnClick()
        {
            toolbar.SetCategory(transform.GetSiblingIndex(), false);
            
            if(toolbar.IsOpen())
                this.text.Translate(TranslationKey.Editor_Toolbar_Main_Close, SaveManager.currentSave.language, FontType.Stylized, true);
            else if(shortcut != null)
                this.text.text = defaultName;
            else
                this.text.text = defaultName;
        }

        public void SetShortcut(LevelEntityButton shortcut)
        {
            return;
            this.shortcut = shortcut;
            this.icon.sprite = shortcut.icon.sprite;

            if(!toolbar.IsOpen())
                this.text.text = defaultName;//shortcut.text.text;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(shortcut == null)
                return;

            this.text.text = defaultName;//shortcut.text.text;

            toolbar.Close();

            shortcut.OnClick();

            if(shortcut == null)
                return;

            shortcut.OnBeginDrag(eventData);
        }

        private void On_TabChange(int index)
        {
            if(transform.GetSiblingIndex() == index)
                return;

            if(shortcut != null)
                this.text.text = defaultName;//shortcut.text.text;
            else
                this.text.text = defaultName;
        }

        private void OnEnable()
        {
            Toolbar.On_TabChange += On_TabChange;
        }

        private void Unsubscribe()
        {
            Toolbar.On_TabChange -= On_TabChange;
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
}
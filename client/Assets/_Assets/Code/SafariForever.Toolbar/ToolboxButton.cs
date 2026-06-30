using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SafariForever.Toolbar
{
    public class ToolboxButton : MonoBehaviour
    {
        public Toolbar toolbar;

        public void OnClick()
        {
            toolbar.Toggle();
        }
    }
}
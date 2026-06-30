using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViewAllButton : MonoBehaviour
{
    public BrowserTab tab;
    public TextMeshProUGUI titleToCopy;

    public void OnClick()
    {
        LevelsView levelsView =  (LevelsView)PopupCanvas.instance.levelsView;
        levelsView.levelsList.ClearLevels();

        levelsView.levelsList.widgetHeader.isRightToLeftText = titleToCopy.isRightToLeftText;
        levelsView.levelsList.widgetHeader.text = titleToCopy.text;
        levelsView.levelsList.widgetHeader.font = titleToCopy.font;
        
        PopupView currentView = PopupCanvas.instance.GetActivePopup();
        currentView.Close(false, ()=>
        {
            ((BrowserView)PopupCanvas.instance.browserView).SelectLevelList(tab, levelsView.levelsList);

            levelsView.Show(()=>{
                currentView.Show(null);
            });
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class NameChange : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public TextMeshProUGUI header;

    private Tween tween;
    private bool editable = false;

    public delegate void NameUpdateEvent();
    public static NameUpdateEvent On_NameUpdate;

    public void Initalize(string nickname, int identifier, bool editable)
    {
        header.text = nickname;
        header.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, header.font, Globals.localizationConstants.defaultLanguage);
        Localization.UpdateTextAlignment(header, Globals.localizationConstants.defaultLanguage, true);

        textMesh.text = $"{nickname}#{identifier.ToString().PadLeft(4,'0')}"; 
        
        Localization.UpdateTextAlignment(textMesh, Globals.localizationConstants.defaultLanguage, true);

        this.editable = editable;

        OnEnable();
    }

    public void OnEnable()
    {
        if(this.editable)
        {
            textMesh.DOKill();
            textMesh.color = new Color(0.75f, 0.75f, 0.75f, 1f);
            tween = textMesh.DOColor(Color.white, 0.5f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
            
            textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, header.font, Globals.localizationConstants.defaultLanguage);
        }
        else
        {
            KillTweens();
            textMesh.color = Color.white.SetAlpha(0.5f);

            textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized, header.font, Globals.localizationConstants.defaultLanguage);
        }
    }

    public void OnClick()
    {
        if(!editable)
            return;

        //Since this is suppose to show inside the profile menu which is not accessible without a profile we do some shortcuts.
        SaveManager.currentSave.FetchOnlineProfile((profile)=>{

            if(profile == null)
                return;

            DialogCanvas.instance.ShowNicknameWindow(profile.nickname, (confirmed, validatedNickname)=>
            {
                if(!confirmed)
                    return;

                UserAPI.UpdateNickname(validatedNickname, (success, response)=>
                {
                    if(!success)
                        return;

                    profile.nickname = response.Nickname;
                    profile.identifier = response.Identifier;
                    SaveManager.Save();

                    textMesh.text = $"{profile.nickname}#{profile.identifier.ToString().PadLeft(4,'0')}"; 
                    header.text = profile.nickname;
                    On_NameUpdate?.Invoke();
                });
            });
        }, false);
    }

    private void KillTweens()
    {
        tween?.Kill();
    }

    private void OnDisable()
    {
        KillTweens();
    }

    private void OnDestroy()
    {
        KillTweens();
    }
}

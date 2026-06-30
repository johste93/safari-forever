using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionTextMesh : MonoBehaviour {
	
	TextMeshProUGUI textMeshPro;

	private void Start()
	{
		textMeshPro = GetComponent<TextMeshProUGUI>();
		On_LanguageChanged(SaveManager.currentSave.language);
	}

	private void On_LanguageChanged(Language language)
    {
        textMeshPro.text = string.Format("v.{0}   |   © chumpware.com 2019", Application.version);
    }

    private void Unsubscribe()
    {
        LanguageButton.On_LanguageChanged -= On_LanguageChanged;
    }

    private void OnEnable()
    {
        LanguageButton.On_LanguageChanged += On_LanguageChanged;
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

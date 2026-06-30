using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour
{
    public Image image;
    public Sprite[] characters;

    private void Start()
    {
        On_CharacterSelected();
    }

    private void On_CharacterSelected()
    {
        image.sprite = characters[SaveManager.currentSave.currentCharacter];
    }

    private void OnEnable()
    {
        On_CharacterSelected();
        Shop.On_CharacterSelected += On_CharacterSelected;
    }

    private void Unsubscribe()
    {
        Shop.On_CharacterSelected -= On_CharacterSelected;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HatsButton : MonoBehaviour
{
    public GameObject previousHatButton;
    public GameObject nextHatButton;

    private void OnEnable()
    {
        CheckIfAnyUnlockedHats();
    }

    public void OnClick()
    {
        DialogCanvas.instance.ShowCardsWindow(()=>
        {
            CheckIfAnyUnlockedHats();
        });
    }

    private void CheckIfAnyUnlockedHats()
    {
        int unlockedHatCount = SaveManager.currentSave.unlockedHats.Where(x => x == true).Count();

        previousHatButton.SetActive(unlockedHatCount > 1);
        nextHatButton.SetActive(unlockedHatCount > 1);
    }
}

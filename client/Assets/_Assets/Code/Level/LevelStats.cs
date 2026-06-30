using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelStats : MonoBehaviour
{
	public RectTransform rectTransform;
    public TextMeshProUGUI worldHighscore;
	public TextMeshProUGUI localHighscore;

	public void SetLocalHeighScore(string score)
	{
		localHighscore.alignment = TextAlignmentOptions.Center;
		localHighscore.text = score;
	}

	public void SetWorldHeighScore(string score)
	{
		worldHighscore.alignment = TextAlignmentOptions.Center;
		worldHighscore.text = score;
	}
}

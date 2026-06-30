using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsButton : MonoBehaviour
{
    public void OnClick()
	{
		DialogCanvas.instance.ShowCredithWindow();

		//string another = " \n<b>Mostly everything:</b>\nJohannes Stensen\n\n<b>Sound & Music:</b>\n(Stereo Future Productions)\nKristian Andersen\n\n<b>Special Thanks:</b>\nLinn Helen Skrokbæk\nFredrik Martinsen\n ";

		//new Dialog("Credits", another).AddNeutralButton("Get me out of here!", null).Show();
	}
}

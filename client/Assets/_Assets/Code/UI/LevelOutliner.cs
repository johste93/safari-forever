using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafariForever.Toolbar;

public class LevelOutliner : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private void UpdateLineRenderer()
    {
        Vector2 halfSize = new Vector2(Globals.gameConstants.levelWidth/2f, Globals.gameConstants.levelHeight/2f);
        Vector2 bottomLeft = -halfSize;
        Vector2 topRight = halfSize;

        lineRenderer.positionCount = 5;
        lineRenderer.SetPositions(new Vector3[]{
            bottomLeft,
            new Vector3(bottomLeft.x, topRight.y),
            topRight,
            new Vector3(topRight.x, bottomLeft.y),
            bottomLeft
        });
    }

    private void On_EnterPlayMode()
	{
		lineRenderer.enabled = false;
	}

	private void On_ExitPlayMode()
	{
		lineRenderer.enabled = true;
		//lineRenderer.enabled = Toolbar.instance.GetTabIndex() == 1;
	}

    private void On_TabChange(int tabIndex)
	{
		//lineRenderer.enabled = Toolbar.instance.GetTabIndex() == 1;
	}

	private void Subscribe()
	{
		GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;

        Toolbar.On_TabChange += On_TabChange;
	}

	private void Unsubscribe()
	{
		GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
		GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

        Toolbar.On_TabChange -= On_TabChange;
	}

	private void OnEnable()
	{
        UpdateLineRenderer();
		Subscribe();
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

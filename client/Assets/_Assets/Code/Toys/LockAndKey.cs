using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafariForever.Toolbar;
using FSM_CharacterController2D;

public class LockAndKey : MonoBehaviour
{
    public Key key;
    public LineRenderer lineRenderer;

    private LevelEntity entity;
    private PositionGizmo positionGizmo;

    private Vector2 pointA;
    private Vector2 pointB;

    private Vector2 offset = new Vector2(0.5f, 0.5f);

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
		positionGizmo = entity.GetComponentInChildren<PositionGizmo>();

        pointA = entity.GetSerializableData().topRight - offset;
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;

        key.transform.position = pointB;
        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});
    }

    private void Reset()
    {
        key.transform.position = pointB;
        key.Reset();
    }

    private void On_EnterPlayMode()
    {
        Reset();
        UpdateLineRendererVisibility(false);
    }

    private void On_ExitPlayMode()
    {
        Reset();
        UpdateLineRendererVisibility(Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);
    }

    private void UpdatePosition()
    {
        pointB = positionGizmo.transform.position;
        key.transform.position = pointB;
      
        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});
        LevelBuilder.instance.UpdateTiles();
    }

    private void UpdateLineRendererVisibility(bool interactable)
    {
        lineRenderer.enabled = interactable;
    }

    private void On_TabChange(int tabIndex)
    {
        UpdateLineRendererVisibility(tabIndex == (int)entity.requiredTab);
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;

        positionGizmo.On_PositionMoved += UpdatePosition;

        Toolbar.On_TabChange += On_TabChange;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

        positionGizmo.On_PositionMoved -= UpdatePosition;

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

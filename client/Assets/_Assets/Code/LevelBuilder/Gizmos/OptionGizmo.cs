using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class OptionGizmo : Gizmo
{
    public TextMeshPro textMeshPro;
    public string[] possibleValues;
    public int currentValue;

    public int index
    {
        get
        {
            if (!levelEntity.GetSerializableData().gizmoIndex.HasValue)
                levelEntity.GetSerializableData().gizmoIndex = currentValue;

            return levelEntity.GetSerializableData().gizmoIndex.Value;
        }
        set
        {
            levelEntity.GetSerializableData().gizmoIndex = value;
        }
    }

    public string GetValue()
    {
        return possibleValues[index];
    }

    public SpriteRenderer face;
    public Transform shadow;

    private void Start()
    {
        textMeshPro.text = GetValue().ToString();
    }

    public override void On_TouchStart(TouchInfo touch)
    {
        if (touch.pickedUIElement != null)
            return;

        if (touch.GetFirstPickedGameObject(Camera.main) != gameObject)
            return;

        base.On_TouchStart(touch);

        assignedFingerIndex = touch.fingerIndex;

        touchOffset = transform.position - touch.GetTouchToWorldPoint(10, Camera.main);
    }

    public override void On_TouchMaintained(TouchInfo touch)
    {
        if (touch.fingerIndex != assignedFingerIndex)
            return;

        base.On_TouchMaintained(touch);

        Vector3 touchPos = touch.GetTouchToWorldPoint(10, Camera.main) + (Vector3)touchOffset;
        levelEntity.SetPositon(touchPos);
    }

    public override void On_TouchEnd(TouchInfo touch)
    {
        if (touch.fingerIndex != assignedFingerIndex)
            return;

        base.On_TouchEnd(touch);

        assignedFingerIndex = -1;

        levelEntity.OnFinishedMoving();
    }

    public override void Tap()
    {
        face.transform.DOComplete();
        shadow.transform.DOComplete();
        textMeshPro.transform.DOComplete();

        face.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 1);
        shadow.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 1);

        face.transform.DORotate(new Vector3(0f, 0f, 180f), 0.3f, RotateMode.FastBeyond360).SetRelative(true);
        shadow.transform.DORotate(new Vector3(0f, 0f, 180f), 0.3f, RotateMode.FastBeyond360).SetRelative(true);
        textMeshPro.transform.DORotate(new Vector3(0f, 0f, 360f), 0.3f, RotateMode.FastBeyond360).SetRelative(true);
        ChangeNumber();

        base.Tap();
    }

    private void ChangeNumber()
    {
        index++;
        if (index >= possibleValues.Length)
            index = 0;

        textMeshPro.text = GetValue().ToString().Replace(',', '.');
    }

    private void OnEnable()
    {
        TouchInput.On_TouchStart += On_TouchStart;
        TouchInput.On_TouchMaintained += On_TouchMaintained;
        TouchInput.On_TouchEnd += On_TouchEnd;
    }

    private void Unsubscribe()
    {
        TouchInput.On_TouchStart -= On_TouchStart;
        TouchInput.On_TouchMaintained -= On_TouchMaintained;
        TouchInput.On_TouchEnd -= On_TouchEnd;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Unsubscribe();
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        face.transform.DOKill();
        shadow.transform.DOKill();
        textMeshPro.transform.DOKill();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntitySelector : MonoBehaviour
{
    public LayerMask whatIsSelectableLevelEntityCollider;
    private Collider2D[] previouslyClickedColliders;

    private int selectedIndex = 0;
    private LevelEntity previouslySelectedEntity;

    public void On_TouchStart(TouchInfo touch)
    {
        if (touch.pickedUIElement != null)
            return;
            
        if(touch.GetFirstPickedGameObject(Camera.main) != null)
            return;

        Vector3 touchPos = touch.GetTouchToWorldPoint(10, Camera.main);
        Collider2D[] clickedColliders = Physics2D.OverlapPointAll(touchPos, whatIsSelectableLevelEntityCollider);

        if(Equal(clickedColliders, previouslyClickedColliders))
        {
            selectedIndex++;
            if(selectedIndex >= previouslyClickedColliders.Length)
                selectedIndex = 0;

            Select(previouslyClickedColliders[selectedIndex], touch);
        }
        else
        {
            selectedIndex = -1;
            previouslyClickedColliders = clickedColliders;

            if(previouslyClickedColliders.Length > 0)
            {
                selectedIndex = 0;

                if(previouslyClickedColliders.Length > 1)
                {
                    if(AlreadySelected(previouslyClickedColliders[selectedIndex]))
                    {
                        selectedIndex++;
                    }
                }

                Select(previouslyClickedColliders[selectedIndex], touch);
            }
        }
    }

    private void Select(Collider2D collider, TouchInfo touch)
    {
        Gizmo gizmo = collider.transform.parent.GetComponent<Gizmo>();
        gizmo.levelEntity.OnTouchStart(touch);
    }

    private bool AlreadySelected(Collider2D collider)
    {
        Gizmo gizmo = collider.transform.parent.GetComponent<Gizmo>();
        return gizmo.levelEntity == previouslySelectedEntity;
    }

    private bool Equal(Collider2D[] a, Collider2D[] b)
    {
        if(a == null || b == null)
            return false;

        if(a.Length == 0 || b.Length == 0)
            return false;

        if(a.Length != b.Length)
            return false;

        foreach(Collider2D c2D in a)
        {
            if(!b.Contains(c2D))
                return false;
        }

        return true;
    }

    private void On_EntitySelected(LevelEntity entity)
    {
        previouslySelectedEntity = entity;
    }

    private void OnEnable()
    {
        TouchInput.On_TouchStart += On_TouchStart;
        LevelEntity.On_EntitySelected += On_EntitySelected;
    }

    private void Unsubscribe()
    {
        TouchInput.On_TouchStart -= On_TouchStart;
        LevelEntity.On_EntitySelected -= On_EntitySelected;
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

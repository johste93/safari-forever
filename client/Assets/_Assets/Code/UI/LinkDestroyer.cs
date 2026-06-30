using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF.LogicSystem.v2;

public class LinkDestroyer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask whatIsLink;
    public LayerMask whatIsRailLink;

    private int assignedFingerIndex = -1;

    private void On_TouchStart(TouchInfo touch)
    {
        if (touch.pickedUIElement != null)
            return;
            
        if(touch.GetFirstPickedGameObject(Camera.main) != null)
			return;
        
        if(touch.GetFirstPickedUIElement() != null)
            return;

		assignedFingerIndex = touch.fingerIndex;

        Vector3 touchPos = touch.GetTouchToWorldPoint(10, Camera.main);
        touchPos.z = 0;

        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, touchPos);
        lineRenderer.SetPosition(1, touchPos);
        lineRenderer.SetPosition(2, touchPos);
    }

    private void On_TouchMaintained(TouchInfo touch)
    {
        if (touch.fingerIndex != assignedFingerIndex)
			return;

        Vector3 touchPos = touch.GetTouchToWorldPoint(10, Camera.main);
        touchPos.z = 0;
        lineRenderer.SetPosition(2, touchPos);
        lineRenderer.SetPosition(1, Vector3.Lerp(lineRenderer.GetPosition(0), lineRenderer.GetPosition(2), 0.5f));

        foreach(RaycastHit2D hit in GetInterceptedLinks())
        {
           hit.collider.GetComponent<Link>().lastInterception = Time.frameCount;
        }

        foreach(RaycastHit2D hit in GetInterceptedRailLinks())
        {
           hit.collider.GetComponent<RailLink>().lastInterception = Time.frameCount;
        }

        //CameraScaler.instance.ZoomInOnLink(lineRenderer.GetPosition(0), touchPos);
    }  

    private void On_TouchEnd(TouchInfo touch)
    {
        if (touch.fingerIndex != assignedFingerIndex)
			return;

        assignedFingerIndex = -1;

        Vector3 touchPos = touch.GetTouchToWorldPoint(10, Camera.main);
        touchPos.z = 0;
        lineRenderer.SetPosition(1, Vector3.Lerp(lineRenderer.GetPosition(0), lineRenderer.GetPosition(2), 0.5f));
        lineRenderer.SetPosition(2, touchPos);

        Audio.Play(SFX.instance.level.logicSwitch.detach, Channel.Game);
        int linksDestroyd = 0;
        RaycastHit2D[] hits = GetInterceptedLinks();
        foreach(RaycastHit2D hit in hits)
        {
           Link link = hit.collider.GetComponent<Link>();
           link.Detach();
           linksDestroyd++;
        }

        RaycastHit2D[] railHits = GetInterceptedRailLinks();
        foreach(RaycastHit2D hit in railHits)
        {
           RailLink link = hit.collider.GetComponent<RailLink>();
           link.Detach();
           linksDestroyd++;
        }

        if(linksDestroyd > 0)
            Audio.Play( SFX.instance.level.logicSwitch.detach, Channel.Game );

        lineRenderer.positionCount = 0;

        //CameraScaler.instance.ResetLinkZoom();
    }

    private RaycastHit2D[] GetInterceptedLinks()
    {
        return Physics2D.LinecastAll(lineRenderer.GetPosition(0), lineRenderer.GetPosition(2), whatIsLink);
    }

    private RaycastHit2D[] GetInterceptedRailLinks()
    {
        return Physics2D.LinecastAll(lineRenderer.GetPosition(0), lineRenderer.GetPosition(2), whatIsRailLink);
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

    private void OnDestroy()
    {
        Unsubscribe();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLaserBeam : MonoBehaviour
{
    public AimLaserCannon laserCannon;
    public Transform beam;
    public Transform beamStart;
    public Transform beamEnd;
    public LayerMask whatIsObstacle;
    public BoxCollider2D boxCollider;
    public GameObject beamStartCore;
    public GameObject beamEndCore;

    private const float maxLength = 30f;
    private const float maxWidth = 0.75f;
    private const float minSize = 0.1f;
    private const float beamEndScale = 1f;

    private float beamWidth;
    private float fluctuation;
    private float distance;
    
    public void Prepare(Vector2 direction)
    {
        beamWidth = 0f;
        AdjustLength(direction);

        beam.localScale = new Vector3(beamWidth, distance, beam.localScale.y);
        beamStart.localScale = new Vector3(beamWidth, beamWidth, 1f);
        beamEnd.localScale = beamStart.localScale * beamEndScale;

        beamStart.gameObject.SetActive(true);
        beam.gameObject.SetActive(true);
        beamEnd.gameObject.SetActive(true);
        beamStartCore.SetActive(false);
        beamEndCore.SetActive(false);
    }

    public void AdjustLength(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(beamStart.transform.position, direction, maxLength, whatIsObstacle);

        distance = maxLength;
        if(hit.collider != null)
        {
            distance = hit.distance - (0.5f);
        }

        beamEnd.localPosition = new Vector3(0, distance, 0f);
    }

    public void Fire()
    {
        boxCollider.gameObject.SetActive(true);
        beamStartCore.SetActive(true);
        beamEndCore.SetActive(true);

        beam.localScale = new Vector3(beamWidth, distance, beam.localScale.y);
        beamStart.localScale = new Vector3(beamWidth, beamWidth + fluctuation, 1f);
        beamEnd.localScale = beamStart.localScale * beamEndScale;
    }

    public void Stop()
    {
        boxCollider.gameObject.SetActive(false);
        beamStartCore.SetActive(false);
        beamEndCore.SetActive(false);

        //beamStart.gameObject.SetActive(false);
        //beam.gameObject.SetActive(false);
        //beamEnd.gameObject.SetActive(false);

    }

    private void Update()
    {
		if(laserCannon.isSuspended)
			return;

        if(!laserCannon.isFiring && !laserCannon.isPreparing)
        {
            if(beamWidth > 0f)
                beamWidth = Mathf.Max( beamWidth - (Time.deltaTime * SaveManager.currentSave.gameSpeed * 4f) , 0f);

            beam.localScale = new Vector3(beamWidth, distance, beam.localScale.y);
            beamStart.localScale = new Vector3(beamWidth, beamWidth, 1f);
            beamEnd.localScale = beamStart.localScale * beamEndScale;
        }

        if(laserCannon.isPreparing)
        {
            if(beamWidth < minSize)
                beamWidth += Time.deltaTime * LaserCannon.warmUpDuration * minSize * SaveManager.currentSave.gameSpeed;

            beam.localScale = new Vector3(beamWidth, distance, beam.localScale.y);
            beamStart.localScale = new Vector3(beamWidth, beamWidth, 1f);
            beamEnd.localScale = beamStart.localScale * beamEndScale;
        }

        if(!laserCannon.isFiring)
            return;

        fluctuation = Mathf.Abs(Mathf.Sin((Time.time - laserCannon.startFireTime) * 16f) * 0.2f);

        if(beamWidth < maxWidth)
            beamWidth += Time.deltaTime * SaveManager.currentSave.gameSpeed * 4f;
        

        if(beamWidth + fluctuation < maxWidth)
        {
            beam.localScale = new Vector3(beamWidth, distance, beam.localScale.y);
            beamStart.localScale = new Vector3(beamWidth, beamWidth, 1f);
            beamEnd.localScale = beamStart.localScale * beamEndScale;
        }
        else
        {
            beam.localScale = new Vector3(beamWidth + fluctuation, distance, beam.localScale.y);
            beamStart.localScale = new Vector3(beamWidth + fluctuation, beamWidth + fluctuation, 1f);
            beamEnd.localScale = beamStart.localScale * beamEndScale;
        }
    }
}

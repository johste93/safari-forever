using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ghost : MonoBehaviour
{
	public Transform pivot;
    public GhostRecording recording;

	private int currentFrame = 0;

	private float previousFaceDir;

	public void StartRunning(GhostRecording recording)
	{
		this.recording = recording;
		previousFaceDir = Mathf.Sign(recording.path[0].z);
	}

	private void Update()
	{
		if(recording == null)
			return;

		if(currentFrame >= recording.path.Count-1)
			return;
			
		transform.position = (Vector2) recording.path[currentFrame];
		currentFrame++;

		float faceDir = Mathf.Sign(recording.path[currentFrame].z);

		if(faceDir != previousFaceDir)
		{
			pivot.DOComplete();
			pivot.DOScaleX(faceDir * Mathf.Abs(transform.localScale.x), 0.15f).SetEase(Ease.InOutQuad);
		}

		previousFaceDir = faceDir;
	}

	public void Rewind()
	{
		currentFrame = 0;
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		pivot.DOKill();
	}
}

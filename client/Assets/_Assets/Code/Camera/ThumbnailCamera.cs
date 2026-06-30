using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbnailCamera : Singleton<ThumbnailCamera>
{
    private Camera cam;

    private float currentMinWidth;
	private float currentMinHeight;

    private float minOrthographicSize;
    private float editorMinWidth = 12.5f;

	private byte[] largeThumbnail;
	private byte[] smallThumbnail;

    private void Awake()
	{
		cam = GetComponent<Camera>();
		editorMinWidth = (Globals.gameConstants.levelWidth/2f) + 2.75f;
		currentMinWidth = editorMinWidth;
		minOrthographicSize = Mathf.Max(18f, currentMinWidth / cam.aspect);
		cam.orthographicSize = minOrthographicSize;
		cam.enabled = false;
	}

    public void Snap()
    {
		PrepareCamera();
		cam.enabled = true;

		cam.targetTexture = new RenderTexture(400, 400, 24, RenderTextureFormat.ARGB32);
		cam.targetTexture.Create();

		cam.Render();

        Texture2D largeTexture2D = ToTexture2D(cam.targetTexture);
		largeTexture2D.Apply();
		

		cam.targetTexture = new RenderTexture(64, 64, 24, RenderTextureFormat.ARGB32);
		cam.targetTexture.Create();

		cam.Render();
		
        Texture2D smallTexture2D = ToTexture2D(cam.targetTexture);
		smallTexture2D.Apply();

		cam.enabled = false;

		largeTexture2D = DecreaseColourDepth(largeTexture2D, 16);
		smallTexture2D = DecreaseColourDepth(smallTexture2D, 16);

		largeThumbnail = largeTexture2D.EncodeToGIF();
		smallThumbnail = smallTexture2D.EncodeToGIF();
		
		//System.IO.File.WriteAllBytes(Application.persistentDataPath + "/smallThumbnail.gif", smallThumbnail);
		//Debug.Log("gif Thumbnail size after colordepth reduction: " + lastThumbnail.Length + " bytes");

		//Debug.Log("Brotli compressed: " + Brotli.Compress(lastThumbnail).Length + " bytes");
    }

	private Texture2D DecreaseColourDepth(Texture2D input, int offset)
	{
		Texture2D output = new Texture2D(input.width, input.height);

		for (int y = 0; y < input.height; y++)
		{
			for (int x = 0; x < input.width; x++)
			{
				Vector3Int pixelColor = input.GetPixel(x, y).ToVector3Int();

				int R = Mathf.Max(0, (pixelColor.x + offset / 2) / offset * offset - 1);
				int G = Mathf.Max(0, (pixelColor.y + offset / 2) / offset * offset - 1);
				int B = Mathf.Max(0, (pixelColor.z + offset / 2) / offset * offset - 1);

				output.SetPixel(x,y, new Color(R/255f, G/255f, B/255f));
			}
		}

		return output;
	}

	public byte[] GetLargeThumbnail()
	{
		if(largeThumbnail == null)
		{
			Debug.LogError("No thumbnail has been captured yet!");
		}

		return largeThumbnail;
	}

	public byte[] GetSmallThumbnail()
	{
		if(smallThumbnail == null)
		{
			Debug.LogError("No thumbnail has been captured yet!");
		}

		return smallThumbnail;
	}

    private void PrepareCamera()
    {
		UpdateCameraPosition();
       	UpdateCameraSize();

		cam.backgroundColor = LevelBuilder.instance.GetCurrentColors().main;
    }

	private void UpdateCameraSize()
	{
		/*
		Vector4 realLevelSize = LevelBuilder.instance.GetRealLevelSize();
		float levelWidth = realLevelSize.x;
		float levelHeight = realLevelSize.y;

		if(levelWidth >= levelHeight)
		{
			//match width
			//4:3 image
			cam.orthographicSize = (levelWidth/2f) + 3;
		}
		else
		{
			//3:4 image
			//match height
			cam.orthographicSize = (levelHeight/2f) +3;
		}
		*/

		Vector4 realLevelSize = LevelBuilder.instance.GetRealLevelSize();
		float levelWidth = realLevelSize.x;
		float levelHeight = realLevelSize.y;

        float heightBasedSize = (levelHeight / 2f);
        float widthBasedSize = (levelWidth / 2);
        
		cam.orthographicSize = (Mathf.Max(widthBasedSize, heightBasedSize) + 3f) / cam.aspect;
	}

	private void UpdateCameraPosition()
	{
		Vector4 realLevelSize = LevelBuilder.instance.GetRealLevelSize();
		float x = realLevelSize.z;
		float y = realLevelSize.w;
		cam.transform.position = new Vector3(x,y,cam.transform.position.z); 
	}

	private Texture2D ToTexture2D(RenderTexture rTex)
	{
		Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
		RenderTexture.active = rTex;
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		tex.Apply();
		return tex;
	}

	private void On_EnterPlayMode()
    {
		//Snap();
    }

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
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

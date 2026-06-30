using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteOptimizer : EditorWindow
{
    private Texture2D selectedTexture2D;
    private string pathToSelectedTexture;
    private int targetSize;

    
    [MenuItem("Chumpware/SpriteOptimizer")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SpriteOptimizer window = (SpriteOptimizer)EditorWindow.GetWindow(typeof(SpriteOptimizer));
        window.Show();
    }

    void OnGUI()
    {
        GUI.enabled = false;
        //Get selected sprite.
        EditorGUILayout.TextField("Selected Texture:", pathToSelectedTexture);

        GUI.enabled = !string.IsNullOrEmpty(pathToSelectedTexture);
        if(GUILayout.Button("Optimize"))
        {
            Optimize();
        }
        GUI.enabled = true;
    }

    private void Optimize()
    {
        bool wasReadable = selectedTexture2D.isReadable;
        if(!wasReadable)
            SetTextureImporterFormat(selectedTexture2D, true);

        //Texture2D optimizedTexture = new Texture2D(selectedTexture2D.width, selectedTexture2D.height, selectedTexture2D.format, selectedTexture2D.mipmapCount, true);
        //Graphics.CopyTexture(selectedTexture2D, optimizedTexture);

        /*
		Texture2D texResult = new Texture2D(targetSize, targetSize);
        // Replace all pixels with a zero-ed pixel array (ensure alpha value equal zero)
        Color32[] pixels = new Color32[targetSize*targetSize];

        Color32[] sourcePixels = selectedTexture2D.GetPixels32();

        int xPos = 0;
        int sourceIndex = 0;
        for(int i = 0; sourceIndex < sourcePixels.Length; i++)
        {
            if(xPos > selectedTexture2D.width)
            {
                int remainingPixelsOnLine = targetSize - selectedTexture2D.width;
                Debug.Log(remainingPixelsOnLine);
                i += remainingPixelsOnLine;
                xPos = 0;
            }   

            pixels[i] = sourcePixels[sourceIndex];
            sourceIndex++;
        }

        texResult.SetPixels32(pixels);
		texResult.Apply();

        byte[] png = texResult.EncodeToPNG();
        File.Delete(pathToSelectedTexture);
        File.WriteAllBytes(pathToSelectedTexture, png);
        AssetDatabase.Refresh();
        //CreateOrReplaceAsset<Texture2D>(optimizedTexture, pathToSelectedTexture);
        */
        //selectedTexture2D.Resize(targetSize,targetSize);
        //selectedTexture2D.Apply();

        File.WriteAllBytes(pathToSelectedTexture, AddWatermark(new Texture2D(targetSize, targetSize), selectedTexture2D).EncodeToPNG());
    }

    public static Texture2D Resize(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0,0);
        nTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return nTex;
    }


    public Texture2D AddWatermark(Texture2D background, Texture2D watermark)
    {

        int startX = 0;
        int startY = background.height - watermark.height;

        for (int x = startX; x < background.width; x++)
        {

            for (int y = startY; y < background.height; y++)
            {
                Color bgColor = background.GetPixel(x, y);
                Color wmColor = watermark.GetPixel(x - startX, y - startY);

                Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                background.SetPixel(x, y, final_color);
            }
        }

        background.Apply();
        return background;
    }

     
    private void OnSelectionChange()
    {
        if(Selection.activeObject is Texture2D && Selection.assetGUIDs.Length > 0)
        {
            selectedTexture2D = Selection.activeObject as Texture2D;
            pathToSelectedTexture = AssetDatabase.GetAssetPath(selectedTexture2D);
        }
        else
        {
            selectedTexture2D = null;
            pathToSelectedTexture = null;
        }

        targetSize = GetTargetSize();

        Repaint();
    }

    private int GetTargetSize()
    {
        int width = upper_power_of_two(selectedTexture2D.width);
        int height = upper_power_of_two(selectedTexture2D.height);
        
        if(width > height)
            return width;
        else
            return height;
    }

    private T CreateOrReplaceAsset<T> (T asset, string path) where T:Object
    {
        T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);
        
        if (existingAsset == null){
            AssetDatabase.CreateAsset(asset, path);
            existingAsset = asset;
        }
        else{
            EditorUtility.CopySerialized(asset, existingAsset);
        }
        
        return existingAsset;
    }

    private static void SetTextureImporterFormat( Texture2D texture, bool isReadable)
    {
        if ( null == texture ) return;

        string assetPath = AssetDatabase.GetAssetPath( texture );
        var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
        if ( tImporter != null )
        {
            tImporter.textureType = TextureImporterType.Sprite;

            tImporter.isReadable = isReadable;

            AssetDatabase.ImportAsset( assetPath );
            AssetDatabase.Refresh();
        }
    }

    private int upper_power_of_two(int v)
    {
        v--;
        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;
        v++;
        return v;
    }
}

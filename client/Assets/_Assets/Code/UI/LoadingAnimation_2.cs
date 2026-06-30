using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimation_2 : MonoBehaviour
{
    public Image[] images;
    public float speed = 8f;
    public float lag = 2f;

    private int currentIndex;
    private float timer;

    private void OnEnable()
    {
        for(int i = 0; i < images.Length; i++)
        {
            images[i].color = Color.clear; //GetRandomColor(Mathf.Clamp01(images[i].color.a - ((Time.deltaTime * speed)/(images.Length+1))));
        }
    }

    private void Update()
    {
        for(int i = 0; i < images.Length; i++)
        {
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, Mathf.Clamp01(images[i].color.a - ((Time.deltaTime * speed)/(images.Length-lag))));
        }

        timer += Time.deltaTime * speed;
        int timerFloored = Mathf.FloorToInt(timer);

        if(timerFloored != currentIndex)
        {
            if(timer >= images.Length)
            {
                timer = 0f;
                timerFloored = 0;
            }

            currentIndex = timerFloored;
            images[currentIndex].color = GetRandomColor(0.8f);
        }
    }

    private Color GetRandomColor(float alpha)
    {
        float vibrance = 1f;
        float saturation = Random.Range(0.3f,0.9f);
        float hue = Random.Range(0f,1f);
        return Color.HSVToRGB(hue, saturation, vibrance).SetAlpha(alpha);
    }
}

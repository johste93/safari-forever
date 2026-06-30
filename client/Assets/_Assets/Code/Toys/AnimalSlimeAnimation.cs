using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimalSlimeAnimation : MonoBehaviour
{
    public List<Transform> balls;
    private List<Tween> tweens;
    private List<Vector3> defaultScales;

    private void Awake()
    {
        tweens = new List<Tween>();
        defaultScales = new List<Vector3>();

        foreach(Transform t in balls)
            defaultScales.Add(t.localScale);
    }

    private void OnEnable()
    {
        Reset();


        int j = 0;
        for(int i = 0; i < balls.Count; i++)
        {
            tweens.Add(balls[i].DOScale(defaultScales[i], 0.2f).SetEase(Ease.OutBack).SetDelay(j * 0.05f));
            if(i % 2 == 0)
                j++;
        }
    }

    private void Reset()
    {
        KillAllTweens();

        foreach(Transform t in balls)
            t.localScale = Vector3.zero;
    }

    private void KillAllTweens()
    {
        foreach(Tween t in tweens)
            t?.Kill();

        tweens = new List<Tween>();
    }

    private void OnDisable() => Reset();
}

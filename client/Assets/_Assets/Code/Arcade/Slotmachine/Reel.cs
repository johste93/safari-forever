using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Reel : MonoBehaviour
{
    public float maxSpeed = -1500;
    public float minSpeed = -500;
    public float currentSpeed = -500;
    public List<Transform> symbols;

    private Dictionary<Transform, Symbol> symbolsDictionary;
    
    private RectTransform reelRect;
    public float currentOffset;
    private float fixedOffset = 15f;

    private const float step = 65f;
    
    private bool slowdown;
    private float slowdownDuration = 1f;

    private bool stopOnNextSpin;
    private int stepsBeforeStop;
    private float stopDuration = 1f;
    private int fullSpinsWhileSlowingDown = 2;

    private State currentState = State.Stopped;
    private float normalizedSpeed;
    private float timeToStop;

    private void Start()
    {
        reelRect = GetComponent<RectTransform>();
        symbolsDictionary = new Dictionary<Transform, Symbol>(){
            {symbols[0], Symbol.Grapes},
            {symbols[1], Symbol.Pineapple},
            {symbols[2], Symbol.Watermelon},
            {symbols[3], Symbol.Banana},
            {symbols[4], Symbol.Orange},
            {symbols[5], Symbol.Cherry},
            {symbols[6], Symbol.Pear},
            {symbols[7], Symbol.Apple},
        };

        SetSymbol((Symbol) Random.Range(0, symbols.Count));
    }

    public void Spinn()
    {            
        normalizedSpeed = 1f;
        slowdown = false;
        currentState = State.Spinning;
        currentSpeed = maxSpeed;
    }

    public void Stop(Symbol targetSymbol, int fullSpinsWhileSlowingDown)
    {
        if(currentState != State.Spinning)
            return;
        
        this.fullSpinsWhileSlowingDown = Mathf.Max(1, fullSpinsWhileSlowingDown);

        stepsBeforeStop = GetNumberOfStepsToTargetSymbol(targetSymbol);
        stepsBeforeStop += symbols.Count * this.fullSpinsWhileSlowingDown;

        slowdown = true;
        timeToStop = 1f;
    }

    public void SetSymbol(Symbol targetSymbol)
    {
        currentState = State.Stopped;
        normalizedSpeed = 1f;
        slowdown = false;

        int stepsToTarget = GetNumberOfStepsToTargetSymbol(targetSymbol)-1;
        if(stepsToTarget < 0)
            stepsToTarget = symbols.Count-1;

        for(int i = 0; i < stepsToTarget; i++)
            MoveFirstSymbolToTop();

        currentOffset = -FindCenterPosition();
        timeToStop = 0f;
    }

    private int GetNumberOfStepsToTargetSymbol(Symbol targetSymbol)
    {
        int currentSymbolIndex = (int) symbolsDictionary[symbols[0]];
        int stepCount = 0;

        while(currentSymbolIndex != (int)targetSymbol)
        {
            currentSymbolIndex++;
            if(currentSymbolIndex >= symbols.Count)
                currentSymbolIndex = 0;

            stepCount++;
        }
        
        return stepCount;
    }

    public void Stop()
    {
        Stop(Symbol.Apple, 1);
    }

    public State GetCurrentState()
    {
        return currentState;
    }

    private void Update()
    {
        switch(currentState)
        {
            case State.Stopped:
                break;

            case State.Stopping:

                float targetOffset = -FindCenterPosition();
                
                if(timeToStop > 0)
                    timeToStop -= Time.deltaTime/stopDuration;
                else
                {
                    timeToStop = 0;
                    currentOffset = targetOffset;
                    currentState = State.Stopped;
                }

                currentOffset = targetOffset * EaseOutBack(1f - timeToStop);
                break;

            case State.Spinning:

                currentOffset += Time.deltaTime * Mathf.Lerp(minSpeed, maxSpeed, normalizedSpeed);
                
                if(slowdown)
                {
                    normalizedSpeed = (float)stepsBeforeStop / (float)(symbols.Count * fullSpinsWhileSlowingDown);
                }
                else
                {
                    normalizedSpeed = 1f;
                }

                if(Mathf.Abs(currentOffset) >= step)
                {
                    currentOffset = 0f;
                    MoveFirstSymbolToTop();
                    Audio.Play(SFX.instance.ui.slotMachine.spinnTick, Channel.UI).SetPitch(Mathf.Lerp(0.7f, 1f, normalizedSpeed));
                    
                    if(slowdown)
                    {
                        stepsBeforeStop--;

                        if(stepsBeforeStop < 1)
                        {
                            Debug.LogError("stepsBeforeStop is below 1!");
                        }

                        if(stepsBeforeStop == 1)
                        {
                            currentState = State.Stopping;
                        }
                    }
                }

                break;
        }

        for(int i = 0; i < symbols.Count; i++)
        {
            symbols[i].localPosition = new Vector3(0, (step * i) + currentOffset, 0);
        }
    }

    private void MoveFirstSymbolToTop()
    {
        Transform symbolToMoveToTop = symbols[0];

        for(int i = 0; i < symbols.Count-1; i++)
            symbols[i] = symbols[i+1];

        symbols[symbols.Count-1] = symbolToMoveToTop;

        symbolToMoveToTop.SetAsLastSibling();
    }

    private float FindCenterPosition()
    {
        return (reelRect.sizeDelta.y * 0.5f) - (25f); //25 is half the height of a symbol witch is 50.
    }
    


    private float EaseOutBack(float x) 
    {
        float magicNumber = 3f;
        return 1 + (magicNumber + 1) * Mathf.Pow(x - 1f, 3f) + magicNumber * Mathf.Pow(x - 1f, 2f);
    }
    
    public enum State
    {
        Stopped,
        Spinning,
        Stopping
    }
}

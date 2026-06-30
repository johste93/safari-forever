using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlotMachine : MonoBehaviour
{
    public Crank crank;
    public Reel[] reels;

    private State currentState;
    private Coroutine coroutine;

    private System.Action onStartedSpinning;
    private System.Action onStopSpinning;

    private float spinnUpTime;

    public void Spinn(System.Action onStartedSpinning)
    {
        if(currentState != State.Ready)
            return;

        this.onStartedSpinning = onStartedSpinning;

        spinnUpTime = 0;
        spinnUpTime += Spinn(0);
        spinnUpTime += Spinn(1);
        spinnUpTime += Spinn(2);

        currentState = State.PrepareToSpinn;
    }
    
    private bool leverSoundPlayed;
    public void OnValueChanged(float value)
    {
        if(!IsReady())
            return;

        if(crank.slider.value > Crank.MaxValueWhenLocked && !leverSoundPlayed && crank.interactable)
        {
            Audio.Play(SFX.instance.ui.slotMachine.leverDown, Channel.UI);
            leverSoundPlayed = true;
        }
    }

    public void LetLeverGo()
    {
        if(leverSoundPlayed)
        {
            Audio.Play(SFX.instance.ui.slotMachine.leverUp, Channel.UI);
            leverSoundPlayed = false;
        }
    }

    private void Update()
    {
        if(currentState == State.PrepareToSpinn)
        {
            if(spinnUpTime > 0f)
            {
                spinnUpTime -= Time.deltaTime;
            }
            else
            {
                currentState = State.Spinning;
                onStartedSpinning?.Invoke();
            }
        }
    }

    private float Spinn(int i)
    {
        float delay = 0.1f * i;
        this.Delay(delay,()=>{
            reels[i].Spinn();
        });
        return delay;
    }

    public void Stop()
    {
        if(currentState != State.Spinning)
            return;

        int result = Random.Range(0, 2);
        if( result == 1 )
        {
            Win();
        }
        else
        {
            Loose();
        }
    }

    private IEnumerator WaitForReelsToStop()
    {
        while( reels[0].GetCurrentState() != Reel.State.Stopped || 
            reels[1].GetCurrentState() != Reel.State.Stopped || 
            reels[2].GetCurrentState() != Reel.State.Stopped)
            {
                yield return 0;
            }

        currentState = State.Ready;
        coroutine = null;
        onStopSpinning?.Invoke();
        onStopSpinning = null;
    }

    public void Win(System.Action onStopSpinning = null)
    {
        this.onStopSpinning = onStopSpinning;
        int numberOfSymbols = System.Enum.GetNames(typeof(Symbol)).Length;
        Symbol winningSymbol = (Symbol) Random.Range(0, numberOfSymbols);
        
        for(int i = 0; i < reels.Length; i++)
            reels[i].Stop(winningSymbol, 1 + (2 * i));

        coroutine = StartCoroutine(WaitForReelsToStop());
    }

    public void Loose(System.Action onStopSpinning = null, bool instant = false)
    {
        this.onStopSpinning = onStopSpinning;
        int numberOfSymbols = System.Enum.GetNames(typeof(Symbol)).Length;

        Symbol[] result = new Symbol[3];
        while(result[0] == result[1] && result[0] == result[2])
        {
            result = new Symbol[]{
                (Symbol) Random.Range(0, numberOfSymbols),
                (Symbol) Random.Range(0, numberOfSymbols),
                (Symbol) Random.Range(0, numberOfSymbols),
            };
        }

        for(int i = 0; i < reels.Length; i++)
            reels[i].Stop(result[i], 1 + (2 * i));

        coroutine = StartCoroutine(WaitForReelsToStop());
    }

    private void OnDisable()
    {
        onStartedSpinning = null;
        onStopSpinning = null;
        
        currentState = State.Ready;

        onStopSpinning = null;
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public bool IsReady()
    {
        return currentState == State.Ready;
    }

    public enum State{
        Ready,
        PrepareToSpinn,
        Spinning
    }
}

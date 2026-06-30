using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSFXPool : ScriptableObject
{
    public FlagPole flagPole;
    public Door door;
    public Cannon cannon;
    public BreakableBlock breakableBlock;
    public Saw saw;
    public LogicSwitch logicSwitch;
    public SpikeTrap spikeTrap;
    public Spike spike;
    public JumpPad jumpPad;
    public Fire fire;
    public Crush crush;
	public OneWayGate gate;
	public Slice slice;
    public Warp warp;
    public Spikeball spikeball;
    public NoteBox noteBox;
    public LockAndKey lockAndKey;
    public Laser laser;
    public Bubble bubble;
    public Electric electric;

    [System.Serializable]
	public class Bubble
	{
         public AudioEntry enter;
        public AudioEntry pop;
    }

    [System.Serializable]
	public class FlagPole
	{
        public AudioEntry wiggle;
        public AudioEntry flagDown;
        public AudioEntry flagUp;
        public AudioEntry backflip;
        public AudioEntry fanfare;
    }

    [System.Serializable]
	public class Door
	{
        public AudioEntry open;
        public AudioEntry close;
    }

    [System.Serializable]
	public class Cannon
	{
        public AudioEntry fire;
    }

    [System.Serializable]
	public class JumpPad
	{
        public AudioEntry jump;
    }

    [System.Serializable]
    public class BreakableBlock
    {
        public AudioEntry smash;
    }

    [System.Serializable]
    public class Saw
    {
        public AudioEntry cut;
    }

    [System.Serializable]
    public class LogicSwitch
    {
        public AudioEntry beep;
        public AudioEntry create;
        public AudioEntry attach;
        public AudioEntry detach;
    }

    [System.Serializable]
    public class SpikeTrap
    {
        public AudioEntry trigger;
    }

    [System.Serializable]
    public class Spike
    {
        public AudioEntry impale;
    }

    [System.Serializable]
    public class Fire
    {
        public AudioEntry burn;
    }

    [System.Serializable]
    public class Crush
    {
        public AudioEntry crush;
    }

	[System.Serializable]
    public class Slice
    {
        public AudioEntry slice;
    }

	[System.Serializable]
    public class OneWayGate
    {
        public AudioEntry open;
		public AudioEntry close;
    }

    [System.Serializable]
    public class Warp
    {
        public AudioEntry enter;
		public AudioEntry exit;
        public AudioEntry travel;
    }

    [System.Serializable]
    public class Spikeball
    {
        public AudioEntry splat;
    }

    [System.Serializable]
    public class NoteBox
    {
        public AudioEntry sfx;
        public AudioEntry octave_high;
        public AudioEntry octave_medium;
        public AudioEntry octave_low;
    }

    [System.Serializable]
    public class LockAndKey
    {
        public AudioEntry unlock;
    }

    [System.Serializable]
    public class Laser
    {
        public AudioEntry warmup;
        public AudioEntry beam;
    }

    [System.Serializable]
    public class Electric
    {
        public AudioEntry zap;
    }
}

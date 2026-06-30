using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISFXPool : ScriptableObject
{
    public AudioEntry tallButtonDown;
    public AudioEntry tallButtonUp;
    public AudioEntry flatButton;
    public AudioEntry tabChange;
    public AudioEntry deathEffect;
    public AudioEntry levelEntityScaling;
    public AudioEntry musicSwitch;
	public AudioEntry dialogBoxOpen;
	public AudioEntry dialogBoxClose;
	public AudioEntry ready;
    public SlotMachine slotMachine;

    [System.Serializable]
    public class SlotMachine
    {
        public AudioEntry leverDown;
        public AudioEntry leverUp;
        public AudioEntry win;
        public AudioEntry loose;
        public AudioEntry spinnTick;
    }
}

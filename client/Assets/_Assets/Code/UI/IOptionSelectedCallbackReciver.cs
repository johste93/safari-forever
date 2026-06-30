using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IOptionSelectedCallbackReciver
{
    void SelectOption(int optionIndex);

    void RegisterOnOptionSelectedListener(UnityAction<int> optionSelected);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISuspendable
{
    void Suspend(bool suspend);

    void On_SuspensionEvent(bool suspend);
}

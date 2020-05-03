using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : ScriptableObject
{
    protected bool complete = false;

    public abstract void Complete();
}

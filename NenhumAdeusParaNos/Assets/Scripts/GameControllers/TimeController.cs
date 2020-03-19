﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float slowdownFactor = 0.1f;

    public void StartSlowdown()
    {
        Time.timeScale = slowdownFactor;
    }

    public void EndSlowdown()
    {
        Time.timeScale = 1;
    }
}

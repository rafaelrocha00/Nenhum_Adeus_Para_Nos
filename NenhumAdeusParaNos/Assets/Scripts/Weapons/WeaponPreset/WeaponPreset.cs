using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPreset : MonoBehaviour
{
    public Collider col;
    public GameObject model;

    public void Enable(bool v)
    {
        model.SetActive(v);
        gameObject.SetActive(v);
    }
}

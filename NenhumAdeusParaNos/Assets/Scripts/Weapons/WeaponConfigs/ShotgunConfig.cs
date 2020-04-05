using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shotgun", menuName = "Weapons/Shotgun")]
public class ShotgunConfig : NormalRangedConfig
{
    public float shootAngle;
    [SerializeField] int shootQuant = 1;
    public int ShootQuantEachSide { get { return shootQuant; } set { if (value < 1) shootQuant = 1; else shootQuant = value; } }
}

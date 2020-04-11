using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Item/Weapon Item")]
public class WeaponItem : Item
{
    public WeaponConfig thisWeapon;//Precisa ter uma mesh, e um alcance pras armas melee q vai mudar o tamanho do colisor da arma.
    //public GameObject weaponPref;//Prefab da arma para substituir a do player, se for melee prefab é melhor já que alem de substituir o modelo, vai tambem mudar junto colisor e ja vai vim com a configuração da arma certa

}

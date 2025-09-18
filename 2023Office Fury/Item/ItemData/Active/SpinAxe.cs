using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpinAxe", menuName = "ScriptableObjects/Active Item/SpinAxe Item")]
public class SpinAxe : BaseItem
{
    public float Damage;
    public float item_Duration;
    public GameObject Axe;
}
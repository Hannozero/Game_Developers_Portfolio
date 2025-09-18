using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FireBottle", menuName = "ScriptableObjects/Active Item/Fire Bottle Item")]
public class FireBottle : BaseItem
{
    public float Damage;
    public float item_Duration;
    public GameObject Bottle;
}


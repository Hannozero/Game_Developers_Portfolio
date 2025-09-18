using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour
{
    public GameObject reloadUIObj;
    public Slider reloadSilder;
    GameObject player;
    float maxReloadTime;


    public float RuntimeCheck;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        maxReloadTime = player.GetComponent<FireCtrl>().reloadTime;
        
    }


    
    void Update()
    {
        if (player.GetComponent<FireCtrl>().isReload == true)
        {
            reloadUIObj.SetActive(true);
            RuntimeCheck += Time.deltaTime;
            reloadSilder.value = RuntimeCheck / maxReloadTime;
            if (RuntimeCheck >= maxReloadTime) {
                UIOff();
            }
        }
    }

    void UIOff() {
        reloadUIObj.SetActive(false);
    }
}

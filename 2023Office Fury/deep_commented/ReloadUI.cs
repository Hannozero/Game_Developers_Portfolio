// ====== Deep Commented Version (ReloadUI.cs) — 함수/라인 설명 강화 ======

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
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        maxReloadTime = player.GetComponent<FireCtrl>().reloadTime;
        
    }


    
    void Update()
    {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        if (player.GetComponent<FireCtrl>().isReload == true)
        {
            reloadUIObj.SetActive(true);
// 프레임 독립 보정(deltaTime)
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
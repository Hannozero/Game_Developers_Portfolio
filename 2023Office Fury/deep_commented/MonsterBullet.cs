// ====== Deep Commented Version (MonsterBullet.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
// 사운드 재생/효과음 트리거
using FMODPlus;

public class MonsterBullet : MonoBehaviour
{
    
// 데미지 계산/적용 — 방어/저항 고려 지점
    float Damage = 1;
// 사운드 재생/효과음 트리거
    [SerializeField] FMODAudioSource hit;
    [SerializeField] MeshRenderer sim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //────────────────────────────────────────────────────────────────────

    // OnTriggerStay : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    private void OnTriggerStay(Collider other)
    {
        if(!hit.isPlaying)
            hit.Play();
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        sim.GetComponent<MeshRenderer>().enabled = false;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        this.GetComponent<SphereCollider>().enabled = false;
// 오브젝트 파괴/정리 예약
        Destroy(gameObject,1.15f);
    }
}
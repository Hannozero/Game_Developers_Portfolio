// ====== Deep Commented Version (FlyingMob.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMob : BaseMonster
{
// 스폰(생성) 로직 — 수량/간격 제어
    public GameObject spawnMonster;
    public Transform body;

    //────────────────────────────────────────────────────────────────────

    // Awake : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    //────────────────────────────────────────────────────────────────────
    // Start : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame

    float deathSpeed = 1f;
    public Transform deathTarget;
    //────────────────────────────────────────────────────────────────────
    // Update : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    protected override void Update()
    {
        base.Update();

        if (isDead == true) {
            isMoving = false;
// 프레임 독립 보정(deltaTime)
            body.position = Vector3.MoveTowards(body.position, deathTarget.position, deathSpeed * Time.deltaTime);
            anima.SetInteger("Death", 1);
        }
    }

    //플레이어와 충돌하면 그 즉시 자살하고 몬스터생성
    //이란 기능이 있었으나 그냥 충돌하면 데미지주고 삭제
    //────────────────────────────────────────────────────────────────────
    // OnTriggerEnter : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
// 트리거 진입 콜백 — 충돌체가 IsTrigger일 때
    protected override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);

        if (other.gameObject.tag == "Player") {
            //Instantiate(spawnMonster, monster.transform.position, monster.transform.rotation);
            S_Manager.m_flyCount--;
            anima.SetInteger("Death", 2);
            isDead = true;
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
            hp = 0;
        }
    }
}
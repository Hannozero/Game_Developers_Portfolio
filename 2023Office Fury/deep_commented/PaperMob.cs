// ====== Deep Commented Version (PaperMob.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperMob : BaseMonster
{
    public int moveSet;
    public int DeathSet;
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
// 난수 — 범위 내 무작위값
        moveSet = UnityEngine.Random.Range(0, 3);
        Debug.Log(moveSet);
        anima.SetInteger("Move", moveSet);
    }

    // Update is called once per frame
    //────────────────────────────────────────────────────────────────────
    // Update : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    protected override void Update()
    {
        base.Update();

        if (isDead == true) {
// 난수 — 범위 내 무작위값
            DeathSet = UnityEngine.Random.Range(0, 2);
            anima.SetInteger("Death", DeathSet);
        }
    }
}
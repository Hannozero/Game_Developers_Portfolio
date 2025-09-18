// ====== Deep Commented Version (MonsterAI.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    GameObject player;  //플레이어 오브젝트
// NavMeshAgent 사용 — 내비게이션 기반 경로 추적/이동
    NavMeshAgent nav;   //네비메쉬

    public bool isMoving = true;

    //────────────────────────────────────────────────────────────────────

    // Awake : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

// NavMeshAgent 사용 — 내비게이션 기반 경로 추적/이동
        nav=GetComponent<NavMeshAgent>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
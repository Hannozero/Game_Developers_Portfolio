// ====== Deep Commented Version (MonsterTest.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class MonsterTest : MonoBehaviour
{
    private Transform player;  // 플레이어의 Transform
// NavMeshAgent 사용 — 내비게이션 기반 경로 추적/이동
    private NavMeshAgent agent;  // NavMesh 에이전트

// 데미지 계산/적용 — 방어/저항 고려 지점
    public float damage = 10f;
    public float speed = 1.0f;
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
    public float hp = 100f;

    //────────────────────────────────────────────────────────────────────

    // Setup : 함수 개요

    //  - 목적: 초기화/설정

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    public void Setup(float newHp, float newDamage, float newSpeed)
    {
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
        hp = newHp;
// 데미지 계산/적용 — 방어/저항 고려 지점
        damage = newDamage;
        agent.speed = newSpeed;
        //텍스처나 머테리얼을 가져와서 바꿀 수 있음.
    }

    //────────────────────────────────────────────────────────────────────

    // Awake : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        // 플레이어의 Transform 찾기 (플레이어 오브젝트의 태그를 "Player"로 설정해야 함)
        player = GameObject.FindGameObjectWithTag("Player").transform;
// NavMeshAgent 사용 — 내비게이션 기반 경로 추적/이동
        agent = GetComponent<NavMeshAgent>();// NavMesh 에이전트 설정
    }

    void Start()
    {

        if (agent != null)
        {
            if (player != null)
            {
                // NavMeshAgent에 목적지 설정
// 목표 지점 설정 — NavMesh 경로 갱신
                agent.SetDestination(player.position);
            }
            else
            {
                Debug.LogError("목표 위치 (target)가 설정되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogError("NavMeshAgent가 이 객체에 추가되지 않았거나 비활성화되어 있습니다.");
        }
    }

    void Update()
    {
        // 플레이어를 계속 추적
        if (player != null)
        {
// 목표 지점 설정 — NavMesh 경로 갱신
            agent.SetDestination(player.position);
        }
    }
}
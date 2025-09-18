// ====== Deep Commented Version (RangeMob.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
// 사운드 재생/효과음 트리거
using FMODPlus;

public class RangeMob : BaseMonster
{
    float distance;
    public float detect;

    public int moveSet;
    public bool isAttack;
    public bool isShoot;
    public GameObject Bullet;
    public Transform firepos;

    [SerializeField] ParticleSystem Shootvfx;
    [SerializeField] ParticleSystem firevfx;
// 사운드 재생/효과음 트리거
    [SerializeField] FMODAudioSource Shoot;
// 사운드 재생/효과음 트리거
    [SerializeField] FMODAudioSource Ready;

    float targetRadius = 0.5f;
    float targetRange = 10f;
    //────────────────────────────────────────────────────────────────────
    // Awake : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    protected override void Awake()
    {
        base.Awake();

    }

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
    }

    //────────────────────────────────────────────────────────────────────

    // Update : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    protected override void Update()
    {
        base.Update();

// 두 점 사이 거리 계산
        distance = Vector3.Distance(player.transform.position, monster.transform.position);

        if (isDead == false)
        {
            if (distance <= detect)
            {
                isAttack = true;
                nav.isStopped = true;
                anima.SetBool("isReady", true);
                if(!Ready.isPlaying)
                    Ready.Play();
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                nav.SetDestination(target.GetComponent<Transform>().position);

                // 에이전트가 항상 플레이어를 바라보게 합니다.
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                Vector3 direction = (target.GetComponent<Transform>().position - firepos.position).normalized;
                if (direction != Vector3.zero)
                {
                    // LookRotation을 사용하여 에이전트가 플레이어 방향으로 회전합니다.
// 방향 벡터를 회전값으로 변환
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
// 프레임 독립 보정(deltaTime)
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * nav.angularSpeed);
                }
                if (!isShoot)
                {
                    anima.SetBool("isShoot", true);
                    Invoke("ShootBullet", 0.3f);
                    isShoot = true;
                }
            }
            else
            {
                nav.isStopped = false;
                isAttack = false;
                anima.SetBool("isReady", false);
                anima.SetBool("isShoot", false);
            }
        }
    }

    void ShootBullet()
    {
        Shoot.Play();
// 프리팹 인스턴스 생성
        GameObject instantBullet = Instantiate(Bullet, firepos.position, transform.rotation);
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
        rigidBullet.velocity = transform.forward * 6;
// 쿨타임 관리 — 재사용 제한
        Invoke("CoolTime", 2.5f);
        anima.SetBool("isShoot", false);
    }

// 쿨타임 관리 — 재사용 제한
    void CoolTime()
    {
        isShoot = false;
    }
}
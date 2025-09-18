// ====== Deep Commented Version (SafeMob.cs) — 함수/라인 설명 강화 ======

// 사운드 재생/효과음 트리거
using FMODPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SafeMob : BaseMonster
{
    float distance;
    public float detect;

    public float deleteTime;

    public float jumpSpeed;
// 사운드 재생/효과음 트리거
    public FMODAudioSource jump;

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

        isMoving = false;
        Invoke("DeleteRun",deleteTime);
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

        //플레이어와의 거리
// 두 점 사이 거리 계산
        distance = Vector3.Distance(player.transform.position, monster.transform.position);


        if (isDead == false)
        {
            //플레이어가 거리내에 있으면 플레이어반대방향으로 도망감
            if (distance <= detect)
            {
                Debug.Log("감지!");
                anima.SetBool("Jump", true);
                jump.Play();
                nav.speed = jumpSpeed;
                destination = new Vector3(transform.position.x - player.transform.position.x, 0f, transform.position.z - player.transform.position.z).normalized;
                nav.SetDestination(transform.position + destination * jumpSpeed);
            }
            else
            {
                anima.SetBool("Jump", false);
                jump.Play();
                isMoving = true;
                nav.speed = 1f;
            }
        }
    }



    void DeleteRun() {
        S_Manager.m_safeCount--;
        
        isDead = true;
    }
}
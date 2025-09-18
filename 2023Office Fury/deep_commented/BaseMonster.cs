// ====== Deep Commented Version (BaseMonster.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// 사운드 재생/효과음 트리거
using FMODPlus;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class BaseMonster : MonoBehaviour
{

// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
    public float hp;
    public float speed;

// 무적 프레임 — 연속 피격 방지
    public bool invincibility = false;
// 무적 프레임 — 연속 피격 방지
    public float invincibilityTime = 0.5f;

// 사운드 재생/효과음 트리거
    [SerializeField]FMODAudioSource[] source;
    //0번 moen , 1번 burn, 2번 hit, 3번 walk, 4번 dead

    public GameObject player;
    public GameObject target;
    public GameObject fire;
    public GameObject monster;
// NavMeshAgent 사용 — 내비게이션 기반 경로 추적/이동
    public NavMeshAgent nav;

    public Animator anima;

    [SerializeField] ParticleSystem firefx;
    [SerializeField] ParticleSystem debuffx;
    [SerializeField] ParticleSystem fragment;
    [SerializeField] float BurnTime = 5f;
    float CurTime;
    float DotTime = 0;
    [SerializeField] bool isBurn;

    public Renderer monsterColor;

// 데미지 계산/적용 — 방어/저항 고려 지점
    public float damage;

    public bool isDead = false;
    bool isDeads = false;
    public float deathTime;
    public int dropCoin;

    public bool isMoving = true;

    public Vector3 destination;

    public GameObject scoreManager;
    public int monsterType;
    public int addScore;


    //────────────────────────────────────────────────────────────────────


    // Awake : 함수 개요


    //  - 목적: 주요 기능 처리


    //  - 주의: NRE 방지(널 체크) 및 상태 동기화


    //────────────────────────────────────────────────────────────────────


    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player;
        fire = GameObject.FindGameObjectWithTag("Fire");
        scoreManager = GameObject.Find("ScoreManager");

        //anima = GetComponent<Animator>();

// NavMeshAgent 사용 — 내비게이션 기반 경로 추적/이동
        nav = GetComponent<NavMeshAgent>();

        //monsterColor = gameObject.GetComponent<Renderer>();
        isBurn = false;
        DotTime = 0;
    }

    // Start is called before the first frame update
    //────────────────────────────────────────────────────────────────────
    // Start : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    protected virtual void Start()
    {
        nav.speed = speed;
    }

    protected virtual void BattleCryDestination()  // 다음 행동 준비
    {
        destination = new Vector3(transform.position.x - player.transform.position.x,
            0f, transform.position.z - player.transform.position.z).normalized;
    }

    // Update is called once per frame
    //────────────────────────────────────────────────────────────────────
    // Update : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    protected virtual void Update()
    {

        if (isMoving == true)
        {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            if (player.GetComponent<PlayerItem>().isbattlecry)
            {
                BattleCryDestination();
                if (!debuffx.isPlaying)
                    debuffx.Play();
                nav.SetDestination(transform.position + destination * nav.speed);
            }
            else
            {
                debuffx.Stop();
                nav.SetDestination(target.transform.position);
            }

// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            if(player.GetComponent<NewProjectile>().PipeThrowing)
                target =  GameObject.FindGameObjectWithTag("Pipe");
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            else if(!player.GetComponent<NewProjectile>().PipeThrowing)
                target = GameObject.FindGameObjectWithTag("Player");
        }

        //사망상태 확인
        if (isDead == false)
        {

// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
            if (hp <= 0)
            {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                if (player.GetComponent<BasePlayer>().activeGauge < player.GetComponent<BasePlayer>().maxActiveGauge)
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                    player.GetComponent<BasePlayer>().activeGauge += 5;
                else
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                    player.GetComponent<BasePlayer>().activeGauge = player.GetComponent<BasePlayer>().maxActiveGauge;

                isDead = true;
                isDeads = true;
                isMoving = false;
                nav.speed = 0;
            }
        }
        else if (isDead == true)
        {
            DeadSound();
            anima.SetBool("Dead", true);
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            this.GetComponent<Collider>().enabled = false;
            Invoke("Dead", deathTime);
        }

        if (isBurn)
            BurnDamage();
        else
            CurTime = 0;


    }



    //────────────────────────────────────────────────────────────────────



    // DeadSound : 함수 개요



    //  - 목적: 사망 처리/정리



    //  - 주의: NRE 방지(널 체크) 및 상태 동기화



    //────────────────────────────────────────────────────────────────────



    private void DeadSound()
    {
        source[0].Stop();
        source[3].Stop();
        if (isDeads)
        {
            source[4].Play();
            isDeads = false;
        }
    }

    //────────────────────────────────────────────────────────────────────

    // BurnDamage : 함수 개요

    //  - 목적: 피해 적용/이펙트

    //  - 주의: 중복 타격/무적 프레임 고려

    //────────────────────────────────────────────────────────────────────

    private void BurnDamage()
    {
        if (!source[1].isPlaying)
            source[1].Play();
// 프레임 독립 보정(deltaTime)
        CurTime += Time.deltaTime;
// 프레임 독립 보정(deltaTime)
        DotTime += Time.deltaTime;
        if (DotTime > 0.5)
        {
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
            hp -= 1;
            DotTime = 0;
        }    
    }

    //죽으면 실행되는 함수
    //────────────────────────────────────────────────────────────────────
    // Dead : 함수 개요
    //  - 목적: 사망 처리/정리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    protected virtual void Dead() {
        S_Manager.GameMonsterCount--;
        Debug.Log("죽음");
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        player.GetComponent<BasePlayer>().coin += dropCoin;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        scoreManager.GetComponent<Score>().score += addScore;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        scoreManager.GetComponent<Score>().monsterCount[monsterType]++;
        
// 오브젝트 파괴/정리 예약
        Destroy(monster);
    }

// 오브젝트 파괴/정리 예약
    void Destroy() {
        
    }


    protected virtual void InvincibilityClear()   //무적해제
    {
// 무적 프레임 — 연속 피격 방지
        invincibility = false;
    }


    //────────────────────────────────────────────────────────────────────


    // OnTriggerEnter : 함수 개요


    //  - 목적: 주요 기능 처리


    //  - 주의: NRE 방지(널 체크) 및 상태 동기화


    //────────────────────────────────────────────────────────────────────


// 트리거 진입 콜백 — 충돌체가 IsTrigger일 때
    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }


    //────────────────────────────────────────────────────────────────────


    // OnTriggerStay : 함수 개요


    //  - 목적: 주요 기능 처리


    //  - 주의: NRE 방지(널 체크) 및 상태 동기화


    //────────────────────────────────────────────────────────────────────


    protected virtual void OnTriggerStay(Collider other) {

        if (other.gameObject.tag == "Fire")
        {
// 무적 프레임 — 연속 피격 방지
            if (invincibility == false)
            {
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
                Debug.Log("Hp" + hp);
                firefx.Play();
                fragment.Play();
                source[2].Play();
                isBurn = true;
// 무적 프레임 — 연속 피격 방지
                invincibility = true;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                StartCoroutine(GetComponent<dissolve>().HitColorChange(0.1f,0.1f));
                monsterColor.material.color = Color.white;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                hp -= player.GetComponent<BasePlayer>().fireDamage;
// 무적 프레임 — 연속 피격 방지
                Invoke("InvincibilityClear", invincibilityTime);
                Invoke("BurnEffect", BurnTime);        
            }
            
        }

        if (other.gameObject.tag == "Axe")
        {
// 무적 프레임 — 연속 피격 방지
            if (invincibility == false)
            {
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
                Debug.Log("Hp" + hp);
// 무적 프레임 — 연속 피격 방지
                invincibility = true;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                hp -= player.GetComponent<BasePlayer>().axeDamage;
// 무적 프레임 — 연속 피격 방지
                Invoke("InvincibilityClear", invincibilityTime);

            }
        }
    }

    //────────────────────────────────────────────────────────────────────

    // HitExplosion : 함수 개요

    //  - 목적: 피해 적용/이펙트

    //  - 주의: 중복 타격/무적 프레임 고려

    //────────────────────────────────────────────────────────────────────

// 폭발 로직 — 범위 판정 및 넉백/피해
    public virtual void HitExplosion(Vector3 explosionPos)
    {
// 무적 프레임 — 연속 피격 방지
        if (invincibility == false)
        {
            Debug.Log("FireBottleHit");
            firefx.Play();
            isBurn = true;
// 무적 프레임 — 연속 피격 방지
            invincibility = true;
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
            hp -= 100f;
// 무적 프레임 — 연속 피격 방지
            Invoke("InvincibilityClear", invincibilityTime);
            Invoke("BurnEffect", BurnTime);
        }
    }

    //────────────────────────────────────────────────────────────────────

    // PipeExplosion : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

// 폭발 로직 — 범위 판정 및 넉백/피해
    public virtual void PipeExplosion(Vector3 explosionPos)
    {
// 무적 프레임 — 연속 피격 방지
        if (invincibility == false)
        {
            Debug.Log("FireBottleHit");
            isBurn = true;
            firefx.Play();
// 무적 프레임 — 연속 피격 방지
            invincibility = true;
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
            hp -= 50f;
            //StartCoroutine(HitColorChange());
// 무적 프레임 — 연속 피격 방지
            Invoke("InvincibilityClear", invincibilityTime);
        }
    }

    //────────────────────────────────────────────────────────────────────

    // GasExplosion : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

// 폭발 로직 — 범위 판정 및 넉백/피해
    public virtual void GasExplosion(Vector3 explosionPos)
    {
// 무적 프레임 — 연속 피격 방지
        if (invincibility == false)
        {
            Debug.Log("FireBottleHit");
            isBurn = true;
            firefx.Play();
// 무적 프레임 — 연속 피격 방지
            invincibility = true;
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
            hp -= 100f;
// 무적 프레임 — 연속 피격 방지
            Invoke("InvincibilityClear", invincibilityTime);
        }
    }

    //────────────────────────────────────────────────────────────────────

    // BurnHit : 함수 개요

    //  - 목적: 피해 적용/이펙트

    //  - 주의: 중복 타격/무적 프레임 고려

    //────────────────────────────────────────────────────────────────────

// 폭발 로직 — 범위 판정 및 넉백/피해
    public virtual void BurnHit(Vector3 explosionPos)
    {
// 무적 프레임 — 연속 피격 방지
        if (invincibility == false)
        {
            Debug.Log("FireBottleHit");
            firefx.Play();
            isBurn = true;
// 무적 프레임 — 연속 피격 방지
            invincibility = true;
            BurnDamage();
// 무적 프레임 — 연속 피격 방지
            Invoke("InvincibilityClear", invincibilityTime);
            Invoke("BurnEffect", BurnTime);
        }
    }

    //────────────────────────────────────────────────────────────────────

    // BurnEffect : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    protected virtual void BurnEffect()
    {
        firefx.Stop();
        fragment.Stop();
        isBurn = false;
    }
    //────────────────────────────────────────────────────────────────────
    // OnhitDamage : 함수 개요
    //  - 목적: 피해 적용/이펙트
    //  - 주의: 중복 타격/무적 프레임 고려
    //────────────────────────────────────────────────────────────────────
    public void OnhitDamage()
    {
// 무적 프레임 — 연속 피격 방지
        if (invincibility == false)
        {
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
            Debug.Log("Hp" + hp);
            firefx.Play();
            fragment.Play();
            isBurn = true;
// 무적 프레임 — 연속 피격 방지
            invincibility = true;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            StartCoroutine(GetComponent<dissolve>().HitColorChange(0.6f, 0.2f));
            //monsterColor.material.color = Color.white;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            hp -= player.GetComponent<BasePlayer>().fireDamage;
// 무적 프레임 — 연속 피격 방지
            Invoke("InvincibilityClear", invincibilityTime);
            Invoke("BurnEffect", 5f);
        }
    }

    //────────────────────────────────────────────────────────────────────

    // OnParticleTrigger : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    private void OnParticleTrigger()
    {
        
    }

    //────────────────────────────────────────────────────────────────────

    // OnTriggerExit : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    protected virtual void OnTriggerExit(Collider other)
    {

    }
}
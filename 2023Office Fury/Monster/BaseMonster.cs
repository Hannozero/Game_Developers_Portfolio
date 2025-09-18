using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODPlus;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class BaseMonster : MonoBehaviour
{

    public float hp;
    public float speed;

    public bool invincibility = false;
    public float invincibilityTime = 0.5f;

    [SerializeField]FMODAudioSource[] source;
    //0번 moen , 1번 burn, 2번 hit, 3번 walk, 4번 dead

    public GameObject player;
    public GameObject target;
    public GameObject fire;
    public GameObject monster;
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


    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player;
        fire = GameObject.FindGameObjectWithTag("Fire");
        scoreManager = GameObject.Find("ScoreManager");

        //anima = GetComponent<Animator>();

        nav = GetComponent<NavMeshAgent>();

        //monsterColor = gameObject.GetComponent<Renderer>();
        isBurn = false;
        DotTime = 0;
    }

    // Start is called before the first frame update
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
    protected virtual void Update()
    {

        if (isMoving == true)
        {
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

            if(player.GetComponent<NewProjectile>().PipeThrowing)
                target =  GameObject.FindGameObjectWithTag("Pipe");
            else if(!player.GetComponent<NewProjectile>().PipeThrowing)
                target = GameObject.FindGameObjectWithTag("Player");
        }

        //사망상태 확인
        if (isDead == false)
        {

            if (hp <= 0)
            {
                if (player.GetComponent<BasePlayer>().activeGauge < player.GetComponent<BasePlayer>().maxActiveGauge)
                    player.GetComponent<BasePlayer>().activeGauge += 5;
                else
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
            this.GetComponent<Collider>().enabled = false;
            Invoke("Dead", deathTime);
        }

        if (isBurn)
            BurnDamage();
        else
            CurTime = 0;


    }



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

    private void BurnDamage()
    {
        if (!source[1].isPlaying)
            source[1].Play();
        CurTime += Time.deltaTime;
        DotTime += Time.deltaTime;
        if (DotTime > 0.5)
        {
            hp -= 1;
            DotTime = 0;
        }    
    }

    //죽으면 실행되는 함수
    protected virtual void Dead() {
        S_Manager.GameMonsterCount--;
        Debug.Log("죽음");
        player.GetComponent<BasePlayer>().coin += dropCoin;
        scoreManager.GetComponent<Score>().score += addScore;
        scoreManager.GetComponent<Score>().monsterCount[monsterType]++;
        
        Destroy(monster);
    }

    void Destroy() {
        
    }


    protected virtual void InvincibilityClear()   //무적해제
    {
        invincibility = false;
    }


    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }


    protected virtual void OnTriggerStay(Collider other) {

        if (other.gameObject.tag == "Fire")
        {
            if (invincibility == false)
            {
                Debug.Log("Hp" + hp);
                firefx.Play();
                fragment.Play();
                source[2].Play();
                isBurn = true;
                invincibility = true;
                StartCoroutine(GetComponent<dissolve>().HitColorChange(0.1f,0.1f));
                monsterColor.material.color = Color.white;
                hp -= player.GetComponent<BasePlayer>().fireDamage;
                Invoke("InvincibilityClear", invincibilityTime);
                Invoke("BurnEffect", BurnTime);        
            }
            
        }

        if (other.gameObject.tag == "Axe")
        {
            if (invincibility == false)
            {
                Debug.Log("Hp" + hp);
                invincibility = true;
                hp -= player.GetComponent<BasePlayer>().axeDamage;
                Invoke("InvincibilityClear", invincibilityTime);

            }
        }
    }

    public virtual void HitExplosion(Vector3 explosionPos)
    {
        if (invincibility == false)
        {
            Debug.Log("FireBottleHit");
            firefx.Play();
            isBurn = true;
            invincibility = true;
            hp -= 100f;
            Invoke("InvincibilityClear", invincibilityTime);
            Invoke("BurnEffect", BurnTime);
        }
    }

    public virtual void PipeExplosion(Vector3 explosionPos)
    {
        if (invincibility == false)
        {
            Debug.Log("FireBottleHit");
            isBurn = true;
            firefx.Play();
            invincibility = true;
            hp -= 50f;
            //StartCoroutine(HitColorChange());
            Invoke("InvincibilityClear", invincibilityTime);
        }
    }

    public virtual void GasExplosion(Vector3 explosionPos)
    {
        if (invincibility == false)
        {
            Debug.Log("FireBottleHit");
            isBurn = true;
            firefx.Play();
            invincibility = true;
            hp -= 100f;
            Invoke("InvincibilityClear", invincibilityTime);
        }
    }

    public virtual void BurnHit(Vector3 explosionPos)
    {
        if (invincibility == false)
        {
            Debug.Log("FireBottleHit");
            firefx.Play();
            isBurn = true;
            invincibility = true;
            BurnDamage();
            Invoke("InvincibilityClear", invincibilityTime);
            Invoke("BurnEffect", BurnTime);
        }
    }

    protected virtual void BurnEffect()
    {
        firefx.Stop();
        fragment.Stop();
        isBurn = false;
    }
    public void OnhitDamage()
    {
        if (invincibility == false)
        {
            Debug.Log("Hp" + hp);
            firefx.Play();
            fragment.Play();
            isBurn = true;
            invincibility = true;
            StartCoroutine(GetComponent<dissolve>().HitColorChange(0.6f, 0.2f));
            //monsterColor.material.color = Color.white;
            hp -= player.GetComponent<BasePlayer>().fireDamage;
            Invoke("InvincibilityClear", invincibilityTime);
            Invoke("BurnEffect", 5f);
        }
    }

    private void OnParticleTrigger()
    {
        
    }

    protected virtual void OnTriggerExit(Collider other)
    {

    }
}

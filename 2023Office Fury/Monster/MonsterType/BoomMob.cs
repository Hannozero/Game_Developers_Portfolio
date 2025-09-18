using FMODPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomMob : BaseMonster
{
    [SerializeField] ParticleSystem explosion;
    float distance;
    public float detect;

    public float boomTime;
    public float boomRange;


    [SerializeField] FMODAudioSource explosionsfx;

    public bool boomCheck = false;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //플레이어와의 거리
        distance = Vector3.Distance(player.transform.position, monster.transform.position);

        //플레이어가 범위내에 들어왔는지 체크, 범위내에 들오면 자폭실행
        if (distance <= detect)
        {
            if (boomCheck == false)
            {
                Debug.Log("감지!");
                isMoving = false;
                anima.SetBool("Boom", true);
                Invoke("Boom", boomTime);
                boomCheck = true;
            }
        }
    }

    private void ExplosionEffect()
    {
        explosion.Play();
    }

    private void ExplosionSfx()
    {
        FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
        cam.IsShake = true;
        explosionsfx.Play();
    }

    protected override void Dead()
    {
        this.GetComponent<Collider>().enabled = false;
        ExplosionEffect();
        S_Manager.GameMonsterCount--;
        Debug.Log("죽음");
        
        player.GetComponent<BasePlayer>().coin += dropCoin;
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Dead();
    }

    //폭발범위내에 있으면 데미지
    void Boom() {
        ExplosionEffect();
        ExplosionSfx();
        if (distance <= boomRange) {
            player.GetComponent<BasePlayer>().hp = player.GetComponent<BasePlayer>().hp - damage;
        }
        S_Manager.m_compCount--;
        Dead();
    }
}

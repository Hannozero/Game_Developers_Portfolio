using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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
    [SerializeField] FMODAudioSource Shoot;
    [SerializeField] FMODAudioSource Ready;

    float targetRadius = 0.5f;
    float targetRange = 10f;
    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();
        moveSet = UnityEngine.Random.Range(0, 3);
    }

    protected override void Update()
    {
        base.Update();

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
                nav.SetDestination(target.GetComponent<Transform>().position);

                // 에이전트가 항상 플레이어를 바라보게 합니다.
                Vector3 direction = (target.GetComponent<Transform>().position - firepos.position).normalized;
                if (direction != Vector3.zero)
                {
                    // LookRotation을 사용하여 에이전트가 플레이어 방향으로 회전합니다.
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
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
        GameObject instantBullet = Instantiate(Bullet, firepos.position, transform.rotation);
        Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
        rigidBullet.velocity = transform.forward * 6;
        Invoke("CoolTime", 2.5f);
        anima.SetBool("isShoot", false);
    }

    void CoolTime()
    {
        isShoot = false;
    }
}

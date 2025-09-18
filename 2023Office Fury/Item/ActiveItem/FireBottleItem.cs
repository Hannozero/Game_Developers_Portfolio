using FMODPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireBottleItem : MonoBehaviour
{
    [SerializeField] float BotSize = 2f;
    [SerializeField] float time = 5f;
    [SerializeField] float curtime = 0;
    [SerializeField] bool brokebtl = false;
    bool isboom = false;
    [SerializeField] ParticleSystem trail;
    [SerializeField] ParticleSystem Boom;
    [SerializeField] GameObject Fire;
    [SerializeField] FMODAudioSource BoomSound;
    [SerializeField] Vector3 Area;
    [SerializeField] Renderer model;
    [SerializeField] bool isShake;


    Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddTorque(transform.up*5,ForceMode.Force);
        rigid.AddTorque(transform.right * 5, ForceMode.Force);

        trail.Play();
        Area = new Vector3(2f, 0, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (brokebtl)
        {
            curtime += Time.deltaTime;
            if(curtime < time) { }
            {
                RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, BotSize, Vector3.up, 0, LayerMask.GetMask("Monster"));
                foreach (RaycastHit hitObj in rayHits)
                {
                    hitObj.transform.GetComponent<BaseMonster>().BurnHit(transform.position);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.tag == "ActiveFloor")
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Area.y = this.gameObject.transform.position.y;
            BoomSound.Play();
            Boom.Play();
            FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
            cam.IsShake = true;
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, BotSize, Vector3.up, 0, LayerMask.GetMask("Monster"));
            foreach (RaycastHit hitObj in rayHits)
            {
                hitObj.transform.GetComponent<BaseMonster>().HitExplosion(transform.position);
            }
            trail.Stop();
            RandomFire();
            model.enabled = false;
            brokebtl = true;
            isboom = true;
            Destroy(this.gameObject, time);
        }
    }

    private void RandomFire()
    {
        
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCirclePoint = Random.insideUnitCircle * BotSize;

            // 스폰 위치를 설정합니다. Y 축은 0으로 설정합니다.
            Vector3 spawnPosition = new Vector3(
                transform.position.x + randomCirclePoint.x,
                0f,
                transform.position.z + randomCirclePoint.y
            );

            Debug.Log(spawnPosition);
            Destroy(Instantiate(Fire, spawnPosition, Quaternion.identity), 5f);
        }
    }
}

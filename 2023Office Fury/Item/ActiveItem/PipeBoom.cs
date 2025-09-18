using FMODPlus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PipeBoom : MonoBehaviour
{
    [SerializeField] float CurTime;
    [SerializeField] float BoomTime = 10f;
    [SerializeField] float BotSize = 2f;
    [SerializeField] bool isDrop;
    [SerializeField] bool isBoom;
    [SerializeField] ParticleSystem Boom;
    [SerializeField] GameObject Fire;
    [SerializeField] FMODAudioSource BoomSound;
    [SerializeField] Vector3 Area;
    [SerializeField] Renderer model;
    Rigidbody rigid;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        CurTime = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        rigid = GetComponent<Rigidbody>();
        rigid.AddTorque(transform.up * 5, ForceMode.Force);
        rigid.AddTorque(transform.right * 5, ForceMode.Force);

        Area = new Vector3(2f, 0, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrop)
        {
            CurTime += Time.deltaTime;
            if (CurTime >= BoomTime)
            {
                if (!isBoom)
                {
                    BoomSound.Play();
                    Boom.Play();
                    FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
                    cam.IsShake = true;
                    RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, BotSize, Vector3.up, 0, LayerMask.GetMask("Monster"));
                    foreach (RaycastHit hitObj in rayHits)
                    {
                        hitObj.transform.GetComponent<BaseMonster>().PipeExplosion(transform.position);
                    }
                    RandomFire();
                    model.GetComponent<MeshRenderer>().enabled = false;
                    player.GetComponent<NewProjectile>().PipeThrowing = false;

                    isBoom = true;
                    Destroy(this.gameObject, 5f);
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "ActiveFloor")
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Area.y = this.gameObject.transform.position.y;
            isDrop = true;
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

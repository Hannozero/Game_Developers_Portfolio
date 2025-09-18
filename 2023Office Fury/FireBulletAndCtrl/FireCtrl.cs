using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODPlus;



public class FireCtrl : MonoBehaviour
{

    const float seconds = 60;

    //화염프리팹
    public GameObject fireBullet;
    public GameObject fireBall;
    public GameObject fireExplosin;

    public int skillSet = 0;

    public ParticleSystem blueFire;

    public float maxGasGauge = 1000;
    public float gasGauge = 1000;

    public float reloadTime = 2.5f;
    public FMODAudioSource firesfx;
    public FMODAudioSource ReLoadsfx;
    public GameObject BurnDecal;

    [SerializeField] public bool isReload = false;
    [SerializeField] bool isFire = false;
    public bool CanFire = false;
    

    //화염이 발사되는 총구
    public Transform firePos;


    public float maxFireDelay;
    float curFireDelay;
    

    // Start is called before the first frame update
    void Start()
    {
        //RaycastHit hit;
        //if(Physics.Raycast())
        //GameObject hitHole = Instantiate(BurnDecal, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

        //Destroy(hitHole, 5f); // Destroying automatically
        curFireDelay = 0;
        gasGauge = maxGasGauge;
        isFire = false;
    }


    float fireballDelay = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        fireballDelay += Time.deltaTime;
        if (skillSet == 0)
        {
            maxFireDelay = 0.01f;
        }
        else if (skillSet >= 1) {
            maxFireDelay = 2f;
        }




        if (this.GetComponent<BasePlayer>().hp > 0)
        {
            if (gasGauge > 0)
            {
                //마우스 왼쪽 클릭중이면 화염발사
                if (Input.GetMouseButton(0) && CanFire)
                {
                    if (this.GetComponent<PlayerItem>().activeAxe)
                    {
                        FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
                        cam.IsShake = true;
                    }
                    else
                    {
                        if (skillSet == 0)
                        {
                            Fire();
                            if (!isFire)
                            {
                                firesfx.Play();
                                isFire = true;
                                blueFire.Play();
                            }
                            gasGauge--;
                        }
                        else if (skillSet == 1) {
                            FireBall();
                            if (!isFire)
                            {
                                firesfx.Play();
                                FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
                                cam.IsShake = true;
                                isFire = true;
                                blueFire.Play();
                            }
                            
                        }
                        else if (skillSet == 2)
                        {
                            FireExplosion();
                            if (!isFire)
                            {
                                firesfx.Play();
                                FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
                                cam.IsShake = true;
                                isFire = true;
                                blueFire.Play();
                            }

                        }
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (this.GetComponent<PlayerItem>().activeAxe)
                    {

                    }
                    else
                    {
                        firesfx.Stop();
                        isFire = false;
                        blueFire.Stop();
                    }
                }
            }
            else
            {
                if (!isReload)
                {
                    firesfx.Stop();
                    ReLoadsfx.Play();
                    blueFire.Stop();
                    Invoke("Reload", reloadTime);
                    isReload = true;
                    if (skillSet >= 1) {
                        skillSet = 0;
                    }
                }
            }

            //R누르면 즉시 재장전 시작
            if (Input.GetKeyDown(KeyCode.R) && !isReload && gasGauge != maxGasGauge)
            {
                firesfx.Stop();
                ReLoadsfx.Play();
                blueFire.Stop();
                isReload = true;
                gasGauge = 0;
                Invoke("Reload", reloadTime);
            }
        }

        //Debug.Log(Time.deltaTime);
    }

    void Fire() {
        //fireCliynder.SetActive(true);

        curFireDelay += Time.deltaTime;

        if (curFireDelay < maxFireDelay) {
            return;
        }

        CreateFire();
        curFireDelay = 0;
        
    }

    void FireBall()
    {
        //fireCliynder.SetActive(true);

        if (fireballDelay < maxFireDelay)
        {
            
            return;
        }
        CreateFireBall();


        fireballDelay = 0;
        gasGauge -= 20;

    }
    void FireExplosion()
    {
        //fireCliynder.SetActive(true);

        if (fireballDelay < maxFireDelay)
        {

            return;
        }
        CreateFireExplosion();


        fireballDelay = 0;
        gasGauge -= 20;

    }

    //재장전
    void Reload() {
        gasGauge = maxGasGauge;
        isReload = false;
        isFire = false;
    }

    void CreateFire() {
        Instantiate(fireBullet, firePos.position, firePos.rotation);
    }
    void CreateFireBall()
    {
        Instantiate(fireBall, firePos.position, firePos.rotation);
    }

    void CreateFireExplosion()
    {
        Instantiate(fireExplosin, firePos.position, firePos.rotation);
    }
}

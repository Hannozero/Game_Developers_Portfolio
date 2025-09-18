using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] private GameObject firebottle;
    [SerializeField] private GameObject PipeBoom;
    [SerializeField] ParticleSystem CryEffect;
    [SerializeField] private bool BattleCry;
    [SerializeField] private bool ActiveFB;
    [SerializeField] private bool ActiveAxe;
    [SerializeField] private bool ActiveBoom;
    [SerializeField] private bool ActiveCry;

    public bool isbattlecry { get { return BattleCry; } set { BattleCry = value; } }
    public bool battlecry { get { return ActiveCry; } set { ActiveCry = value; } }
    public bool activeAxe { get { return ActiveAxe; } set { ActiveAxe = value; } }
    public bool activeFB { get { return ActiveFB; } set { ActiveFB = value; } }
    public bool activeBoom { get { return ActiveBoom; } set { ActiveBoom = value; } }
    public GameObject getFB { get { return firebottle; } }
    public GameObject getBoom { get { return PipeBoom; } }

    public Transform playerEquipPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Animator ani;
    public GameObject axeObj;
    float Curtime;
    // Update is called once per frame
    void Update()
    {
        if (ActiveAxe == true) {
            if (Input.GetKeyDown(KeyCode.E)) {
                axeObj.SetActive(true);
                ani.SetBool("isAxe", true);

            }

            if (Input.GetKeyUp(KeyCode.E)) {
                Invoke("AxeOff", 2f);
                ani.SetBool("isAxeAttac", true);
                ani.SetBool("isAxe", false);
            }   
        }

        if(ActiveCry)
        {
            if (Input.GetKeyDown(KeyCode.E) && this.GetComponent<BasePlayer>().activeGauge == this.GetComponent<BasePlayer>().maxActiveGauge)
            {
                this.GetComponent<BasePlayer>().activeGauge = 0;
                FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
                cam.IsShake = true;
                BattleCry = true;
                CryEffect.Play();
            }
            if (BattleCry)
            {
                Curtime += Time.deltaTime;
                if (Curtime >= 5f)
                {
                    BattleCry = false;
                    Curtime = 0;
                }
            }
        }

    }

    void AxeOff() {
        axeObj.SetActive(false);
    }
}

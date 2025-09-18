// ====== Deep Commented Version (PlayerItem.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] private GameObject firebottle;
// 폭발 로직 — 범위 판정 및 넉백/피해
    [SerializeField] private GameObject PipeBoom;
    [SerializeField] ParticleSystem CryEffect;
    [SerializeField] private bool BattleCry;
    [SerializeField] private bool ActiveFB;
    [SerializeField] private bool ActiveAxe;
// 폭발 로직 — 범위 판정 및 넉백/피해
    [SerializeField] private bool ActiveBoom;
    [SerializeField] private bool ActiveCry;

    public bool isbattlecry { get { return BattleCry; } set { BattleCry = value; } }
    public bool battlecry { get { return ActiveCry; } set { ActiveCry = value; } }
    public bool activeAxe { get { return ActiveAxe; } set { ActiveAxe = value; } }
    public bool activeFB { get { return ActiveFB; } set { ActiveFB = value; } }
// 폭발 로직 — 범위 판정 및 넉백/피해
    public bool activeBoom { get { return ActiveBoom; } set { ActiveBoom = value; } }
    public GameObject getFB { get { return firebottle; } }
// 폭발 로직 — 범위 판정 및 넉백/피해
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
// 키 입력 처리 — 즉시 반응(폴링)
            if (Input.GetKeyDown(KeyCode.E)) {
                axeObj.SetActive(true);
                ani.SetBool("isAxe", true);

            }

// 키 입력 처리 — 즉시 반응(폴링)
            if (Input.GetKeyUp(KeyCode.E)) {
                Invoke("AxeOff", 2f);
                ani.SetBool("isAxeAttac", true);
                ani.SetBool("isAxe", false);
            }   
        }

        if(ActiveCry)
        {
// 키 입력 처리 — 즉시 반응(폴링)
            if (Input.GetKeyDown(KeyCode.E) && this.GetComponent<BasePlayer>().activeGauge == this.GetComponent<BasePlayer>().maxActiveGauge)
            {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                this.GetComponent<BasePlayer>().activeGauge = 0;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
                cam.IsShake = true;
                BattleCry = true;
                CryEffect.Play();
            }
            if (BattleCry)
            {
// 프레임 독립 보정(deltaTime)
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
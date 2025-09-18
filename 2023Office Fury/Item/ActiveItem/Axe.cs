using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Axe : MonoBehaviour
{
    public float AxeDamage = 100;
    [SerializeField] float AxeTime = 1f;
    [SerializeField] float AxeUseTime = 100f;
    [SerializeField] bool SwingAxe;
    [SerializeField] ParticleSystem axevfx;

    [SerializeField] GameObject player;
    [SerializeField] bool isAxe;
    [SerializeField] float CurTime;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<BasePlayer>().axeDamage = AxeDamage;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerItem>().activeAxe && player.GetComponent<BasePlayer>().activeGauge >= 300 && Input.GetKeyDown(KeyCode.E))
        {
            player.GetComponent<BasePlayer>().anima.SetBool("isAxe", true);
            player.GetComponent<BasePlayer>().activeGauge -= 300;
            isAxe = true;
        }
        if (isAxe)
        {
            CurTime += Time.deltaTime;
            if (Input.GetMouseButtonDown(0) && !SwingAxe)
            {
                player.GetComponent<BasePlayer>().anima.SetBool("isAxeAttac", true);
                axevfx.Play();
                SwingAxe = true;
                Invoke("AxeSwingDelay", AxeTime);
            }

            if (AxeUseTime < CurTime)
            {
                CurTime = 0f;
                isAxe = false;
            }
        }
    }

    void AxeSwingDelay()
    {
        SwingAxe = false;
        player.GetComponent<BasePlayer>().anima.SetBool("isAxeAttac", false);
    }
}

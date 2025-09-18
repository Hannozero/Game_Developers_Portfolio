using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMob : BaseMonster
{
    public GameObject[] dropItem;
    public bool Drop = true;

    public int moveSet;
    public int DeathSet;


    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        moveSet = UnityEngine.Random.Range(0, 3);
        Debug.Log(moveSet);
        anima.SetInteger("Move", moveSet);
    }

    // Update is called once per frame
    protected override void Update()
    {
        
        if (isDead == true) {
            S_Manager.m_boxCount--;
            int i = Random.Range(0, 3);
            if (Drop)
                Instantiate(dropItem[0], new Vector3(monster.transform.position.x, monster.transform.position.y + 0.5f, monster.transform.position.z), monster.transform.rotation);

                DeathSet = UnityEngine.Random.Range(0, 2);
                anima.SetInteger("Death", DeathSet);
            
            Drop = false;
        }

        base.Update();
    }

    protected override void Dead()
    {
        base.Dead();
    }
}

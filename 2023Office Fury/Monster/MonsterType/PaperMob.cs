using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperMob : BaseMonster
{
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
        base.Update();

        if (isDead == true) {
            DeathSet = UnityEngine.Random.Range(0, 2);
            anima.SetInteger("Death", DeathSet);
        }
    }
}

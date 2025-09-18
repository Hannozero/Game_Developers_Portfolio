using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    GameObject player;  //플레이어 오브젝트
    NavMeshAgent nav;   //네비메쉬

    public bool isMoving = true;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        nav=GetComponent<NavMeshAgent>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

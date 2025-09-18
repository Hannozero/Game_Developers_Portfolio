using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class MonsterTest : MonoBehaviour
{
    private Transform player;  // 플레이어의 Transform
    private NavMeshAgent agent;  // NavMesh 에이전트

    public float damage = 10f;
    public float speed = 1.0f;
    public float hp = 100f;

    public void Setup(float newHp, float newDamage, float newSpeed)
    {
        hp = newHp;
        damage = newDamage;
        agent.speed = newSpeed;
        //텍스처나 머테리얼을 가져와서 바꿀 수 있음.
    }

    private void Awake()
    {
        // 플레이어의 Transform 찾기 (플레이어 오브젝트의 태그를 "Player"로 설정해야 함)
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();// NavMesh 에이전트 설정
    }

    void Start()
    {

        if (agent != null)
        {
            if (player != null)
            {
                // NavMeshAgent에 목적지 설정
                agent.SetDestination(player.position);
            }
            else
            {
                Debug.LogError("목표 위치 (target)가 설정되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogError("NavMeshAgent가 이 객체에 추가되지 않았거나 비활성화되어 있습니다.");
        }
    }

    void Update()
    {
        // 플레이어를 계속 추적
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }
}

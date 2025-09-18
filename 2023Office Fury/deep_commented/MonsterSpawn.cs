// ====== Deep Commented Version (MonsterSpawn.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    private float nextSpawnTime;

    public GameObject monsterPrefab;
// 스폰(생성) 로직 — 수량/간격 제어
    public GameObject spawnObject;

    [SerializeField]private float TimeTick = 0f;
    [SerializeField] private float TimeCheck = 3f;
    //몬스터 게임 오브젝트-> 위치 ->주기 설정
    //스폰 매니저 만들기
    // Start is called before the first frame update
    void Start()
    {
// 스폰(생성) 로직 — 수량/간격 제어
        spawnObject = GameObject.Find("MonsterContainer");
    }

    // Update is called once per frame
    void Update()
    {
// 프레임 독립 보정(deltaTime)
        TimeTick += Time.deltaTime;

        if (TimeTick > TimeCheck)
        {
            if (Time.time >= nextSpawnTime && S_Manager.MaxMonsterCount > S_Manager.GameMonsterCount)
            {
// 스폰(생성) 로직 — 수량/간격 제어
                SpawnMonster();
// 스폰(생성) 로직 — 수량/간격 제어
                nextSpawnTime = Time.time + S_Manager.spawnInterval; // 다음 스폰 시간 갱신
            }
        }
    }

// 스폰(생성) 로직 — 수량/간격 제어
    void SpawnMonster()
    {
        if(this.gameObject.activeSelf)
        {
            MonsterData randomdata = S_Manager.GetMonsterData();
// 프리팹 인스턴스 생성
            GameObject monster = Instantiate(randomdata.visualPrefab, this.transform.position, Quaternion.identity);
// 스폰(생성) 로직 — 수량/간격 제어
            monster.transform.parent = spawnObject.transform;
            S_Manager.GameMonsterCount++;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    private float nextSpawnTime;

    public GameObject monsterPrefab;
    public GameObject spawnObject;

    [SerializeField]private float TimeTick = 0f;
    [SerializeField] private float TimeCheck = 3f;
    //몬스터 게임 오브젝트-> 위치 ->주기 설정
    //스폰 매니저 만들기
    // Start is called before the first frame update
    void Start()
    {
        spawnObject = GameObject.Find("MonsterContainer");
    }

    // Update is called once per frame
    void Update()
    {
        TimeTick += Time.deltaTime;

        if (TimeTick > TimeCheck)
        {
            if (Time.time >= nextSpawnTime && S_Manager.MaxMonsterCount > S_Manager.GameMonsterCount)
            {
                SpawnMonster();
                nextSpawnTime = Time.time + S_Manager.spawnInterval; // 다음 스폰 시간 갱신
            }
        }
    }

    void SpawnMonster()
    {
        if(this.gameObject.activeSelf)
        {
            MonsterData randomdata = S_Manager.GetMonsterData();
            GameObject monster = Instantiate(randomdata.visualPrefab, this.transform.position, Quaternion.identity);
            monster.transform.parent = spawnObject.transform;
            S_Manager.GameMonsterCount++;
        }
    }
}

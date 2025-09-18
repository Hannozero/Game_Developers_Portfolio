// ====== Deep Commented Version (NewProjectile.cs) — 함수/라인 설명 강화 ======

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SocialPlatforms;
// 사운드 재생/효과음 트리거
using FMODPlus;

public class NewProjectile : MonoBehaviour
{
// 사운드 재생/효과음 트리거
    [SerializeField] FMODAudioSource throwsound;

    public GameObject bulletPrefabs;
   // public GameObject cursor;
    public Transform ThrowPoint;
    public LayerMask layer;

    private Camera _cam;
    public float AttackRange = 6f;

    public GameObject Cursor;
    public GameObject Range;

    public Vector3 point;
    [SerializeField] bool Throwing = false;
    public bool PipeThrowing = false;

    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main;
        Range.SetActive(false);
        Cursor.SetActive(false);
        SetBullet();
    }
    void SetBullet()
    {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        if (this.GetComponent<PlayerItem>().activeFB)
        {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            bulletPrefabs = this.GetComponent<PlayerItem>().getFB;
        }
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        if (this.GetComponent<PlayerItem>().activeBoom)
        {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            bulletPrefabs = this.GetComponent<PlayerItem>().getBoom;
        }
    }

    // Update is called once per frame
    void Update()
    {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        if (this.GetComponent<PlayerItem>().activeBoom || this.GetComponent<PlayerItem>().activeFB)
        {
// 키 입력 처리 — 즉시 반응(폴링)
            if (Input.GetKeyDown(KeyCode.E) && this.gameObject.GetComponent<BasePlayer>().activeGauge == this.gameObject.GetComponent<BasePlayer>().maxActiveGauge)
            {
                Throwing = true;
            }
        }
        if(Throwing)
            LaunchProjectile();
        if(Input.GetMouseButtonDown(1))
        {
            throwsound.Play();
            Throwing = false;
            Range.SetActive(false);
            Cursor.SetActive(false);
        }
    }

    void LaunchProjectile()
    {
        Ray camRay = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //100f = distance
// Raycast — 화면/월드에서 충돌체 탐지
        if (Physics.Raycast(camRay, out hit, 100f, layer))
        {
            Cursor.SetActive(true);
            Range.SetActive(true);
            Vector3 dir = hit.point - transform.position;
            dir.y = 0.0f;
            dir.Normalize();
// 두 점 사이 거리 계산
            float distance = Vector3.Distance(hit.point, transform.position);
            Vector3 skillPoint = transform.position + dir * MathF.Min(AttackRange, distance);
            point = skillPoint;

            Vector3 Vo = CalculateVelocity(skillPoint, ThrowPoint.position, 1f);
            Cursor.transform.position = point;

// 방향 벡터를 회전값으로 변환
            ThrowPoint.rotation = Quaternion.LookRotation(Vo);

            if (Input.GetMouseButtonDown(0))
            {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                this.gameObject.GetComponent<BasePlayer>().activeGauge -= 100;
// 프리팹 인스턴스 생성
                Rigidbody obj = Instantiate(bulletPrefabs.GetComponent<Rigidbody>(), ThrowPoint.position, Quaternion.identity);
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
                if (this.GetComponent<PlayerItem>().activeBoom)
                    PipeThrowing = true;
                obj.velocity = Vo;
                Cursor.SetActive(false);
                Range.SetActive(false);
                Throwing = false;
            }
        }
        else
        {
            Cursor.SetActive(false);
        }
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        //define the distance x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        //create a float the represent our distance
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;

    }
}
// ====== Deep Commented Version (Movement.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// 사운드 재생/효과음 트리거
using FMODPlus;

public class Movement : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public Transform upperBody;

    public float speed = 10f;
    public float rotateSpeed = 7f;
    public float maxRotationAngle = 90f;
    public bool isDead = false;
// 사운드 재생/효과음 트리거
    [SerializeField] FMODAudioSource footstep;

    [SerializeField] Vector3 RotPos = Vector3.zero;
    //public Quaternion upperBodyTargetRotation; 애니메이션 제작 된 후 사용 예정
    // Start is called before the first frame update
    void Start()
    {
        //_controller = this.GetComponent<CharacterController>();
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        _rigidbody = this.GetComponent<Rigidbody>();
        //upperBodyTargetRotation = upperBody.rotation;
    }

    void LateUpdate()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            CharacterRotate();
            CharacterMove();
        }
    }

    //────────────────────────────────────────────────────────────────────

    // CharacterMove : 함수 개요

    //  - 목적: 이동/회전 처리

    //  - 주의: deltaTime/물리충돌과의 상호작용 주의

    //────────────────────────────────────────────────────────────────────

    private void CharacterMove()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        // -1 ~ 1

        if (inputX != 0 || inputZ != 0)
        {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            GetComponent<BasePlayer>().anima.SetBool("FrontRun", true);

            if (!footstep.isPlaying)
                footstep.Play();
        }
        else
        {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            GetComponent<BasePlayer>().anima.SetBool("FrontRun", false);
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            GetComponent<BasePlayer>().anima.SetBool("BackRun", false);
            footstep.Stop();
        }

        Vector3 velocity = new Vector3(inputX, 0, inputZ).normalized * speed;

        _rigidbody.velocity = velocity;
    }

    //────────────────────────────────────────────────────────────────────

    // CharacterRotate : 함수 개요

    //  - 목적: 이동/회전 처리

    //  - 주의: deltaTime/물리충돌과의 상호작용 주의

    //────────────────────────────────────────────────────────────────────

    private void CharacterRotate()
    {
// 화면 좌표 → 레이 생성(마우스 조준에 사용)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 pointToLook = ray.GetPoint(rayLength);
            Vector3 lookDirection = pointToLook - transform.position;
            lookDirection.y = 0; // y-축 회전 방지
            lookDirection.Normalize(); // 방향 벡터를 정규화

            if (lookDirection != Vector3.zero)
            {
// 방향 벡터를 회전값으로 변환
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                targetRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0); // x-축 회전 고정
// 프레임 독립 보정(deltaTime)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
        }
    }

}
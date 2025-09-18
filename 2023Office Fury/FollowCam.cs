using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.UI.Image;

public class FollowCam : MonoBehaviour
{
    public Vector3 CamRotation;
    public Vector3 CamPoision;
    

    [SerializeField] Transform target = null;
    [SerializeField] GameObject player;
    [SerializeField] Camera _cam;
    [SerializeField] float TargetDistance = 400f;
    [SerializeField] float MaxRangeDistance = 1000f;
    [SerializeField] float Range = 3f;
    [SerializeField] float Speed = 3f;


    //카메라쉐이크 관련변수
    [SerializeField] Vector3 shakeDir = Vector3.zero;
    [SerializeField] float shakeForce = 0f;
    [SerializeField] bool isShake;
    [SerializeField] bool firstShake;
    [SerializeField] float shakeTime;

    public bool IsShake
    { 
        get { return isShake; }
        set { isShake = value; }
    }
    

    Quaternion origin;

    // Start is called before the first frame update
    public void SetTarget(GameObject _target)
    {
        target = _target.transform;
    }

    void Start()
    {
        origin = transform.rotation;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public IEnumerator Shake()
    {
        while (true)
        {
            float rotX = UnityEngine.Random.Range(-shakeDir.x, shakeDir.x);
            float rotY = UnityEngine.Random.Range(-shakeDir.y, shakeDir.y);
            float rotZ = UnityEngine.Random.Range(-shakeDir.z, shakeDir.z);

            Vector3 rotationV = origin.eulerAngles + new Vector3(rotX, rotY, rotZ);
            Quaternion rotationQ = Quaternion.Euler(rotationV);

            // 랜덤각도와의 오차가 0.1도일때까지 카메라 회전
            while (Quaternion.Angle(transform.rotation, rotationQ) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationQ, shakeForce * Time.deltaTime);

                yield return null;
            }
            yield return null;
        }
    }

    public IEnumerator Stop()
    {
        // 초기 각도와 같아질때까지 회전
        while (Quaternion.Angle(transform.rotation, origin) > 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, origin, shakeForce * Time.deltaTime);
            yield return null;
        }
    }




    void Update()
    {
        Vector3 movePos = Vector3.zero;

        //transform.localRotation = Quaternion.Euler(CamRotation);

        Vector3 targetPos = Camera.main.WorldToScreenPoint(target.position);
        targetPos.z = 0f;
        Vector3 MousePos = Input.mousePosition;
        Vector3 Dir = MousePos - targetPos;
        Dir.Normalize();
        float Distance = Vector3.Distance(targetPos, MousePos);
        Distance = Mathf.Min(MaxRangeDistance, Distance);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            StartCoroutine(Shake());
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            StopAllCoroutines();
            StartCoroutine(Stop());
        }

        if (isShake)
        {
            if (!firstShake)
            {
                StopAllCoroutines();
                StartCoroutine(Shake());
                firstShake = true;
            }
            shakeTime += Time.deltaTime;
            if(shakeTime > 0.5f)
            {
                StopAllCoroutines();
                StartCoroutine(Stop());
                firstShake = false;
                isShake = false;
                shakeTime = 0;
            }
        }

        if (TargetDistance > Distance)
        {
            movePos = target.position + CamPoision;
        }
        else
        {
            movePos = target.position + CamPoision + new Vector3(Dir.x, 0f, Dir.y) * (Distance / MaxRangeDistance * Range);
        }

        if (Vector3.Distance(transform.position, movePos) < Speed * Time.deltaTime)
        {
            transform.position = movePos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, movePos, Speed * Time.deltaTime);
        }
        if (player.GetComponent<BasePlayer>().hp <= 0)
        {
            movePos = target.position + CamPoision;
            transform.position = Vector3.Lerp(transform.position, movePos, Speed * Time.deltaTime);
        }
    }
}

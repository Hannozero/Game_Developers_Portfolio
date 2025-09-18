using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using FMODPlus;

public class NewProjectile : MonoBehaviour
{
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
        if (this.GetComponent<PlayerItem>().activeFB)
        {
            bulletPrefabs = this.GetComponent<PlayerItem>().getFB;
        }
        if (this.GetComponent<PlayerItem>().activeBoom)
        {
            bulletPrefabs = this.GetComponent<PlayerItem>().getBoom;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<PlayerItem>().activeBoom || this.GetComponent<PlayerItem>().activeFB)
        {
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
        if (Physics.Raycast(camRay, out hit, 100f, layer))
        {
            Cursor.SetActive(true);
            Range.SetActive(true);
            Vector3 dir = hit.point - transform.position;
            dir.y = 0.0f;
            dir.Normalize();
            float distance = Vector3.Distance(hit.point, transform.position);
            Vector3 skillPoint = transform.position + dir * MathF.Min(AttackRange, distance);
            point = skillPoint;

            Vector3 Vo = CalculateVelocity(skillPoint, ThrowPoint.position, 1f);
            Cursor.transform.position = point;

            ThrowPoint.rotation = Quaternion.LookRotation(Vo);

            if (Input.GetMouseButtonDown(0))
            {
                this.gameObject.GetComponent<BasePlayer>().activeGauge -= 100;
                Rigidbody obj = Instantiate(bulletPrefabs.GetComponent<Rigidbody>(), ThrowPoint.position, Quaternion.identity);
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

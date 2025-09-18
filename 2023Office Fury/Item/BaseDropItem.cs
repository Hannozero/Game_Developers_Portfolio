using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDropItem : MonoBehaviour
{
    public GameObject player;
    public GameObject dropItem;


    protected virtual void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void Delete() {
        Destroy(dropItem, 1f);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            this.gameObject.SetActive(false);
            Delete();
        }
    }

}

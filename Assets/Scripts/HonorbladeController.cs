using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonorbladeController : MonoBehaviour
{
    private GuardPatrol enemy;
    private PlayerController playerControl;
    public Collider damageArc;

    // Start is called before the first frame update
    void Start()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponent<GuardPatrol>())
        {
            enemy = other.gameObject.GetComponent<GuardPatrol>();
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<GuardPatrol>())
        {
            enemy = null;
        }

    }

    public GuardPatrol getEnemy()
    {
        return enemy;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayController : MonoBehaviour
{
    public float interactDistance;

    private Animator m_Animator;
    private GameObject player;
    private PlayerController playerControl;
    private bool isOpen = false;
    private Collider collider;

    float m_Timer;
    //private float timeRemaining = 5;
    private bool timerIsRunning = false;
    private Vector3 diff = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        playerControl = player.GetComponent<PlayerController>();
        collider = transform.Find("DoorL").gameObject.GetComponent<Collider>();
        diff = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        //diff = diff - transform.position;
        //collider.transform.position = collider.transform.position + diff;
        //// set timer
        //if (timerIsRunning)
        //{
        //    if (timeRemaining > 0)
        //    {
        //        timeRemaining -= Time.deltaTime;
        //    }
        //    else
        //    {
        //        timeRemaining = 0;
        //        timerIsRunning = false;
        //    }
        //}

        //if (playerControl.getIsInteracting() && !timerIsRunning)
        //{
            
        //    checkOpen();
        //    timeRemaining = 2f;
        //    timerIsRunning = true;

        //}
    }

    void FixedUpdate()
    {
        //diff = diff - transform.position;
        //collider.transform.position = collider.transform.position + diff;
    }

    void checkOpen()
    {
        var distance = Vector3.Distance(transform.position, player.transform.position);
        //Debug.DrawLine(transform.position, player.transform.position, Color.yellow);
        if (distance < interactDistance)
        {
            isOpen = !isOpen;
            
        }

        if(isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }

    }

    void OnTriggerEnter(Collider collider)
    {
        OpenDoor();
    }

    void OnTriggerExit(Collider collider)
    {
        CloseDoor();
    }

    public void OpenDoor()
    {
        m_Animator.SetBool("IsOpen", true);
        m_Animator.SetBool("IsClose", false);
        //m_Animator.ResetTrigger("Close");
        if (collider)
        {
            //collider.enabled = false;
        }
    }

    public void CloseDoor()
    {
        m_Animator.SetBool("IsClose", true);
        m_Animator.SetBool("IsOpen", false);
        //m_Animator.ResetTrigger("Open");
        if (collider)
        {
            //collider.enabled = true;
            playerControl.setIsInteracting(false);
        }
        else
        {
            //collider = transform.Find("DoorL").gameObject.GetComponent<Collider>();
        }
    }
}

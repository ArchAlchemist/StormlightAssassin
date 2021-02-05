using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPatrol : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;
    public int sensoryRange;
    public int health = 1;
    public int damage = 1;
    //public AudioSource m_Audio;

    //Audio
    //public AudioSource m_Audio;
    //public AudioClip deathBite;
    //public AudioClip walkBite;

    int m_CurrentWaypointIndex;
    Transform target;
    bool isChasing;
    private GameObject player;
    private PlayerController playerControl;
    private Animator m_Animator;
    private float distance;
    private float m_Timer;
    private float timeRemaining = 5;
    private bool timerIsRunning = false;
    private bool isDead = false;
    private Collider damageArc;
    private Rigidbody m_Rigidbody;

    bool m_HasAudioPlayed;

    // set the initial waypoint for mesh agent to travel to
    void Start()
    {
        navMeshAgent.SetDestination(waypoints[0].position);
        isChasing = false;

        player = GameObject.Find("Player");
        playerControl = player.GetComponent<PlayerController>();

        m_Animator = GetComponent<Animator>();
        distance = Vector3.Distance(player.transform.position, transform.position);

        //m_Audio = GetComponent<AudioSource>();

        damageArc = transform.Find("damageArc").gameObject.GetComponent<Collider>();

        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // change to next waypoint if the mech agent is within an acceptable distance from the current waypoint.
    void Update()
    {
        if(!isDead)
        {
            distance = Vector3.Distance(player.transform.position, transform.position);


            // if player is within radius make the player the current target
            if (distance <= sensoryRange && !playerControl.getIsDead())
            {
                chaseTarget(player.transform);
                // set run
                m_Animator.SetBool("IsRunning", true);
                //m_Audio.Play();
            }
            else
            {
                stopChase();
            }

            if (isChasing)
            {
                if (distance < 1.25f)
                {
                    m_Animator.SetBool("IsWalking", false);
                    m_Animator.SetBool("IsRunning", false);
                    //m_Audio.Stop();
                    // set destination for here so they stay
                    navMeshAgent.isStopped = true;
                    // rotate
                    RotateTowards(player.transform);

                    if (!checkAttacking())
                    {
                        m_Animator.SetTrigger("Thrust");
                        timerIsRunning = true;
                        timeRemaining = 2f;
                        if (playerControl.IsInside(damageArc))
                        {
                            playerControl.setDamage(damage);
                            
                        }
                        m_Animator.SetBool("IsAttacking", true);
                    }
                }
                else
                {
                    navMeshAgent.isStopped = false;
                    m_Animator.SetBool("IsAttacking", false);
                    m_Animator.SetBool("IsRunning", true);
                    //m_Audio.Play();
                }
                navMeshAgent.SetDestination(target.position);
                // set speed
                navMeshAgent.speed = 3.0f;
            }
            else if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {

                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                m_Animator.SetBool("IsWalking", true);
                //m_Audio.Play();
            }
        }
        else
        {
            //m_Audio.Stop();
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                }
                else
                {
                    timeRemaining = 0;
                    timerIsRunning = false;
                    m_Animator.speed = 0;
                    m_Rigidbody.gameObject.SetActive(false);
                }
            }
        }
        
    }

    // other functions can have current parent chase target
    public void chaseTarget(Transform t)
    {
        target = t;
        isChasing = true;
    }

    // have current parent stop chasing current target
    public void stopChase()
    {
        isChasing = false;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position); // TODO change to closest waypoint
        m_Animator.SetBool("IsWalking", true);
        //m_Audio.Play();
        m_Animator.SetBool("IsRunning", false);
        navMeshAgent.speed = 1.0f;
        navMeshAgent.isStopped = false;
        m_Animator.SetBool("IsAttacking", false);
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 14);
    }

    // set player to dead so they can't continue, then open menu
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player.gameObject)
        {
                //playerControl.setIsDead(true);
                //playerControl.setLevelLoad(true);
        }
    }

    bool checkAttacking()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                m_Animator.SetBool("IsAttacking", false);
                //loadNextScene(); // DO THE THING!
                //if (isAttacking)
                //{
                //    isAttacking = false;
                //    //m_Animator.
                //}
            }
        }

        return timerIsRunning;
    }

    public bool IsInside(Collider c)
    {
        Vector3 closest = c.ClosestPoint(transform.position);
        // Because closest=point if inside - not clear from docs I feel
        return closest == transform.position;
    }

    public void setDamage(int dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            m_Animator.SetBool("IsDead", true);
            m_Animator.SetTrigger("Dead");
            //m_Audio.PlayOneShot(deathBite, 07f);
            isDead = true;
            timerIsRunning = true;
            timeRemaining = 0.8f;

            if(gameObject.tag == "Boss")
            {
                //Debug.Log("Boss is dead");
                playerControl.setLevelLoad();
            }


        }
    }
}

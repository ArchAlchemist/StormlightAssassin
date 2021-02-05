using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Stormlight Assassin
public class PlayerController : MonoBehaviour
{
    public float turnSpeed = 10.0f;
    public float walkSpeed = 0.04f;
    public float runSpeed = 0.08f;
    public float animationSlowSpeed = 0.04f;
    public float speed = 0.04f;
    public Vector3 jump;
    public float jumpForce = 2.0f;
    public float jumpBuffer = 0.1f;
    public AudioSource m_CaughtAudioSource;
    public int trophyTotal;
    public float jumpHeight;
    public float mass = 62; // kilograms, 1 kg == 1 unity mass unit
    public int health;
    public int damage;
    public float heightAllowance = 1f;

    public CameraForwardController camForwardController;

    private GameObject m_freeLookComponent;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI powerText;
    public GameObject levelLoadObject;
    public GameObject menuObject;
    public GameObject continueOption;
    public RectTransform stormLightMeter;
    public int stormlightRatio;

    Animator m_Animator;
    private Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_PlayerVelocity;
    Quaternion m_Rotation = Quaternion.identity;
    
    private int count;
    private bool inMenu = false;
    float m_Timer;
    private float timeRemaining = 5;
    private bool timerIsRunning = false;
    private GameObject winText;
    private float movementX;
    private float movementY;
    private FreeLookController m_Flc;
    private bool groundedPlayer = true;
    private float halfHeight;
    private Vector3 desiredForward = Vector3.forward;
    private Vector3 newRotationTarget;
    private bool isDead = false;
    private HonorbladeController damageArc;
    private GuardPatrol enemy;
    private Vector3 closest = Vector3.zero;
    private bool isClosest = false;
    private bool isInteracting = false;
    private bool isHero = false;
    private bool hasFailed = false;




    private float currentGravity = -9.81f;
    private Vector3 m_CurrentPos;
    private bool isWalking;
    private int runningAnimation = 0;
    private bool jumpDelayed = false;
    private bool isAttacking = false;





    // Start is called before the first frame update
    // sets player object dependant components
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        m_freeLookComponent = GameObject.Find("CM_ThirdPerson");
        m_Flc = m_freeLookComponent.GetComponent<FreeLookController>();//.look(context);
        m_Animator.SetBool("IsWalking", false);
        m_CurrentPos = transform.position;
        m_AudioSource.Stop();
        currentGravity = Physics.gravity.y;
        isWalking = false;

        levelLoadObject.SetActive(false);
        menuObject.SetActive(false);
        

        RaycastHit hit;
        Ray downRay = new Ray(transform.position, -Vector3.up);
        Physics.Raycast(downRay, out hit);
        halfHeight = hit.distance;
        damageArc = transform.Find("damageArc").gameObject.GetComponent<HonorbladeController>();
        stormLightMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, stormlightRatio * health);
        //stormLightMeter.ForceUpdateRectTransforms();
        //winText = GameObject.Find("Win Text");

    }

    void Update()
    {
        
        // TODO place universal timer here. no other timers can be set while another is active? Wish I could pass it a lambda when timer is done, so nice
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                if (isAttacking)
                {

                }
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                //loadNextScene(); // DO THE THING!
                if(jumpDelayed)
                {
                    LaunchJump();
                    jumpDelayed = false;
                }
                if(isAttacking)
                {
                    isAttacking = false;
                }
                if(isDead)
                {
                    levelLoadObject.SetActive(true);
                    winText = GameObject.Find("Win Text");
                    winText.GetComponent<TextMeshProUGUI>().text = "Ooops, you died!";
                    //m_Animator.speed = 0;
                    //Time.timeScale = 0; // works to pause!
                    // launch menu
                }
                if(isHero)
                {
                    Time.timeScale = 0;
                }
                if(hasFailed)
                {
                    levelLoadObject.SetActive(true);
                    winText = GameObject.Find("Win Text");
                    winText.GetComponent<TextMeshProUGUI>().text = "The king had extra time to escape, you failed";
                    speed = 0f;
                    turnSpeed = 0f;
                    m_AudioSource.Stop();
                    isDead = true;
                    Time.timeScale = 0f;
                    
                }
            }
        }

    }

    // Update is called once per frame in time with the physics engine (asd opposed to the render engine)
    void FixedUpdate()
    {
        if (isWalking && !isDead && !isHero && !hasFailed)
        {
            
            // rotate
            desiredForward = Vector3.RotateTowards(transform.forward, m_PlayerVelocity, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
        }

    }


    // called in time with physics engine
    // Animator component switched from render to physics time in GUI
    void OnAnimatorMove()
    {
        if(!isAttacking && !isDead )
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_PlayerVelocity * speed);
            m_Rigidbody.MoveRotation(m_Rotation);
        }
    }


    //**************************************************
    // Animation Updaters
    //*************************

    void OnCollisionEnter(Collision collision) // updates Jumping animation
    {
        Vector3 contact = collision.contacts[0].point;
        RaycastHit hit;
        Ray downRay = new Ray(transform.position, -Vector3.up);
        if(Physics.Raycast(downRay, out hit))
        {
            groundedPlayer = hit.distance <= (halfHeight + jumpBuffer); // player grounded
            if (groundedPlayer)
            {
                m_Animator.SetBool("IsJumping", false);
                //runningAnimation = 0;
                //m_Animator.SetBool("IsLanding", true);

                if(isWalking)
                {
                    m_AudioSource.Play();
                }
            }
        }
    }

    float JumpForceCalculate(float targetHeight, float mass, float gravity)
    {
        float linearForce = -2 * targetHeight * gravity;
        float forceDown = Mathf.Sqrt(linearForce);
        //Debug.Log("force of Gravity: " + forceDown);
        return mass * forceDown;
    }

    void LaunchJump()
    {
        float jumpForce = JumpForceCalculate(jumpHeight, mass, currentGravity);
        //Debug.Log("jumpForce: " + jumpForce);
        //Debug.Log("vector: " + Vector3.up * jumpForce);
        m_Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //m_PlayerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * currentGravity);
    }

    //Transform GetClosestEnemy(Transform[] enemies)
    //{
    //    Transform bestTarget = null;
    //    float closestDistanceSqr = Mathf.Infinity;
    //    Vector3 currentPosition = transform.position;
    //    foreach (Transform potentialTarget in enemies)
    //    {
    //        Vector3 directionToTarget = potentialTarget.position - currentPosition;
    //        float dSqrToTarget = directionToTarget.sqrMagnitude;
    //        if (dSqrToTarget < closestDistanceSqr)
    //        {
    //            closestDistanceSqr = dSqrToTarget;
    //            bestTarget = potentialTarget;
    //        }
    //    }

    //    return bestTarget;
    //}

    public bool IsInside(Collider c)
    {
        isClosest = false;
        closest = c.ClosestPoint(transform.position);// if inside it will give you the same point you asked to check
        if (Vector3.Distance(closest, transform.position) <= heightAllowance)
        {
                isClosest = true;
        }
        
        return isClosest;
    }

    

    //**************************************************
    // Player Input Commands
    //*************************

    public void OnSlash(InputAction.CallbackContext context)
    {
        if(context.started && !isDead)
        {
            m_Animator.SetTrigger("Slash");
            runningAnimation = 4;
            isAttacking = true;
            timeRemaining = 0.4f;
            timerIsRunning = true;
            // TODO set forward collider trigger as active
            enemy = damageArc.getEnemy();
            if(enemy)
            {
                enemy.setDamage(damage);
            }

        }
    }

    public void OnPunch(InputAction.CallbackContext context)
    {
        //if (context.started && !isDead)
        //{
        //    m_Animator.SetTrigger("Punch");
        //    runningAnimation = 5;
        //    isAttacking = true;
        //    timeRemaining = 0.4f;
        //    timerIsRunning = true;
        //    // TODO set forward collider trigger as active
        //}
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started && !isDead)
        {
            isInteracting = true;
        }
        if(context.canceled)
        {
            isInteracting = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started && !isDead && !isHero && !hasFailed)
        {
            m_Animator.SetBool("IsRunning", true);
            runningAnimation = 2;
            speed = runSpeed;
        }
        if (context.canceled)
        {
            m_Animator.SetBool("IsRunning", false);
            speed = walkSpeed;
        }
    }

    // add jump functionality via Input System asset
    public void OnJump(InputAction.CallbackContext context)
    {
        
        // TODO add delay for animation wind up
        // add timer here, trigger in update

        if (context.started && !isDead && !isHero && !hasFailed)
        {
            m_Animator.SetBool("IsJumping", true);
            runningAnimation = 3;

            // check for distance of surface below player
            RaycastHit hit;
            Ray downRay = new Ray(transform.position, -Vector3.up);

            if (Physics.Raycast(downRay, out hit))
            {
                var toGround = halfHeight + jumpBuffer - hit.distance;
                groundedPlayer = (hit.distance - (halfHeight + jumpBuffer)) <= 0;
                // allow jump if distance is not greater than the collider radius
                if (groundedPlayer)
                {
                    jumpDelayed = true;
                    timerIsRunning = true;
                    timeRemaining = 0.2f;
                    m_AudioSource.Stop();
                }
            }
        }
        
    }

    // Unity standard inputs listener
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!inMenu && !isDead && !isHero && !hasFailed)
        {
            if (context.canceled)
            {
                m_PlayerVelocity = Vector3.zero;
                m_Animator.SetBool("IsWalking", false);
                isWalking = false;
                m_AudioSource.Stop();
            }
            else
            {
                m_Animator.SetBool("IsWalking", true);
                isWalking = true;
                runningAnimation = 1;

                
                Vector2 movementVector = context.ReadValue<Vector2>();
                
                // position
                movementX = movementVector.x;
                movementY = movementVector.y;
                //Debug.Log("(X,Y): " + movementVector.x + " , " + movementVector.y);

                newRotationTarget = camForwardController.getNewTarget(movementVector.x, movementVector.y);
                //m_PlayerVelocity.Set(newTarget.x, 0f, newTarget.y);

                m_PlayerVelocity.Set(newRotationTarget.x, 0f, newRotationTarget.z);
                //m_PlayerVelocity.Set(movementX, 0f, movementY);
                m_PlayerVelocity.Normalize();

                if (!m_AudioSource.isPlaying && runningAnimation < 3) // never called bellow 1, and 3+ are all seperate sounds
                {
                    m_AudioSource.Play();
                }
            }
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        m_Flc.look(context);
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if(context.started && !isDead && !isHero && !hasFailed)
        {
            
            //menuObject.SetActive(!inMenu);
            if (!inMenu)
            {
                menuObject.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                menuObject.SetActive(false);
                Time.timeScale = 1f;
            }
            inMenu = !inMenu;
        }
        
    }


    //**************************************************
    // Getters 
    //*************************

    public Vector3 getMoveTarget()
    {
        return m_PlayerVelocity;// desiredForward;
    }

    public bool getIsDead()
    {
        return isDead;
    }

    public bool getIsInteracting()
    {
        return isInteracting;
    }


    //**************************************************
    // Setters 
    //*************************

    public void setMoveTarget(Vector3 target)
    {
        desiredForward = target;
    }

    public void setIsDead(bool val)
    {
        isDead = val;
        // TODO continueOption.SetActive(!isDead);
        m_Animator.SetTrigger("Dead");
        //m_Animator.speed = 0.04f;
        timerIsRunning = true;
        timeRemaining = 2f;
        Time.timeScale = 0.25f;
        m_AudioSource.Stop();

        if (isDead)
        {
            continueOption.SetActive(false);
        }
        else
        {
            continueOption.SetActive(true);
        }
    }

    public void increaseStormlight(int h)
    {
        if (!isDead)
        {
            health += h;
            stormLightMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, stormlightRatio * health);
        }

    }

    public void setDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0 && !isDead)
        {
            health = 0;
            setIsDead(true);
            
        }
        stormLightMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, stormlightRatio * health);
    }

    

    public void setIsInteracting(bool val)
    {
        isInteracting = val;
    }

    public void setInMenu()
    {
        inMenu = !inMenu;
    }

    public void setMenu(bool val)
    {
        menuObject.SetActive(val);
    }

    public void setLevelLoad()
    {

        //m_Animator.speed = 0.04f;
        timerIsRunning = true;
        timeRemaining = 2f;
        Time.timeScale = 0.25f;
        isHero = true;
        m_AudioSource.Stop();
        //Debug.Log(winText);
        levelLoadObject.SetActive(true);
        //winText.GetComponent<TextMeshProUGUI>().text = "You did it! You got the big guy! Congrats!";
        //if (isDead)
        //{
        //    winText = GameObject.Find("Win Text");
        //    winText.GetComponent<TextMeshProUGUI>().text = "Ooops, you died!";
        //}
    }

    public void HasFailed()
    {
        //m_Animator.speed = 0.04f;
        timerIsRunning = true;
        timeRemaining = 2f;
        Time.timeScale = 0.25f;
        m_AudioSource.Stop();

        continueOption.SetActive(false);
        continueOption.SetActive(true);

        hasFailed = true;
    }

}


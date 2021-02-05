using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StormlightGiver : MonoBehaviour
{
    public GameObject player;
    public GameObject infobox;
    public string text;
    public int delay;
    public int stormlight;

    private TextMeshProUGUI textBox;
    private bool timerIsRunning;
    private float timeRemaining = 0f;
    private PlayerController playerControl;
    private bool isEmpty = false;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerControl = player.GetComponent<PlayerController>();
        textBox = infobox.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
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
                infobox.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.CompareTag("Player") && !isEmpty)
        {
            // if player steps into it then change info box's text
            textBox.text = text;
            // set info box to active
            infobox.SetActive(true);
            // set info timer
            timeRemaining = delay;
            timerIsRunning = true;
            // if timer is up then set info box to inactive
            // increase player stormlight
            playerControl.increaseStormlight(stormlight);
            transform.gameObject.GetComponent<Light>().enabled = false;
            isEmpty = true;

        }
    }
}

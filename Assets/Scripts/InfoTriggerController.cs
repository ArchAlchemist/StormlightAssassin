using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoTriggerController : MonoBehaviour
{
    public GameObject player;
    public GameObject infobox;
    public string text;
    public int delay;

    private TextMeshProUGUI textBox;
    private bool timerIsRunning;
    private float timeRemaining = 0f;
    private bool triggered = false;


    // Start is called before the first frame update
    void Start()
    {
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
        if(collider.gameObject.CompareTag("Player") && !triggered)
        {
            // if player steps into it then change info box's text
            textBox.text = text;
            // set info box to active
            infobox.SetActive(true);
            // set info timer
            timeRemaining = delay;
            timerIsRunning = true;
            // if timer is up then set info box to inactive
            triggered = true;
        }

    }
}

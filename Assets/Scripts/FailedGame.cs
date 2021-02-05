using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedGame : MonoBehaviour
{
    public GameObject player;
    public AudioSource exitAudio;

    float m_Timer;
    bool m_IsPlayerAtExit;
    bool m_IsPlayerCaught;
    bool m_HasAudioPlayed;

    // execute code when the ui specified collider object collides with the trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            EndLevel(exitAudio);
        }
    }

    // standard update function, determines what end screen to play
    void Update()
    {
    }

    // run the specified background and sound for end of game
    void EndLevel(AudioSource audioSource)
    {

        player.GetComponent<PlayerController>().HasFailed();
        // only play audio file if it has not yet played
        if (!m_HasAudioPlayed)
        {
            audioSource.Play();
            m_HasAudioPlayed = true;
        }
    }
}

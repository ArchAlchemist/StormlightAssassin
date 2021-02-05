using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public GameObject player;
    public CanvasGroup exitBackgroundImageCanvasGroup;
    public CanvasGroup caughtBackgroundImageCanvasGroup;
    public AudioSource exitAudio;
    public AudioSource caughtAudio;

    float m_Timer;
    bool m_IsPlayerAtExit;
    bool m_IsPlayerCaught;
    bool m_HasAudioPlayed;

    // execute code when the ui specified collider object collides with the trigger
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            m_IsPlayerAtExit = true;
        }
    }

    // we caught the player (only object we can catch)
    public void CaughtPlayer()
    {
        m_IsPlayerCaught = true;
    }

    // standard update function, determines what end screen to play
    void Update()
    {
        if(m_IsPlayerAtExit)
        {
            EndLevel(exitBackgroundImageCanvasGroup, false, exitAudio);
        }
        else if (m_IsPlayerCaught)
        {
            EndLevel(caughtBackgroundImageCanvasGroup, true, caughtAudio);
        }
    }

    // run the specified background and sound for end of game
    void EndLevel(CanvasGroup imageCanvasGroup, bool doRestart, AudioSource audioSource)
    {
        m_Timer += Time.deltaTime;
        imageCanvasGroup.alpha = m_Timer / fadeDuration;

        // only play audio file if it has not yet played
        if (!m_HasAudioPlayed)
        {
            audioSource.Play();
            m_HasAudioPlayed = true;
        }

        // check if we;ve reached the end of the fade out
        if(m_Timer > fadeDuration + displayImageDuration)
        {
            // restart the initial level
            if(doRestart)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                // quit the game
                Application.Quit();
            }
        }
    }
}

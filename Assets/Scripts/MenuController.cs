using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menuObject;
    public PlayerController playerControl;

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void Continue()
    {
        // TODO turn the menu off
        menuObject.SetActive(false);
        playerControl.setInMenu();
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        Debug.Log("quitting");
        Application.Quit();
    }
}

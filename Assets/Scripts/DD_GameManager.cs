using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DD_GameManager : MonoBehaviour
{
    [Header("Menus & Screens")]
    [SerializeField] GameObject StartScreen;
    [SerializeField] GameObject GameOverScreen;
    [SerializeField] GameObject ControlsScreen;
    [Header("Audio")]
    [SerializeField] AudioSource StartMusic;
    [SerializeField] GameObject SM;
    [Header("Other")]
    [SerializeField] bool isPaused;
    [SerializeField] ControllerCharacter2D Player;
    [SerializeField] State CurrentState;
    enum State
    {
        START, PLAY, PAUSE, GAMEOVER
    }

    void Start()
    {
        CurrentState = State.START;
        StartMusic.Stop();
    }

    void Update()
    {

        if (Player.health <= 0)
        {
            StartMusic.Stop();
            CurrentState = State.GAMEOVER;
        }

        switch(CurrentState)
        {
            case State.START:
            {
                Time.timeScale = 0;
                StartScreen.SetActive(true);
                if (Input.GetKeyDown(KeyCode.S))
                {
                    StartScreen.SetActive(false);
                    Time.timeScale = 1;
                    CurrentState = State.PLAY;
                    StartMusic.Play();
                }
                break;
            }

            case State.PLAY:
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    CurrentState = State.PAUSE;
                    TogglePause();
                }
                break;
            }

            case State.PAUSE:
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    CurrentState = State.PLAY;
                    TogglePause();
                }
                break;
            }

            case State.GAMEOVER:
            {
                Time.timeScale = 1;
                GameOverScreen.SetActive(true);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                break;
            }
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause the game
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game
            Time.timeScale = 1;
        }
    }
}

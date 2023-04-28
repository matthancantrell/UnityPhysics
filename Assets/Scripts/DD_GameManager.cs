using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DD_GameManager : MonoBehaviour
{
    [Header("Menus & Screens")]
    [SerializeField] GameObject StartScreen;
    [SerializeField] GameObject GameOverScreen;
    [SerializeField] GameObject WinScreen;
    [Header("Audio")]
    [SerializeField] AudioSource StartMusic;
    [SerializeField] GameObject SM;
    [Header("Other")]
    [SerializeField] bool isPaused;
    [SerializeField] bool winGame;
    [SerializeField] ControllerCharacter2D Player;
    [SerializeField] State CurrentState;
    [SerializeField] TextMeshProUGUI HP_Text;
    [SerializeField] DD_ColliderThing Win;
    enum State
    {
        START, PLAY, PAUSE, WIN, GAMEOVER
    }

    void Start()
    {
        CurrentState = State.START;
        StartMusic.Stop();
        winGame = false;
    }

    void Update()
    {
        if (Player.health <= 0)
        {
            StartMusic.Stop();
            CurrentState = State.GAMEOVER;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartMusic.Stop();
            CurrentState = State.WIN;
        }

        switch(CurrentState)
        {
            case State.START:
            {
                Time.timeScale = 0;
                StartScreen.SetActive(true);
                HP_Text.SetText("");
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
                HP_Text.SetText("HP: " + Player.health);

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

            case State.WIN:
            {
                Time.timeScale = 0;
                HP_Text.SetText("");
                WinScreen.SetActive(true);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                break;
            }

            case State.GAMEOVER:
            {
                Time.timeScale = 0;
                HP_Text.SetText("");
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

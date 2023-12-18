using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    AudioSource[] soundEffects;

    public GameObject m_camera;
    public Unit playerNexus;
    public Unit aiNexus;

    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject pauseMenu;
    bool pauseMenuActive = false;
    bool gameEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        soundEffects = GetComponents<AudioSource>();
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        loseScreen.SetActive(false);
        winScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // open and close the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseResumeGame();
        }

        if (playerNexus.currentHealth <= 0 && !gameEnded)
        {
            StartCoroutine(LossFocus());
        }
        else if (aiNexus.currentHealth <= 0 && !gameEnded)
        {
            StartCoroutine(WinFocus());
        }
    }

    public void PauseResumeGame()
    {
        pauseMenu.SetActive(!pauseMenuActive);
        pauseMenuActive = !pauseMenuActive;
        // pause and unpause time
        if (pauseMenuActive)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    IEnumerator WinFocus()
    {
        gameEnded = true;
        m_camera.transform.position = new Vector3(aiNexus.position.x, aiNexus.position.y, -10f);
        yield return new WaitForSeconds(3);
        Time.timeScale = 0;
        winScreen.SetActive(true);
        soundEffects[0].Play();
    }

    IEnumerator LossFocus()
    {
        gameEnded = true;
        m_camera.transform.position = new Vector3(playerNexus.position.x, playerNexus.position.y, -10f);
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0;
        loseScreen.SetActive(true);
        soundEffects[1].Play();
    }
}

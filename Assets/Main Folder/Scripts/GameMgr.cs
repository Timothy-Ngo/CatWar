using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public Unit playerNexus;
    public Unit aiNexus;

    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject pauseMenu;
    bool pauseMenuActive = false;

    // Start is called before the first frame update
    void Start()
    {
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

        if (playerNexus.currentHealth <= 0)
        {
            Time.timeScale = 0;
            loseScreen.SetActive(true);
        }
        else if (aiNexus.currentHealth <= 0)
        {
            Time.timeScale = 0;
            winScreen.SetActive(true);
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
}

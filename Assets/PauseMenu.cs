using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    HealthBar healthbar;

    private void Start()
    {
        healthbar = FindObjectOfType<HealthBar>();
    }

    public void ShowPauseMenu()
    {
        Time.timeScale = 0f;
        healthbar.gameObject.SetActive(false);
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        healthbar.gameObject.SetActive(true);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("CharacterSelection");
    }
}

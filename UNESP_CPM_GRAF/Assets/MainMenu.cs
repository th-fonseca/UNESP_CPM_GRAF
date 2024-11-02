using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject tutorialMenu;
    public GameObject levelSelectMenu;

    void Start()
    {
        tutorialMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
    }

    public void PlayGame()
    {
        levelSelectMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void TutorialGame()
    {
        mainMenu.SetActive(false);
        tutorialMenu.SetActive(true);
    }

    public void GoBack()
    {
        mainMenu.SetActive(true);
        levelSelectMenu.SetActive(false);
    }

    public void QuitGame()
    {
       Application.Quit();
    }
}

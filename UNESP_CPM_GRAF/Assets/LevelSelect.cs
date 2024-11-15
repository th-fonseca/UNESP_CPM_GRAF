using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public AudioClip stageOneClip;
    public AudioClip stageTwoClip;
    public AudioClip stageThreeClip;
    public void selectFirstLevel()
    {
        if(MusicManager.Instance) MusicManager.Instance.SetMainMusic(stageOneClip);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void selectSecondLevel()
    {
        if (MusicManager.Instance) MusicManager.Instance.SetMainMusic(stageTwoClip);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void selectThirdLevel()
    {
        if (MusicManager.Instance) MusicManager.Instance.SetMainMusic(stageThreeClip);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int mainSceneIndex;

    public void NewGame()
    {
        SceneManager.LoadScene(mainSceneIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

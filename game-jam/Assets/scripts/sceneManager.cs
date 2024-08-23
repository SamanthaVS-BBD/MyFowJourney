using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    public void startGame(){
        SceneManager.LoadScene("game");
    }

    public void goToMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }

    public void closeGame(){
        Application.Quit();
    }
}

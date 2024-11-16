using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public void LevelSelecscene()
    {
        SceneManager.LoadScene("Level1");
    }
    
    public void LoadInfo()
    {
        SceneManager.LoadScene("Info");
    }
    
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}


﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private static MenuController Instance;

    private void Start()
    {
        Instance = this;
    }

    public static MenuController GetInstance()
    {
        if (Instance == null)
        {
            GameObject menuController = new GameObject();
            Instance = menuController.AddComponent<MenuController>();
        }

        return Instance;
    }
    
    public void GoToMainMenu()
    {
        GoToScene(0);
    }
    
    public void WinScene()
    {
        GoToScene(1);
    }
    
    public void LooseScene()
    {
        GoToScene(2);
    }
    
    public void PauseScene()
    {
        GoToScene(3);
    }
    
    public void StoryScene()
    {
        GoToScene(4);
    }
    
    public void HowToScene()
    {
        GoToScene(5);
    }

    public void StartGame()
    {
        GoToScene(StaticData.FirstLevel);
    }
    
    public void ReloadScene()
    {
        GoToScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RetryCurrentLevel()
    {
        GoToScene(StaticData.CurrentLevel);
    }

    public void GoToNextLevel()
    {
        StaticData.NextLevel();
        GoToScene(StaticData.CurrentLevel);
    }

    public void GoToScene(int scene)
    {
        if (scene >= SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0);
            return;
        }
        
        StaticData.TotalEnemies = 0;
        StaticData.KilledEnemies = 0;
        SceneManager.LoadScene(scene);
    }
}

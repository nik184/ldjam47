﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private static MenuController Instance;

    public static bool Paused { get; private set; } = false;
    private PanelPause _pausePanel;

    private void Start()
    {
        Instance = this;
        _pausePanel = FindObjectOfType<PanelPause>();
        Debug.Log(_pausePanel);
        if (_pausePanel != null)
        {
            if (_pausePanel.gameObject.activeSelf) _pausePanel.gameObject.SetActive(false);
        }
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
    
    public void Exit()
    {
        Application.Quit();
    }
    
    public void WinScene()
    {
        GoToScene(1);
    }
    
    public void LooseScene()
    {
        GoToScene(2);
    }
    public void StoryScene()
    {
        GoToScene(3);
    }
    
    public void HowToScene()
    {
        GoToScene(4);
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

    public void OpenLicense()
    {
        Application.OpenURL("https://docs.google.com/document/d/13mYiTxrxDsReZA8H1S_di1-a2qlTlz7WAtl25_dOvJE/edit#");
    }

    public void OpenGamePage()
    {
        Application.OpenURL("https://ldjam.com/events/ludum-dare/47/$216762");
    }

    public void GoToScene(int scene)
    {
        if (scene >= SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0);
            return;
        }

        Paused = false;
        Time.timeScale = 1;
        StaticData.TotalEnemies = 0;
        StaticData.KilledEnemies = 0;
        SceneManager.LoadScene(scene);
    }

    public void Pause()
    {
        Paused = !Paused;
        Time.timeScale = MenuController.Paused ? 0 : 1;
        if (_pausePanel != null)
        {
            _pausePanel.gameObject.SetActive(!_pausePanel.gameObject.activeSelf);
        }
    }
}

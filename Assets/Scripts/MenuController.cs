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

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }
}

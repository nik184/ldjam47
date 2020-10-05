using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    public const int FirstLevel = 5;

    public static int CurrentLevel = FirstLevel;
    
    public static int TotalEnemies;
    
    public static int KilledEnemies = 0;
    
    
    public static void IncrementScore()
    {
        KilledEnemies++;
    }
    
    public static void NextLevel()
    {
        CurrentLevel++;
    }
}

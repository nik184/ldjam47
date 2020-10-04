using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{


    private Text text;
    
    private void Start()
    {
        StaticData.TotalEnemies = FindObjectsOfType<EnemyController>().Length;
        text = GetComponent<Text>();
        RedrawScore();
    }


    public void RedrawScore()
    {
        text.text = StaticData.KilledEnemies + " / " + StaticData.TotalEnemies;
    }
}

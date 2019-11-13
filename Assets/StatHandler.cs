using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class StatHandler : MonoBehaviour
{
    public static StatHandler instance;
    
    internal Dictionary<string, float> gameOverStat; 

    public Text title;
    public Text value;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Init();
    }
    
    private void Init()
    {
        gameOverStat = new Dictionary<string, float>
        {
            {"BEST SCORE", 0},
            {"TIME", 0},
            {"MAX COMBO", 0},
            {"WITHOUT TOUCH", 0},
            {"TOTAL SCORE", 0},
            {"COLLECTED POINTS", 0},
            {"MISSED POINTS", 0}
        };

    }

    internal void ResetStats()
    {
        //gameOverStat["BEST SCORE"] = 0;
        gameOverStat["TIME"] = 0;
        gameOverStat["MAX COMBO"] = 0;
        gameOverStat["WITHOUT TOUCH"] = 0;
        //gameOverStat["TOTAL SCORE"] = 0;
        gameOverStat["COLLECTED POINTS"] = 0;
        gameOverStat["MISSED POINTS"] = 0;
    }
    internal void VisuallyUpdateStat()
    {
        title.text = "";
        value.text = "";
        foreach (var pair in gameOverStat)
        {
            title.text += pair.Key + ":\n";
            value.text += System.Math.Round(pair.Value,1) + "\n";
        }
    }

    
    internal void SetStat(string key, float value, bool checkRecord,  bool cumulative = false)
    {   
        if (!checkRecord)
        {
            if (cumulative)
            {
                gameOverStat[key] += value;
                return;
            }
        }

        else if (value < gameOverStat[key])
        {
            return;
        }
            
        gameOverStat[key] = value;
    }
    
}

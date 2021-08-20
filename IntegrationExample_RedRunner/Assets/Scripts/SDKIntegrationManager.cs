using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKIntegrationManager : MonoBehaviour
{
    public static SDKIntegrationManager Instance { get; private set; }

    void Start()
    {
        Instance = this;
    }

    public void CoinCollected(int newCoinValue)
    {
        //Get CRS20 token rewards after
        //Getting 20\50\150\500 coins or
        //after setting highest score of
        //20 / 40 / 70 / 100 / 200m!

        // TODO check if equals to X value
        Debug.Log("New coin value" + newCoinValue);
    }

    public void HighScoreSet(int newHighScore)
    {
        Debug.Log("New high score" + newHighScore);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  
    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        TowerController.BuildTowers();
    } 

    private TowerController _towerController;
    private TowerController TowerController
    {
        get
        {
            if (_towerController == null)
                _towerController = FindObjectOfType<TowerController>();
            if (_towerController == null)
                Debug.LogError("No TowerController found in scene!");
            return _towerController;
        }
    }
}

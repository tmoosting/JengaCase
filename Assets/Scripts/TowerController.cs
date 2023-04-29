using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TowerController : MonoBehaviour
{

    public Tower towerOne;
    public Tower towerTwo;
    public Tower towerThree;

    public void BuildTowers()
    {
        TowerBuilder.BuildTowers();
    }
    public void RebuildTowers()
    {
        DestroyTowerBlocks(towerOne);   
        DestroyTowerBlocks(towerTwo);   
        DestroyTowerBlocks(towerThree);   
        TowerBuilder.BuildTowers();
    }


    public void DestroyBlock(Block block)
    {
        block.ownerTower.towerBlocks.Remove(block);
        Destroy(block.gameObject);
        UIManager.ResetBlockSelection();
    }
 

    public void EnableTowerPhysics()
    {
        foreach (Block block in towerOne.towerBlocks)
        {
            Rigidbody rb = block.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = false;
        }
        foreach (Block block in towerTwo.towerBlocks)
        {
            Rigidbody rb = block.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = false;
        }
        foreach (Block block in towerThree.towerBlocks)
        {
            Rigidbody rb = block.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = false;
        }
    }
    
    private void DestroyTowerBlocks(Tower tower)
    {
        List<Block> blocksToDestroy = new List<Block>(tower.towerBlocks);
        foreach (Block block in blocksToDestroy)
            if (block != null)
                Destroy(block.gameObject);
        tower.towerBlocks.Clear();
    }
    
    private UIManager _uiManager;
    private UIManager UIManager
    {
        get
        {
            if (_uiManager == null)
                _uiManager = FindObjectOfType<UIManager>();
            if (_uiManager == null)
                Debug.LogError("No TowerBuilder found in scene!");
            return _uiManager;
        }
    }
    private TowerBuilder _towerBuilder;
    private TowerBuilder TowerBuilder
    {
        get
        {
            if (_towerBuilder == null)
                _towerBuilder = FindObjectOfType<TowerBuilder>();
            if (_towerBuilder == null)
                Debug.LogError("No TowerBuilder found in scene!");
            return _towerBuilder;
        }
    }
}

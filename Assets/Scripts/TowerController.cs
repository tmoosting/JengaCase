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
    
    #region UICalls
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
        StartCoroutine(EnableTowerPhysicsWithConstraints(towerOne));
        StartCoroutine(EnableTowerPhysicsWithConstraints(towerTwo));
        StartCoroutine(EnableTowerPhysicsWithConstraints(towerThree));

    }
    private IEnumerator EnableTowerPhysicsWithConstraints(Tower tower)
    {
        foreach (Block block in tower.towerBlocks)
        {
            Rigidbody rb = block.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation; // Lock rotation
                rb.constraints |= RigidbodyConstraints.FreezePositionX; // Lock position in the X axis
                rb.constraints |= RigidbodyConstraints.FreezePositionZ; // Lock position in the Z axis
                rb.isKinematic = false;
            }
        }
        yield return new WaitForSeconds(TowerBuilder.constraintDelay);

        foreach (Block block in tower.towerBlocks)
        {
            Rigidbody rb = block.GetComponent<Rigidbody>();
            if (rb != null)
                rb.constraints = RigidbodyConstraints.None;
        }
    }
 

    public void DestroyGlassBlocks()
    {
        DestroyTowerGlassBlocks(towerOne);   
        DestroyTowerGlassBlocks(towerTwo);   
        DestroyTowerGlassBlocks(towerThree);   
    }
    
    #endregion
    private void DestroyTowerGlassBlocks(Tower tower)
    {
        List<Block> blocksToDestroy = new List<Block>(tower.towerBlocks);
        foreach (Block block in blocksToDestroy)
            if (block != null)
                if (block.blockData.mastery == 0)
                {
                    tower.towerBlocks.Remove(block);
                    Destroy(block.gameObject);
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

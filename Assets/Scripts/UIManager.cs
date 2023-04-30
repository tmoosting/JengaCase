using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI blockInfoText;
    public TextMeshProUGUI towerInfoText;
    public Button blockRemoveButton;

    private Block _clickedBlock;

    private void Start()
    {
        ResetBlockSelection();
    }

    public void ClickBlock(Block block)
    {
        _clickedBlock = block;
        string blockStr = "";
        blockStr += block.blockData.grade + ": " + block.blockData.domain;
        blockStr += "\n\n" + block.blockData.cluster;
        blockStr += "\n\n" + block.blockData.standardid + ": " + block.blockData.standarddescription;

        blockInfoText.text = blockStr;
        towerInfoText.text = "Tower: " + block.ownerTower.gameObject.name;
        
        blockRemoveButton.gameObject.SetActive(true);
    }

 
    public void ClickDestroyBlockButton()
    {
        if (_clickedBlock != null)
            TowerController.DestroyBlock(_clickedBlock);
    }

    public void ResetBlockSelection()
    {
        _clickedBlock = null;
        blockInfoText.text = "No Block Selected";
        towerInfoText.text = "No Tower Selected";
        blockRemoveButton.gameObject.SetActive(false); 
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

    public void ClickDeleteBlock(Block block)
    {
        Debug.Log("del");
        TowerController.DestroyBlock(block);
    }
}

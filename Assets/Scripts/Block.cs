using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
   public Tower ownerTower;
   public BlockData blockData;

   private void OnMouseDown()
   {
      if (Input.GetMouseButtonDown(0))
         UIManager.ClickBlock(this); 
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
}

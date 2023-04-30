using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TowerBuilder : MonoBehaviour
{
    
    public GameObject glassBlockPrefab;
    public GameObject woodBlockPrefab;
    public GameObject stoneBlockPrefab;
    public GameObject labelPrefab; 
 
    private BlockData[] blocksData; 
    
    public Vector3 blockSize = new Vector3(0.075f, 0.015f, 0.025f); // based on actual jenga block dimensions: 1.5 cm × 2.5 cm × 7.5 cm
    public float blockSpacingY = 0.0025f; // Spacing between blocks in meters (0.25 cm) along the Y-axis
    public float blockSpacingXZ = 0.0025f; // Spacing between blocks in meters (0.25 cm) along the X and Z axes

    public  float minScaleFactor = 0.98f; // for random variations / imperfections
    public  float maxScaleFactor = 1.02f;
    public float constraintDelay; //to mitigate initial bounce on physics delay; in seconds
    
    private  List<BlockData> _grade1StackData = new List<BlockData>();
    private  List<BlockData> _grade2StackData = new List<BlockData>();
    private  List<BlockData> _grade3StackData = new List<BlockData>();


    public delegate void StackDataCompleteHandler();
    public event StackDataCompleteHandler OnStackDataComplete;
    
    public void BuildTowers()
    {
     
        // set up delegate to call when data fetch is complete
        OnStackDataComplete -= TowerConstruction;
        OnStackDataComplete += TowerConstruction;
        
        StartCoroutine(FetchStackData("https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack"));
    }
 
    private void TowerConstruction()
    {
        OrganizeStackData(blocksData);
        CalculateLayersPerTower();
        CreateTowers();
    }
    private void CalculateLayersPerTower()
    {
        int blocksPerLayer = 3;

        int grade1Layers = Mathf.CeilToInt(_grade1StackData.Count / (float)blocksPerLayer);
        int grade2Layers = Mathf.CeilToInt(_grade2StackData.Count / (float)blocksPerLayer);
        int grade3Layers = Mathf.CeilToInt(_grade3StackData.Count / (float)blocksPerLayer);

        TowerController.towerOne.totalLayers = grade1Layers;
        TowerController.towerTwo.totalLayers = grade2Layers;
        TowerController.towerThree.totalLayers = grade3Layers;
    }
    private void CreateTowers()
    {
         CreateJengaTower(TowerController.towerOne, _grade1StackData);
         CreateJengaTower(TowerController.towerTwo, _grade2StackData);
         CreateJengaTower(TowerController.towerThree, _grade3StackData);  
    }
    private void CreateJengaTower(Tower tower, List<BlockData> stackDataList)
    {
        // workaround for allowing rescaling of towers 
        Vector3 originalScale = tower.transform.localScale;
        tower.transform.localScale = new Vector3(1, 1, 1); 
        
        
        // Create and position the blocks within the tower
        int blocksPerLayer = 3;
        int layers = tower.totalLayers; 

        Vector3 position = tower.transform.position;

        for (int i = 0; i < layers; i++)
        {
            bool horizontalLayer = i % 2 == 0;
            int startIndex = i * blocksPerLayer;
            BuildLayer(position, horizontalLayer, tower, stackDataList, startIndex);
            position.y += blockSize.y + blockSpacingY;
        }
     //   Debug.Log("Tower: " + tower.name + ", Layers: " + layers + ", Total Blocks: " + stackDataList.Count);
     tower.transform.localScale = originalScale;
    }

  void BuildLayer(Vector3 position, bool horizontal, Tower tower, List<BlockData> stackDataList, int startIndex)
  {
      int blockCount = 3;
      Vector3 offset;

      // Adjust the starting position for centering
      if (horizontal)
      {
          position.x -= (blockSize.z + blockSpacingXZ) * (blockCount - 1) / 2.0f;
      }
      else
      {
          position.z -= (blockSize.z + blockSpacingXZ) * (blockCount - 1) / 2.0f;
      }

      for (int i = 0; i < blockCount; i++)
      {
          if (startIndex + i >= stackDataList.Count) break; // Check if we've reached the end of the stackDataList

          BlockData currentBlockData = stackDataList[startIndex + i];
          GameObject blockPrefab = GetBlockPrefab(currentBlockData.mastery);
          GameObject blockObj = Instantiate(blockPrefab, position, Quaternion.identity, tower.transform);
          Block block = blockObj.AddComponent<Block>();
          block.ownerTower = tower;
          block.blockData = currentBlockData;
          block.GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
          
          // Apply random scale factors
          float randomXScaleFactor = Random.Range(minScaleFactor, maxScaleFactor);
          float randomYScaleFactor = Random.Range(minScaleFactor, maxScaleFactor);
          float randomZScaleFactor = Random.Range(minScaleFactor, maxScaleFactor);
          Vector3 randomBlockScale = new Vector3(blockSize.x * randomXScaleFactor, blockSize.y * randomYScaleFactor, blockSize.z * randomZScaleFactor);
          blockObj.transform.localScale = randomBlockScale; 

          if (horizontal)
          {
              offset = new Vector3(blockSize.z + blockSpacingXZ, 0, 0);
              position += offset;
              blockObj.transform.localRotation = Quaternion.Euler(0, 90, 0);
          }
          else
          {
              offset = new Vector3(0, 0, blockSize.z + blockSpacingXZ);
              position += offset;
              blockObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
          }
        
          tower.towerBlocks.Add(block);
      }
  } 
    private GameObject GetBlockPrefab(int mastery)
    {
        switch (mastery)
        {
            case 0:
                return glassBlockPrefab;
            case 1:
                return woodBlockPrefab;
            case 2:
                return stoneBlockPrefab;
            default:
                return null;
        }
    }

    void OrganizeStackData(BlockData[] stacksData)
    { 
        _grade1StackData = new List<BlockData>();
        _grade2StackData = new List<BlockData>();
        _grade3StackData = new List<BlockData>();

        foreach (BlockData stackData in stacksData)
        {
            if (stackData.grade == "6th Grade") _grade1StackData.Add(stackData);
            else if (stackData.grade == "7th Grade") _grade2StackData.Add(stackData);
            else if (stackData.grade == "8th Grade") _grade3StackData.Add(stackData);
        } 
        
        SortStackData(_grade1StackData);
        SortStackData(_grade2StackData);
        SortStackData(_grade3StackData);
    }
    private void SortStackData(List<BlockData> stackData)
    {
        // Sort the stack data list based on domain, cluster, and standard ID
        stackData.Sort((a, b) =>
        {
            int domainComparison = a.domain.CompareTo(b.domain);
            if (domainComparison != 0) return domainComparison;

            int clusterComparison = a.cluster.CompareTo(b.cluster);
            if (clusterComparison != 0) return clusterComparison;

            return a.standardid.CompareTo(b.standardid);
        });
    }
    

    private IEnumerator FetchStackData(string apiUrl)
    { 
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            // Wrap the JSON array and deserialize JSON data
            string wrappedJson = WrapJsonArray(jsonResponse);
            BlockDataWrapper blockDataWrapper = JsonUtility.FromJson<BlockDataWrapper>(wrappedJson);
            blocksData = blockDataWrapper.blockDatas;
            OnStackDataComplete?.Invoke();
        }
        else
        {
            Debug.LogError("API request failed: " + request.error);
        } 
    }
    private string WrapJsonArray(string jsonArray)
    {
        return "{\"blockDatas\":" + jsonArray + "}";
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class BlockData
{
    public int id;
    public string subject;
    public string grade;
    public int mastery;
    public string domainid;
    public string domain;
    public string cluster;
    public string standardid;
    public string standarddescription;
}

[System.Serializable]
public class BlockDataWrapper
{
    public BlockData[] blockDatas;
}
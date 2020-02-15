using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageMapData"
    , menuName = "Create Stage Map Data"
    , order = 1)]
public class StageMapData : ScriptableObject
{
    public List<StageData> stageList = new List<StageData>();
}

[System.Serializable]
public class StageData
{
    public List<MapData> mapList = new List<MapData>();
}

[System.Serializable]
public class MapData
{
    public int MonsterRate = 50;
    public int ItemBoxRate = 50;
    public int IgnoreRate = 0;
    public string fixedItemBox_Id;
}

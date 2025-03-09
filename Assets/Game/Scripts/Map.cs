using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMap", menuName = "Map/Grid")]
public class Map : ScriptableObject
{
    public int width = 10;
    public int height = 10;
    public List<MapWrapper> mapList = new List<MapWrapper>();

    public void FitMaplist()
    {
        foreach (var item in mapList)
        {
            if (item.x >= width || item.y >= height)
            {
                mapList.Remove(item);
            }
        } 
    }
}
[System.Serializable]
public class MapWrapper
{
    public int x;
    public int y;
    public bool value;
}

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMap", menuName = "Map/Grid")]
public class Map : ScriptableObject
{
    public int width = 10;
    public int height = 10;
    public List<MapPiece> mapList = new List<MapPiece>();

    public void FitMaplist()
    {
        // Remove items with invalid coordinates
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
public class MapPiece
{
    public int x;
    public int y;
    public ItemType value;  // Use TileType enum instead of bool
}

public enum ItemType  // Enum for tile types
{
    E,  // Empty
    W,  // Wall
    YP, // Yellow Portal
    YH, // Yellow Head
    Y,  // Yellow Body
    BP, // Blue Portal
    BH, // Blue Head
    B,  // Blue Body
    OP, // Orange Portal
    OH, // Orange Head
    O,  // Orange Body
    PP, // Pink Portal
    PH, // Pink Head
    P   // Pink Body
}


using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    Map map;
    public static MapController Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    public void SetMap(Map map)
    {
        this.map = map;
    }
    public List<MapPiece> FindMoveables(MapPiece piece)
    {
        var index = map.mapList.FindIndex(p => p == piece);
        List<MapPiece> pieces = new();

        if (piece.x != 0 && map.mapList[index - 1].value != ItemType.W)
            pieces.Add(map.mapList[index - 1]);
        if (piece.x != map.width - 1 && map.mapList[index + 1].value != ItemType.W)
            pieces.Add(map.mapList[index + 1]);
        if (piece.y != 0 && map.mapList[index - map.width].value != ItemType.W)
            pieces.Add(map.mapList[index - map.width]);
        if (piece.y != map.height - 1 && map.mapList[index + map.width].value != ItemType.W)
            pieces.Add(map.mapList[index + map.width]);

        return pieces;
    }

}
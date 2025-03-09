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
    public List<MapPiece> FindMoveables(MapPiece piece, CharacterType type)
    {
        var index = map.mapList.FindIndex(p => p == piece);
        List<MapPiece> pieces = new();

        if (piece.x != 0)
        {
            var targetPiece = map.mapList[index - 1];
            if (CheckPiece(targetPiece, type)) pieces.Add(targetPiece);
        }
        if (piece.x != map.width - 1)
        {
            var targetPiece = map.mapList[index + 1];
            if (CheckPiece(targetPiece, type)) pieces.Add(targetPiece);
        }
        if (piece.y != 0)
        {
            var targetPiece = map.mapList[index - map.width];
            if (CheckPiece(targetPiece, type)) pieces.Add(targetPiece);
        }
        if (piece.y != map.height - 1)
        {
            var targetPiece = map.mapList[index + map.width];
            if (CheckPiece(targetPiece, type)) pieces.Add(targetPiece);
        }

        return pieces;
    }
    bool CheckPiece(MapPiece piece, CharacterType type)
    {
        if (piece.value == ItemType.W)
            return false;
        if (piece.value == ItemType.YP && type != CharacterType.Yellow)
            return false;
        if (piece.value == ItemType.OP && type != CharacterType.Orange)
            return false;
        if (piece.value == ItemType.BP && type != CharacterType.Blue)
            return false;
        if (piece.value == ItemType.PP && type != CharacterType.Pink)
            return false;
        return true;
    }
}
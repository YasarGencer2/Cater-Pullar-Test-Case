using System;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    public static MapCreator Instance { get; private set; }

    [SerializeField] Map map;
    [SerializeField] MapItem mapItem;
    [SerializeField] Transform gridParent, characterParent;
    [SerializeField] Vector3 center = new Vector3(0, 0, 0);
    [SerializeField] float distanceBetweenGridPieces = 1;
    [SerializeField] float padding = 1f;
    Vector3 gridCenter;
    void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        DestroyMap();
        CreateMap();
        AdjustCamera();
    }

    public void DestroyMap()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }
    }

    public void CreateMap()
    {
        gridCenter = new Vector3((map.width - 1) * 0.5f * distanceBetweenGridPieces,
                                          (map.height - 1) * 0.5f * distanceBetweenGridPieces,
                                          0);

        foreach (var item in map.mapList)
            Piece(item);
        MapController.Instance.SetMap(map);
    }
    void Piece(MapWrapper piece)
    {

        if (piece.value == ItemType.W)  // Check if the value is Wall
        {
            var obj = Instantiate(mapItem.wall, PieceToPosition(piece), Quaternion.identity, gridParent);
            obj.name = "Wall Piece (" + piece.x + ", " + piece.y + ")";
        }
        else
        {
            var obj = Instantiate(mapItem.grid, PieceToPosition(piece), Quaternion.identity, gridParent);
            obj.name = "Grid Piece (" + piece.x + ", " + piece.y + ")";

            if (piece.value == ItemType.YP) Portal(piece, mapItem.yellow);
            else if (piece.value == ItemType.BP) Portal(piece, mapItem.blue);
            else if (piece.value == ItemType.OP) Portal(piece, mapItem.orange);
            else if (piece.value == ItemType.PP) Portal(piece, mapItem.pink);

            else if (piece.value == ItemType.YH) Head(piece, ItemType.Y, mapItem.yellow);
            else if (piece.value == ItemType.BH) Head(piece, ItemType.B, mapItem.blue);
            else if (piece.value == ItemType.OH) Head(piece, ItemType.O, mapItem.orange);
            else if (piece.value == ItemType.PH) Head(piece, ItemType.P, mapItem.pink);
        }
    }
    void Portal(MapWrapper piece, Material mat)
    {
        var obj2 = Instantiate(mapItem.portal, PieceToPositionCharacters(piece), Quaternion.identity, characterParent);
        obj2.GetComponent<Renderer>().material = mat;
    }
    void Head(MapWrapper piece, ItemType type, Material mat)
    {
        var obj2 = Instantiate(mapItem.head, PieceToPositionCharacters(piece), Quaternion.identity, characterParent);
        obj2.GetComponent<Renderer>().material = mat;

        var charType = CharacterType.Yellow;
        if (type == ItemType.B) charType = CharacterType.Blue;
        else if (type == ItemType.O) charType = CharacterType.Orange;
        else if (type == ItemType.P) charType = CharacterType.Pink;
        var character = obj2.GetComponent<Head>();
        character.CharacterType = charType;

        character.SetPiece(piece);

        CharacterList.Instance.AddCharacter(character);
        var bodies = new List<string>();
        FindBodies(bodies, charType, type, piece, mat);
        CharacterList.Instance.SetTail(charType);
    }

    private void FindBodies(List<string> bodies, CharacterType charType, ItemType type, MapWrapper piece, Material mat)
    {
        int[] directionsX = { -1, 1, 0, 0 };
        int[] directionsY = { 0, 0, 1, -1 };

        for (int i = 0; i < 4; i++)
        {
            int newX = piece.x + directionsX[i];
            int newY = piece.y + directionsY[i];

            if (newX < 0 || newX >= map.width || newY < 0 || newY >= map.height) continue;

            var nextPiece = map.mapList.Find(item => item.x == newX && item.y == newY);
            if (nextPiece != null && nextPiece.value == type)
            {
                var name = type + " Body (" + newX + ", " + newY + ")";
                if (bodies.Contains(name)) continue;

                var obj2 = Instantiate(mapItem.body, PieceToPositionCharacters(nextPiece), Quaternion.identity, characterParent);
                obj2.name = name;
                obj2.GetComponent<Renderer>().material = mat;

                var body = obj2.GetComponent<Body>();
                body.CharacterType = charType;
                CharacterList.Instance.AddBody(body);
                bodies.Add(name);
                body.SetPiece(nextPiece);
                FindBodies(bodies, charType, type, nextPiece, mat);
            }
        }
    }



    void AdjustCamera()
    {
        if (Camera.main != null)
        {
            float width = map.width * distanceBetweenGridPieces + padding * 2;
            float height = map.height * distanceBetweenGridPieces + padding * 2;

            float horizontalRatio = width / Camera.main.aspect;
            float verticalRatio = height;
            Camera.main.orthographicSize = Mathf.Max(horizontalRatio, verticalRatio) * 0.5f;
        }
    }
    public Vector3 PieceToPosition(MapWrapper piece)
    {
        Vector3 position = new Vector3(piece.x * distanceBetweenGridPieces, piece.y * distanceBetweenGridPieces, 0);
        position = position - gridCenter + center;
        return position;
    }
    public Vector3 PieceToPositionCharacters(MapWrapper piece)
    {
        return PieceToPosition(piece) + Vector3.forward * -5f;
    }
}

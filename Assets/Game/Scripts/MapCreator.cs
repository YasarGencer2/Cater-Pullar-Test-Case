using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    public static MapCreator Instance { get; private set; }

    public Map map;
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

    public void DestroyMap()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }
    }

    public void CreateMap(Map map)
    {
        this.map = map;
        DestroyMap();
        AdjustCamera();

        gridCenter = new Vector3((map.width - 1) * 0.5f * distanceBetweenGridPieces,
                                          (map.height - 1) * 0.5f * distanceBetweenGridPieces,
                                          0);

        StartCoroutine(LocalDelay());
        IEnumerator LocalDelay()
        {
            foreach (var item in map.mapList)
            {
                Piece(item);
                yield return new WaitForSeconds(0.02f);
            }
            MapController.Instance.SetMap(map);
        }
    }
    void Piece(MapPiece piece)
    {

        if (piece.value == ItemType.W)  // Check if the value is Wall
        {
            var obj = Instantiate(mapItem.wall, PieceToPosition(piece), Quaternion.identity, gridParent);
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(0.8f, 0.2f);
            obj.name = "Wall Piece (" + piece.x + ", " + piece.y + ")";
        }
        else
        {
            var obj = Instantiate(mapItem.grid, PieceToPosition(piece), Quaternion.identity, gridParent);
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(1, 0.2f);
            obj.name = "Grid Piece (" + piece.x + ", " + piece.y + ")";

            if (piece.value == ItemType.YP) Portal(piece, CharacterType.Yellow, mapItem.yellow);
            else if (piece.value == ItemType.BP) Portal(piece, CharacterType.Blue, mapItem.blue);
            else if (piece.value == ItemType.OP) Portal(piece, CharacterType.Orange, mapItem.orange);
            else if (piece.value == ItemType.PP) Portal(piece, CharacterType.Pink, mapItem.pink);

            else if (piece.value == ItemType.YH) Head(piece, ItemType.Y, mapItem.yellow);
            else if (piece.value == ItemType.BH) Head(piece, ItemType.B, mapItem.blue);
            else if (piece.value == ItemType.OH) Head(piece, ItemType.O, mapItem.orange);
            else if (piece.value == ItemType.PH) Head(piece, ItemType.P, mapItem.pink);
        }
    }
    void Portal(MapPiece piece, CharacterType type, Material mat)
    {
        var obj = Instantiate(mapItem.portal, PieceToPositionCharacters(piece), Quaternion.identity, characterParent);
        var portal = obj.GetComponent<Portal>();
        portal.piece = piece;
        portal.type = type;
        portal.SetMaterial(mat);
    }
    void Head(MapPiece piece, ItemType type, Material mat)
    {
        var obk = Instantiate(mapItem.head, PieceToPositionCharacters(piece), Quaternion.identity, characterParent);
        obk.GetComponent<Renderer>().material = mat;

        var charType = CharacterType.Yellow;
        if (type == ItemType.B) charType = CharacterType.Blue;
        else if (type == ItemType.O) charType = CharacterType.Orange;
        else if (type == ItemType.P) charType = CharacterType.Pink;
        var character = obk.GetComponent<Head>();
        character.CharacterType = charType;

        character.SetPiece(piece);

        CharacterList.Instance.AddCharacter(character);
        var bodies = new List<string>();
        FindBodies(bodies, charType, type, piece, mat);
        CharacterList.Instance.SetTail(charType);
    }

    private void FindBodies(List<string> bodies, CharacterType charType, ItemType type, MapPiece piece, Material mat)
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

                var obj = Instantiate(mapItem.body, PieceToPositionCharacters(nextPiece), Quaternion.identity, characterParent);
                obj.name = name;
                obj.GetComponent<Renderer>().material = mat;

                var body = obj.GetComponent<Body>();
                body.CharacterType = charType;
                bodies.Add(name);
                body.SetPiece(nextPiece);
                CharacterList.Instance.AddBody(body);
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
    public Vector3 PieceToPosition(MapPiece piece)
    {
        Vector3 position = new Vector3(piece.x * distanceBetweenGridPieces, piece.y * distanceBetweenGridPieces, 0);
        position = position - gridCenter + center;
        return position;
    }
    public Vector3 PieceToPositionCharacters(MapPiece piece)
    {
        return PieceToPosition(piece) + Vector3.forward * -5f;
    }
    public Vector3 PieceToPositionPortals(MapPiece piece)
    {
        return PieceToPosition(piece) + Vector3.forward * -2.5f;
    }
}

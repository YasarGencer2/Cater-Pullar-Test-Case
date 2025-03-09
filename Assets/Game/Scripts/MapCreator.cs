using System;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField] Map map;
    [SerializeField] MapItem mapItem;
    [SerializeField] Transform gridParent, characterParent;
    [SerializeField] Vector3 center = new Vector3(0, 0, 0);
    [SerializeField] float distanceBetweenGridPieces = 1;
    [SerializeField] float padding = 1f;
    Vector3 gridCenter;
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
        {
            Piece(item);
        }
    }
    void Piece(MapWrapper item)
    {
        Vector3 position = new Vector3(item.x * distanceBetweenGridPieces, item.y * distanceBetweenGridPieces, 0);

        if (item.value == ItemType.W)  // Check if the value is Wall
        {
            var obj = Instantiate(mapItem.wall, position - gridCenter + center, Quaternion.identity, gridParent);
            obj.name = "Wall Piece (" + item.x + ", " + item.y + ")";
        }
        else
        {
            var obj = Instantiate(mapItem.grid, position - gridCenter + center, Quaternion.identity, gridParent);
            obj.name = "Grid Piece (" + item.x + ", " + item.y + ")";

            if (item.value == ItemType.YP) Portal(item, mapItem.yellow);
            else if (item.value == ItemType.BP) Portal(item, mapItem.blue);
            else if (item.value == ItemType.OP) Portal(item, mapItem.orange);
            else if (item.value == ItemType.PP) Portal(item, mapItem.pink);

            else if (item.value == ItemType.YH) Head(item, ItemType.Y, mapItem.yellow);
            else if (item.value == ItemType.BH) Head(item, ItemType.B, mapItem.blue);
            else if (item.value == ItemType.OH) Head(item, ItemType.O, mapItem.orange);
            else if (item.value == ItemType.PH) Head(item, ItemType.P, mapItem.pink);
        }
    }
    void Portal(MapWrapper item, Material mat)
    {
        Vector3 position = new Vector3(item.x * distanceBetweenGridPieces, item.y * distanceBetweenGridPieces, 0);
        var obj2 = Instantiate(mapItem.portal, position - gridCenter + center + Vector3.forward * -5f, Quaternion.identity, characterParent);
        obj2.GetComponent<Renderer>().material = mat;
    }
    void Head(MapWrapper item, ItemType type, Material mat)
    {
        Vector3 position = new Vector3(item.x * distanceBetweenGridPieces, item.y * distanceBetweenGridPieces, 0);
        var obj2 = Instantiate(mapItem.head, position - gridCenter + Vector3.forward * -5f + center, Quaternion.identity, characterParent);
        obj2.GetComponent<Renderer>().material = mat;

        var charType = CharacterType.Yellow; 
        if (type == ItemType.B) charType = CharacterType.Blue;
        else if (type == ItemType.O) charType = CharacterType.Orange;
        else if (type == ItemType.P) charType = CharacterType.Pink; 
        var character = obj2.GetComponent<Head>();
        character.CharacterType = charType;

        CharacterList.Instance.AddCharacter(character);
        var bodies = new List<string>();
        FindBodies(bodies, charType, type, item.x, item.y, mat);
        CharacterList.Instance.SetTail(charType);
    }

    private void FindBodies(List<string> bodies, CharacterType charType, ItemType type, int x, int y2, Material mat)
    {
        int[] directionsX = { -1, 1, 0, 0 };
        int[] directionsY = { 0, 0, 1, -1 };

        for (int i = 0; i < 4; i++)
        {
            int newX = x + directionsX[i];
            int newY = y2 + directionsY[i];

            if (newX < 0 || newX >= map.width || newY < 0 || newY >= map.height) continue;

            if (map.mapList.Find(item => item.x == newX && item.y == newY).value == type)
            {
                var name = type + " Body (" + newX + ", " + newY + ")";
                if (bodies.Contains(name)) continue;

                var obj2 = Instantiate(mapItem.body, new Vector3(newX * distanceBetweenGridPieces, newY * distanceBetweenGridPieces, 0) - gridCenter + center + Vector3.forward * -5f, Quaternion.identity, characterParent);
                obj2.name = name;
                obj2.GetComponent<Renderer>().material = mat;

                var body = obj2.GetComponent<Body>();
                body.CharacterType = charType;
                CharacterList.Instance.AddBody(body);
                bodies.Add(name);

                FindBodies(bodies, charType, type, newX, newY, mat);
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
}

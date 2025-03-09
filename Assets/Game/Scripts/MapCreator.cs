using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField] Map map;
    [SerializeField] Transform bg;
    [SerializeField] GameObject gridPiece, wallPiece;
    [SerializeField] Transform gridParent;
    [SerializeField] Vector3 center = new Vector3(0, 0, 0);
    [SerializeField] float distanceBetweenGridPieces = 1;
    [SerializeField] float padding = 1f;
    Vector3 gridCenter;
    void OnEnable()
    {
        DestroyMap();
        CreateMap();
        AdjustCamera();
        AdjustBG();
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
        if (!item.value == false)
        {
            var obj = Instantiate(wallPiece, position - gridCenter + center, Quaternion.identity, gridParent);
            obj.name = "Wall Piece (" + item.x + ", " + item.y + ")";
        }
        else
        {
            var obj = Instantiate(gridPiece, position - gridCenter + center, Quaternion.identity, gridParent);
            obj.name = "Grid Piece (" + item.x + ", " + item.y + ")";
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
    void AdjustBG()
    {
        if (bg != null)
        {
            bg.position = center + Vector3.forward;
            bg.localScale = new Vector3(map.width * distanceBetweenGridPieces * 1.05f, map.height * distanceBetweenGridPieces * 1.05f, 1);
        }
    }
}

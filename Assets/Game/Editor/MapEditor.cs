using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    private Map map;

    private void OnEnable()
    {
        map = (Map)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
 
        map.timeLimit = EditorGUILayout.FloatField("Time Limit", map.timeLimit); 
        map.width = EditorGUILayout.IntField("Width", map.width);
        map.height = EditorGUILayout.IntField("Height", map.height);
 
        ItemType[,] grid = new ItemType[map.width, map.height];
        map.FitMaplist();
 
        foreach (var wrapper in map.mapList)
        {
            grid[wrapper.x, wrapper.y] = wrapper.value;
        }
 
        for (int y = map.height - 1; y >= 0; y--) 
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < map.width; x++)
            {
                ItemType currentValue = grid[x, y];
                int selectedIndex = (int)currentValue;
 
                int newIndex = EditorGUILayout.Popup(selectedIndex, Enum.GetNames(typeof(ItemType)), GUILayout.Width(120));
 
                if (newIndex != selectedIndex)
                {
                    grid[x, y] = (ItemType)newIndex; 
                    UpdateMapList(x, y, grid[x, y]);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        { 
            UpdateMapListFromGrid(grid);
        }
 
        if (GUI.changed)
        {
            EditorUtility.SetDirty(map);
        }
    }

    private void UpdateMapList(int x, int y, ItemType value)
    { 
        MapPiece wrapper = map.mapList.Find(item => item.x == x && item.y == y);
        if (wrapper != null)
        {
            wrapper.value = value;
        }
        else
        { 
            map.mapList.Add(new MapPiece { x = x, y = y, value = value });
        }
    }

    private void UpdateMapListFromGrid(ItemType[,] grid)
    {
        map.mapList.Clear();
 
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                map.mapList.Add(new MapPiece { x = x, y = y, value = grid[x, y] });
            }
        }
    }
}

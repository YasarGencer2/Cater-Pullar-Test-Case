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

        // Display and update width and height
        map.width = EditorGUILayout.IntField("Width", map.width);
        map.height = EditorGUILayout.IntField("Height", map.height);

        // Initialize a 2D boolean grid
        bool[,] grid = new bool[map.width, map.height];
        map.FitMaplist();
        
        // Populate the grid based on mapList
        foreach (var wrapper in map.mapList)
        {
            grid[wrapper.x, wrapper.y] = wrapper.value;
        }

        // Draw grid as toggle buttons with bottom-left corner as (0,0)
        for (int y = map.height - 1; y >= 0; y--)  // Reverse the y-axis to have y = 0 at the bottom
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < map.width; x++)
            {
                bool currentValue = grid[x, y];
                bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(20));

                // Only update if the value changed
                if (newValue != currentValue)
                {
                    grid[x, y] = newValue;
                    // Update mapList to reflect changes
                    UpdateMapList(x, y, newValue);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            // Update the mapList based on the grid
            UpdateMapListFromGrid(grid);
        }

        // Mark the object as dirty to save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(map);
        }
    }

    private void UpdateMapList(int x, int y, bool value)
    {
        // Check if the coordinate exists in the mapList, and update it
        MapWrapper wrapper = map.mapList.Find(item => item.x == x && item.y == y);
        if (wrapper != null)
        {
            wrapper.value = value;
        }
        else
        {
            // Add new entry if it doesn't exist
            map.mapList.Add(new MapWrapper { x = x, y = y, value = value });
        }
    }
    private void UpdateMapListFromGrid(bool[,] grid)
    {
        map.mapList.Clear();

        // Populate mapList from the grid, including both true and false values
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                map.mapList.Add(new MapWrapper { x = x, y = y, value = grid[x, y] });
            }
        }
    }
}

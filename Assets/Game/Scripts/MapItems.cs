using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapItems", menuName = "MapItems")]
public class MapItem : ScriptableObject
{
    public GameObject grid;
    public GameObject wall;
    public GameObject portal;
    public GameObject head;
    public GameObject body;
    public GameObject tail;
    public Material yellow, blue, pink, orange;
}
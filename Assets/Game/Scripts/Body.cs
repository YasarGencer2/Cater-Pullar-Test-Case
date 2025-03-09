using System.Collections.Generic;
using UnityEngine;

public class Body : Moveable
{
    void OnDestroy()
    {
        Debug.Log("destroyed");
    }
}
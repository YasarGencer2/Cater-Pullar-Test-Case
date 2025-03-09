using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    public CharacterType CharacterType;
    public Head head;
    public List<Body> bodies = new();
    public Tail tail;


    public void SetHead(Head head)
    {
        this.head = head;
    }
    public void SetBodies(List<Body> bodies)
    {
        foreach (var item in bodies)
        {
            this.bodies.Add(item);
        }
    }
    public void SetTail(Tail tail)
    {
        this.tail = tail;
    }
    public void Pick()
    {
    }

    public void Release()
    {
    }
}
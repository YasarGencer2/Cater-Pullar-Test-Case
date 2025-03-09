using System;
using System.Collections.Generic;
using UnityEngine;

public class Head : Moveable
{
    void Start()
    {
        SetHead(this);
    }
    public void AddBody(Body body)
    {
        bodies.Add(body);
    }

    public void AddTail(GameObject tail)
    {
        var lastBody = bodies[bodies.Count - 1];
        bodies.RemoveAt(bodies.Count - 1);

        var obj = Instantiate(tail, lastBody.transform.position, Quaternion.identity, lastBody.transform.parent);
        obj.GetComponent<Renderer>().material = lastBody.GetComponent<Renderer>().material;
        this.tail = obj.GetComponent<Tail>();


        this.tail.SetHead(this);
        this.tail.SetBodies(bodies);
        this.tail.SetTail(this.tail);
        this.tail.CharacterType = CharacterType;
        this.tail.SetPiece(lastBody.piece);

        SetTail(this.tail);

        Destroy(lastBody.gameObject);
    }

}
public enum CharacterType
{
    Yellow,
    Blue,
    Orange,
    Pink
}

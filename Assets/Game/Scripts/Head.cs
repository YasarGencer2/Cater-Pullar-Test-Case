using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Head : Moveable
{
    void Start()
    {
        SetHead(this);
        transform.localScale = Vector3.zero;
        transform.DOScale(0.8f, 0.4f).SetDelay(.5f);
    }
    public void AddBody(Body body)
    {
        bodies.Add(body);
        if (bodies.Count == 1)
        {
            RotateAway(bodies[0].piece);
        }
    }

    public void AddTail(GameObject tail)
    {
        var lastBody = bodies[bodies.Count - 1];
        bodies.RemoveAt(bodies.Count - 1);

        var obj = Instantiate(tail, lastBody.transform.position, Quaternion.identity, lastBody.transform.parent);
        obj.GetComponent<Renderer>().material = lastBody.GetComponent<Renderer>().material;
        this.tail = obj.GetComponent<Tail>();


        this.tail.SetHead(this.tail);
        this.tail.SetBodies(bodies.AsEnumerable().Reverse().ToList());
        this.tail.SetTail(this);
        this.tail.CharacterType = CharacterType;
        this.tail.SetPiece(lastBody.piece);
        this.tail.Portal = Portal;

        SetTail(this.tail);
        this.tail.RotateAway(bodies[bodies.Count - 1].piece);

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

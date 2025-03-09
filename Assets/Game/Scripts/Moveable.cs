using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    public CharacterType CharacterType;
    public Head head;
    public List<Body> bodies = new();
    public Tail tail;

    public MapWrapper piece;
    public List<MapWrapper> moveables;
    public void SetHead(Head head) => this.head = head;
    public void SetBodies(List<Body> bodies) => this.bodies = bodies;
    public void SetTail(Tail tail) => this.tail = tail;
    public void SetPiece(MapWrapper piece) => this.piece = piece;
    public void Pick() => moveables = MapController.Instance.FindMoveables(piece);
    public void Hold(Vector3 position)
    {
        head.transform.position = position;
        var distance = Vector3.Distance(head.transform.position, MapCreator.Instance.PieceToPositionCharacters(head.piece));
        bodies[0].Move(this, this, distance);
        TrySettle(distance);
    }
    public void Release()
    {
        head.transform.position = MapCreator.Instance.PieceToPositionCharacters(piece);
        bodies[0].Move(this, this, 0);
    }
    protected void Move(Moveable head, Moveable front, float distance)
    {
        var bodies = head.bodies;
        // Debug.Log("Move " + this.name + " to " + front.name);
        var myPiecePos = MapCreator.Instance.PieceToPositionCharacters(this.piece);
        var frontPiecePos = MapCreator.Instance.PieceToPositionCharacters(front.piece);
        var myTargetPos = Vector3.Lerp(myPiecePos, frontPiecePos, distance / 1);
        transform.position = myTargetPos;


        if (this as Body == null)
            return;
        var index = head.bodies.IndexOf(this as Body);
        var next = index + 1;
        if (next < bodies.Count)
            bodies[next].Move(head, this, distance);
        else
            head.tail.Move(head, this, distance);
    }
    protected void TrySettle(float distance)
    {
        if (distance > 1)
        {
            var dist = Mathf.Infinity;
            MapWrapper findPiece = null;
            foreach (var item in moveables)
            {
                var pos = MapCreator.Instance.PieceToPositionCharacters(item);
                var newDist = Vector2.Distance(pos, transform.position);
                if (dist > newDist)
                {
                    dist = newDist;
                    findPiece = item;
                }
            }
            if (findPiece != null)
            {
                tail.SetPiece(bodies[bodies.Count - 1].piece);
                for (int i = bodies.Count - 1; i >= 0; i--)
                {
                    var body = bodies[i];
                    if (i == 0)
                        body.SetPiece(piece);
                    else
                        body.SetPiece(bodies[i - 1].piece);
                }
                SetPiece(findPiece);
                moveables = MapController.Instance.FindMoveables(piece);
            }
        }
    }

}
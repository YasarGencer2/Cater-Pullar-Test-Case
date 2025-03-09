using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    public CharacterType CharacterType;
    public Moveable head;
    public List<Moveable> bodies = new();
    public Moveable tail;

    public Vector3 piecePos;
    public MapWrapper piece;
    public List<MapWrapper> moveables;


    float clampRight = 0.5f;
    float clampLeft = 0.5f;
    float clampUp = 0.5f;
    float clampDown = 0.5f;
    public void SetHead(Moveable head) => this.head = head;
    public void SetBodies(List<Moveable> bodies) => this.bodies = bodies;
    public void SetTail(Moveable tail) => this.tail = tail;
    public void SetPiece(MapWrapper piece)
    {
        this.piece = piece;
        piecePos = MapCreator.Instance.PieceToPositionCharacters(piece);
    }
    public void Pick() => UpdateMoveablePlaces();
    public void Hold(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, piecePos.x + clampLeft, piecePos.x + clampRight);
        position.y = Mathf.Clamp(position.y, piecePos.y + clampDown, piecePos.y + clampUp);
        head.transform.position = position;
        var distance = Vector3.Distance(head.transform.position, piecePos);
        bodies[0].Move(this, this, distance);
        TrySettle(distance);
    }
    public void Release()
    {
        head.transform.position = piecePos;
        bodies[0].Move(this, this, 0);
    }
    protected void Move(Moveable head, Moveable front, float distance)
    {
        var bodies = head.bodies;
        // Debug.Log("Move " + this.name + " to " + front.name);
        var myPiecePos = piecePos;
        var frontPiecePos = front.piecePos;
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
                UpdateMoveablePlaces();
            }
        }
    }
    void UpdateMoveablePlaces()
    {
        moveables = MapController.Instance.FindMoveables(piece);
        if (this as Tail == null)
            tail.UpdateMoveablePlaces();

        var behind = bodies[0].piece;
        moveables.Remove(behind);

        clampRight = 0.5f;
        clampLeft = -0.5f;
        clampUp = 0.5f;
        clampDown = -0.5f;

        foreach (var item in moveables)
        {
            if (item.x > piece.x)
                clampRight += 1;
            else if (item.x < piece.x)
                clampLeft -= 1;
            else if (item.y > piece.y)
                clampUp += 1;
            else if (item.y < piece.y)
                clampDown -= 1;
        }
    }

}
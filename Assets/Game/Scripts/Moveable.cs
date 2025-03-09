using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    public CharacterType CharacterType;
    public Moveable head;
    public List<Moveable> bodies = new();
    public Moveable tail;

    public Vector3 piecePos;
    public MapPiece piece;
    public List<MapPiece> moveables;
    public bool canMoveBackwards, movingBackwards = false;
    public MapPiece backPiece;
    public Vector3 backPiecePos;

    public List<MapPiece> path = new();


    float clampRight = 0.5f;
    float clampLeft = 0.5f;
    float clampUp = 0.5f;
    float clampDown = 0.5f;
    public void SetHead(Moveable head) => this.head = head;
    public void SetBodies(List<Moveable> bodies) => this.bodies = bodies;
    public void SetTail(Moveable tail) => this.tail = tail;
    public void SetPiece(MapPiece piece)
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

        if (canMoveBackwards)
            movingBackwards = Vector3.Distance(transform.position, bodies[0].piecePos) < 0.9f;
        else
            movingBackwards = false;

        if (movingBackwards)
            tail.MoveBackwards(this, tail.backPiecePos, distance);
        else
            bodies[0].Move(this, this, distance);
        TrySettle(distance);
    }
    public void Release()
    {
        head.transform.position = piecePos;
        bodies[0].Move(this, this, 0);
        path = new();
        tail.path = new();
    }
    protected void Move(Moveable head, Moveable front, float distance)
    {
        var bodies = head.bodies;
        // Debug.Log("Move " + this.name + " to " + front.name);  
        transform.position = Vector3.Lerp(piecePos, front.piecePos, distance / 1);


        if (this as Body == null)
            return;
        var index = head.bodies.IndexOf(this as Body);
        var next = index + 1;
        if (next < bodies.Count)
            bodies[next].Move(head, this, distance);
        else
            head.tail.Move(head, this, distance);
    }
    protected void MoveBackwards(Moveable head, Vector3 back, float distance)
    {
        var bodies = head.tail.bodies;
        // Debug.Log("Move " + this.name + " to " + front.name);  
        transform.position = Vector3.Lerp(piecePos, back, distance / 1);

        var index = -1;
        if ((this is Tail) == false)
            index = bodies.IndexOf(this as Body);
        var next = index + 1;
        if (next < bodies.Count)
            bodies[next].MoveBackwards(head, piecePos, distance);
    }
    protected void TrySettle(float distance)
    {
        if (distance > 1)
        {
            if (movingBackwards)
            {
                if (path.Count > 0)
                    path.RemoveAt(path.Count - 1);
                if (tail.path.Count > 0)
                    tail.path.RemoveAt(tail.path.Count - 1);
                SetPiece(bodies[0].piece);
                for (int i = 0; i < bodies.Count - 1; i++)
                {
                    bodies[i].SetPiece(bodies[i + 1].piece);
                }
                bodies[bodies.Count - 1].SetPiece(tail.piece);
                tail.SetPiece(tail.backPiece);
                UpdateMoveablePlaces();

            }
            else
            {
                var dist = Mathf.Infinity;
                MapPiece findPiece = null;
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
                    tail.path.Add(tail.piece);
                    tail.SetPiece(bodies[bodies.Count - 1].piece);
                    for (int i = bodies.Count - 1; i >= 0; i--)
                    {
                        var body = bodies[i];
                        if (i == 0)
                            body.SetPiece(piece);
                        else
                            body.SetPiece(bodies[i - 1].piece);
                    }
                    path.Add(piece);
                    SetPiece(findPiece);
                    UpdateMoveablePlaces();
                }
            }

        }
    }
    bool UpdateMoveablePlaces()
    {
        canMoveBackwards = tail.CheckOtherEnd();
        moveables = MapController.Instance.FindMoveables(piece);

        var neck = bodies[0].piece;
        if (canMoveBackwards == false)
            moveables.Remove(neck);

        clampRight = 0.25f;
        clampLeft = -0.25f;
        clampUp = 0.25f;
        clampDown = -0.25f;

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
        return true;
    }
    bool CheckOtherEnd()
    {
        Debug.Log(path.Count);
        if (path.Count > 0)
        {
            backPiece = path[path.Count - 1];
            backPiecePos = MapCreator.Instance.PieceToPositionCharacters(backPiece);
            return true;
        }
        moveables = MapController.Instance.FindMoveables(piece);
        var neck = bodies[0].piece;
        moveables.Remove(neck);
        if (moveables.Count == 1)
        {
            backPiece = moveables[0];
            backPiecePos = MapCreator.Instance.PieceToPositionCharacters(backPiece);
            return true;
        }
        else if (moveables.Count > 1)
        {
            foreach (var item in moveables)
            {
                Debug.Log(item.x == neck.x || item.y == neck.y);
                if (item.x == neck.x || item.y == neck.y)
                {
                    backPiece = item;
                    backPiecePos = MapCreator.Instance.PieceToPositionCharacters(backPiece);
                    return true;
                }
            }
            backPiece = null;
            backPiecePos = Vector3.zero;
            return false;
        }
        else
        {
            backPiece = null;
            backPiecePos = Vector3.zero;
            return false;
        }
    }

}
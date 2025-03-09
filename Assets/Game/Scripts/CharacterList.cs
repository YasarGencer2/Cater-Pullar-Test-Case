using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterList : MonoBehaviour
{
    [SerializeField] MapItem mapItem;
    public static CharacterList Instance { get; internal set; }

    List<Head> heads = new();
    List<Body> bodies = new();
    void Awake()
    {
        Instance = this;
    }
    public void AddCharacter(Head character)
    {
        heads.Add(character);
        TryMatch();
    }
    public void AddBody(Body body)
    {
        bodies.Add(body);
        TryMatch();
    }
    void TryMatch()
    {
        foreach (var item in heads)
        {
            foreach (var item2 in bodies)
            {
                if (item.CharacterType == item2.CharacterType)
                {
                    item.AddBody(item2);
                    bodies.Remove(item2);
                    break;
                }
            }
        }
    }

    public void SetTail(CharacterType type)
    {
        foreach (var item in heads)
        {
            if (item.CharacterType == type)
            {
                item.AddTail(mapItem.tail);
            }
        }
    }
    public void SetTarget(Portal portal, CharacterType type, MapPiece piece)
    {
        foreach (var item in heads)
        {
            if (item.CharacterType == type)
            {
                item.Portal = portal;
                item.tail.Portal = portal;
            }
        }
    }
}
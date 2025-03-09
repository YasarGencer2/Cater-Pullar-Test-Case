using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Tail : Moveable
{
    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(0.5f, 0.4f).SetDelay(.5f);
    }
}
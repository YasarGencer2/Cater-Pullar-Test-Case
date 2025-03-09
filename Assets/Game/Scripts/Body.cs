using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class Body : Moveable
{
    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(0.7f, 0.4f).SetDelay(.5f);
    }
}
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public List<Renderer> renderers;
    public Transform rotate;
    public CharacterType type;
    public MapPiece piece;
    public void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.4f).SetDelay(.5f);
        LevelController.AddActivePortal();
        Invoke("SetAsTarget", 1f);
    }
    void SetAsTarget()
    {
        CharacterList.Instance.SetTarget(this, type, piece);
    }
    public void SetMaterial(Material mat)
    {
        rotate.localScale = Vector3.one * 0.8f;
        rotate.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        foreach (var item in renderers)
        {
            item.material = mat;
        }
    }
    void Update()
    {
        rotate.Rotate(Vector3.forward * 100f * Time.deltaTime);
    }
}
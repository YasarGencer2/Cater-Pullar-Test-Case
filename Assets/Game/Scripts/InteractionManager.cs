using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    Head activeCharacter;
    Tail activeTail;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var go = hit.collider.gameObject;
                if (go.tag == "Head")
                {
                    activeCharacter = go.GetComponent<Head>();
                    activeCharacter.Pick();
                    ReleaseTail();
                }
                else if (go.tag == "Tail")
                {
                    activeTail = go.GetComponent<Tail>();
                    activeTail.Pick();
                    ReleaseCharacter();
                }
                else
                {
                    Debug.Log("Tag: " + go.tag, go);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseCharacter();
            ReleaseTail();
        }
        else if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            mousePosition.z = -5;
            if (activeCharacter)
                activeCharacter.Hold(mousePosition);
            else if (activeTail)
                activeTail.Hold(mousePosition);
        }
    }
    void ReleaseTail()
    {
        if (activeTail)
            activeTail.Release();
        activeTail = null;
    }
    void ReleaseCharacter()
    {
        if (activeCharacter)
            activeCharacter.Release();
        activeCharacter = null;
    }
}
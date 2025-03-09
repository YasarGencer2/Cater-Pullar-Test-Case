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
                if (go.CompareTag("Head"))
                {
                    activeCharacter = go.GetComponent<Head>();
                    activeCharacter.Pick();
                    activeTail.Release();
                }
                else if (go.CompareTag("Tail"))
                {
                    activeTail = go.GetComponent<Tail>();
                    activeTail.Pick();
                    activeCharacter.Release();
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            activeCharacter.Release();
            activeTail.Release();
        }
    }
}
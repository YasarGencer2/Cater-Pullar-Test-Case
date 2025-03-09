using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{

    [SerializeField] List<Map> levels;
    [SerializeField] UIController canvas;
    public static int CurrentLevel = 0;
    static int activePortals;
    float currentTime;
    bool started = false;
    public void Start()
    {
        activePortals = 0;
        var level = levels[CurrentLevel % levels.Count];
        MapCreator.Instance.CreateMap(level);
        canvas.LevelText(CurrentLevel);
        canvas.TimeText(level.timeLimit);
        currentTime = level.timeLimit;
        Camera.main.transform.position = new Vector3(-5, 1, -10);
        Camera.main.transform.DOMove(new Vector3(0, 1, -10), 0.3f);
    }
    public static void AddActivePortal()
    {
        activePortals++;
    }
    public static void RemovePortal()
    {
        activePortals--;
        if (activePortals <= 0)
        {
            CurrentLevel++;
            Die();
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            if (started == false)
                started = true;
        if (started == false)
            return;
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            CurrentLevel = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        canvas.TimeText(Mathf.CeilToInt(currentTime));
    }
    public static void Die()
    {
        var width = Camera.main.ViewportToWorldPoint(new Vector3(1, 1)).x;
        Camera.main.transform.DOMove(new Vector3(Camera.main.transform.position.x + width * 3f, 1, -10), 1f).OnComplete(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }
}

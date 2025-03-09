using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Transform line2;
    [SerializeField] Button restart;

    float lastTime = 0;
    void Start()
    {
        restart.onClick.RemoveAllListeners();
        restart.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }
    public void LevelText(int index)
    {
        levelText.SetText("level: " + (index + 1));
    }

    public void TimeText(float time)
    {
        if (lastTime == time)
            return;
        lastTime = time;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeText.text = $"{minutes:00}:{seconds:00}";
        line2.DORotate(new Vector3(0, 0, -90), 0.3f, RotateMode.FastBeyond360).SetEase(Ease.OutBounce).SetRelative(true);
    }
}

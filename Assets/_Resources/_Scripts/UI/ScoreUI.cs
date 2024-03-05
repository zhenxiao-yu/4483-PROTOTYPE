using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI scoreTextTMP;

    private void Awake()
    {
        scoreTextTMP = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        SubscribeToScoreChangedEvent();
    }

    private void OnDisable()
    {
        UnsubscribeFromScoreChangedEvent();
    }

    private void SubscribeToScoreChangedEvent()
    {
        StaticEventHandler.OnScoreChanged += UpdateScoreText;
    }

    private void UnsubscribeFromScoreChangedEvent()
    {
        StaticEventHandler.OnScoreChanged -= UpdateScoreText;
    }

    private void UpdateScoreText(ScoreChangedArgs scoreChangedArgs)
    {
        scoreTextTMP.text = $"SOULS FREED: {scoreChangedArgs.score.ToString("###,###0")}\n\nHIT BONUS: x{scoreChangedArgs.multiplier}";
    }
}

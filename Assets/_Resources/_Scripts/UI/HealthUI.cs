using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    private List<GameObject> healthHeartsList = new List<GameObject>();

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        UpdateHealthBar(healthEventArgs);
    }

    private void ClearHealthBar()
    {
        foreach (GameObject heartIcon in healthHeartsList)
        {
            Destroy(heartIcon);
        }
        healthHeartsList.Clear();
    }

    private void UpdateHealthBar(HealthEventArgs healthEventArgs)
    {
        ClearHealthBar();
        int numHearts = Mathf.CeilToInt(healthEventArgs.healthPercent * 100f / 20f);

        for (int i = 0; i < numHearts; i++)
        {
            GameObject heart = Instantiate(GameResources.Instance.heartPrefab, transform);
            heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing * i, 0f);
            healthHeartsList.Add(heart);
        }
    }
}

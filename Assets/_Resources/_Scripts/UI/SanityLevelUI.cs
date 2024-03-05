using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SanityLevelUI : MonoBehaviour
{
    [SerializeField] private Transform sanityBarImage;
    [SerializeField] private TextMeshProUGUI sanityText;
    [SerializeField] private float sanityIncreaseRate = 0.3f; // Rate at which sanity increases per second
    [SerializeField] private float sanityDecreaseAmount = 0.01f; // Amount of sanity decreased per action

    private float currentSanityLevel = 1f;

    private void Start()
    {
        // Initialize UI based on initial sanity level
        UpdateSanityLevelUI(currentSanityLevel);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.W)|| !Input.GetKeyDown(KeyCode.A) || !Input.GetKeyDown(KeyCode.S) || !Input.GetKeyDown(KeyCode.D)) {
            IncreaseSanity(Time.deltaTime * sanityIncreaseRate);
        }

        // Check for inputs that decrease sanity
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.Space))
        {
            // Decrease sanity when Shift or Space is pressed
            DecreaseSanity(sanityDecreaseAmount);
        }
    }

    private void IncreaseSanity(float amount)
    {
        // Increase sanity level, clamping between 0 and 1
        currentSanityLevel = Mathf.Clamp01(currentSanityLevel + amount);

        // Update sanity UI
        UpdateSanityLevelUI(currentSanityLevel);
    }

    private void DecreaseSanity(float amount)
    {
        // Decrease sanity level, clamping between 0 and 1
        currentSanityLevel = Mathf.Clamp01(currentSanityLevel - amount);

        // Update sanity UI
        UpdateSanityLevelUI(currentSanityLevel);
    }

    private void UpdateSanityLevelUI(float sanityLevel)
    {
        // Update sanity UI elements
        sanityBarImage.localScale = new Vector3(sanityLevel, 1f, 1f);
        
        if (sanityText != null)
        {
            sanityText.text = $"{Mathf.FloorToInt(sanityLevel*100)}%"; // Update the text component to display the sanity value
        }
    }
}

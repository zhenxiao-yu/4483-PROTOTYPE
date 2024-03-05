using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SanityLevelUI : MonoBehaviour
{
    [SerializeField] private Transform sanityBarImage;
    [SerializeField] private TextMeshProUGUI sanityText;
    [SerializeField] private float sanityIncreaseRate = 0.3f; // Rate at which sanity increases per second
    [SerializeField] private float sanityDecreaseAmount = 0.005f; // Amount of sanity decreased per action

    public PostProcessVolume postProcessVolume; // Assign in inspector


    public float maxSaturation = 100f; // Maximum saturation value
    public float minSaturation = 0f;   // Minimum sat

    private float currentSanityLevel = 1f;

    private void Start()
    {
        // Initialize UI based on initial sanity level
        UpdateSanityLevelUI(currentSanityLevel);
    }

    private void Update()
    {
        // Check if the character is idle (no movement or action keys pressed)
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) &&
            !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && !Input.GetKey(KeyCode.Space))
        {
            IncreaseSanity(Time.deltaTime * sanityIncreaseRate);
        }
        // Check for inputs that decrease sanity
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.Space))
        {
            // Decrease sanity when Shift or Space is pressed
            DecreaseSanity(sanityDecreaseAmount);
        }

        //apply sanity filter based on level of sanity
        ApplySanityFilter(currentSanityLevel);
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

    private void ApplySanityFilter(float sanityLevel)
    {
        if (postProcessVolume.profile.TryGetSettings(out Vignette vignette))
        {
            vignette.intensity.value = 1 - sanityLevel; // Increase vignette as sanity decreases
        }

        if (postProcessVolume.profile.TryGetSettings(out ColorGrading colorGrading))
        {
            colorGrading.saturation.value = Mathf.Lerp(-100, 0, sanityLevel); // Desaturate as sanity decreases
        }

        if (postProcessVolume.profile.TryGetSettings(out Grain grain))
        {
            grain.intensity.value = 1 - sanityLevel; // Increase grain as sanity decreases
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class WeaponStatusUI : MonoBehaviour
{
    [Header("Object References")]
    [Tooltip("Image component on the child WeaponImage GameObject")]
    [SerializeField] private Image weaponImage;
    [Tooltip("Transform from the child AmmoHolder GameObject")]
    [SerializeField] private Transform ammoHolderTransform;
    [Tooltip("TextMeshPro-Text component on the child ReloadText GameObject")]
    [SerializeField] private TextMeshProUGUI reloadText;
    [Tooltip("TextMeshPro-Text component on the child AmmoRemainingText GameObject")]
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    [Tooltip("TextMeshPro-Text component on the child WeaponNameText GameObject")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [Tooltip("RectTransform of the child ReloadBar GameObject")]
    [SerializeField] private RectTransform reloadBar;
    [Tooltip("Image component of the child BarImage GameObject")]
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        if (player != null)
        {
            player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
            player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
            player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnWeaponReload;
            player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
            player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
            player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnWeaponReload;
            player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
        }
    }

    private void Start()
    {
        if (player != null)
            SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    private void ReloadWeaponEvent_OnWeaponReload(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void WeaponReloaded(Weapon weapon)
    {
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        if (weapon.isWeaponReloading)
            UpdateWeaponReloadBar(weapon);
        else
            ResetWeaponReloadBar();

        UpdateReloadText(weapon);
    }

    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = $"({weapon.weaponListPosition}) {weapon.weaponDetails.weaponName.ToUpper()}";
    }

    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
            ammoRemainingText.text = "INFINITE AMMO";
        else
            ammoRemainingText.text = $"{weapon.weaponRemainingAmmo} / {weapon.weaponDetails.weaponAmmoCapacity}";
    }

    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);
            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);
            ammoIconList.Add(ammoIcon);
        }
    }

    private void ClearAmmoLoadedIcons()
    {
        foreach (GameObject ammoIcon in ammoIconList)
            Destroy(ammoIcon);

        ammoIconList.Clear();
    }

    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);
        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        barImage.color = Color.red;

        while (currentWeapon.isWeaponReloading)
        {
            float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;
            reloadBar.localScale = new Vector3(barFill, 1f, 1f);
            yield return null;
        }
    }

    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();
        barImage.color = Color.green;
        reloadBar.localScale = new Vector3(1f, 1f, 1f);
    }

    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
            StopCoroutine(reloadWeaponCoroutine);
    }

    private void UpdateReloadText(Weapon weapon)
    {
        if (!weapon.weaponDetails.hasInfiniteClipCapacity && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            barImage.color = Color.red;
            StopBlinkingReloadTextCoroutine();
            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();
        reloadText.text = "";
    }

    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
            StopCoroutine(blinkingReloadTextCoroutine);
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }

#endif
    #endregion Validation
}

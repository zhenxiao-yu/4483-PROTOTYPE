using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class CharacterSelectorUI : MonoBehaviour
{
    [Tooltip("Populate this with the child CharacterSelector GameObject")]
    [SerializeField] private Transform characterSelector;

    [Tooltip("Populate with the TextMeshPro component on the PlayerNameInput GameObject")]
    [SerializeField] private TMP_InputField playerNameInput;

    private List<PlayerDetailsSO> playerDetailsList;
    private GameObject playerSelectionPrefab;
    private CurrentPlayerSO currentPlayer;
    private List<GameObject> playerCharacterGameObjectList = new List<GameObject>();
    private Coroutine moveCharacterCoroutine;
    private int selectedPlayerIndex = 0;
    private float offset = 4f;

    private void Awake()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        playerSelectionPrefab = GameResources.Instance.playerSelectionPrefab;
        playerDetailsList = GameResources.Instance.playerDetailsList;
        currentPlayer = GameResources.Instance.currentPlayer;
    }

    private void Start()
    {
        InstantiatePlayerCharacters();
        SetInitialPlayerDetails();
    }

    private void InstantiatePlayerCharacters()
    {
        for (int i = 0; i < playerDetailsList.Count; i++)
        {
            GameObject playerSelectionObject = Instantiate(playerSelectionPrefab, characterSelector);
            playerCharacterGameObjectList.Add(playerSelectionObject);
            playerSelectionObject.transform.localPosition = new Vector3((offset * i), 0f, 0f);
            PopulatePlayerDetails(playerSelectionObject.GetComponent<PlayerSelectionUI>(), playerDetailsList[i]);
        }
    }

    private void PopulatePlayerDetails(PlayerSelectionUI playerSelection, PlayerDetailsSO playerDetails)
    {
        playerSelection.playerHandSpriteRenderer.sprite = playerDetails.playerHandSprite;
        playerSelection.playerHandNoWeaponSpriteRenderer.sprite = playerDetails.playerHandSprite;
        playerSelection.playerWeaponSpriteRenderer.sprite = playerDetails.startingWeapon.weaponSprite;
        playerSelection.animator.runtimeAnimatorController = playerDetails.runtimeAnimatorController;
    }

    private void SetInitialPlayerDetails()
    {
        playerNameInput.text = currentPlayer.playerName;
        currentPlayer.playerDetails = playerDetailsList[selectedPlayerIndex];
    }

    public void NextCharacter()
    {
        if (selectedPlayerIndex >= playerDetailsList.Count - 1)
            return;

        selectedPlayerIndex++;
        currentPlayer.playerDetails = playerDetailsList[selectedPlayerIndex];
        MoveToSelectedCharacter(selectedPlayerIndex);
    }

    public void PreviousCharacter()
    {
        if (selectedPlayerIndex == 0)
            return;

        selectedPlayerIndex--;
        currentPlayer.playerDetails = playerDetailsList[selectedPlayerIndex];
        MoveToSelectedCharacter(selectedPlayerIndex);
    }

    private void MoveToSelectedCharacter(int index)
    {
        if (moveCharacterCoroutine != null)
            StopCoroutine(moveCharacterCoroutine);

        moveCharacterCoroutine = StartCoroutine(MoveToSelectedCharacterRoutine(index));
    }

    private IEnumerator MoveToSelectedCharacterRoutine(int index)
    {
        float currentLocalXPosition = characterSelector.localPosition.x;
        float targetLocalXPosition = index * offset * characterSelector.localScale.x * -1f;

        while (Mathf.Abs(currentLocalXPosition - targetLocalXPosition) > 0.01f)
        {
            currentLocalXPosition = Mathf.Lerp(currentLocalXPosition, targetLocalXPosition, Time.deltaTime * 10f);
            characterSelector.localPosition = new Vector3(currentLocalXPosition, characterSelector.localPosition.y, 0f);
            yield return null;
        }

        characterSelector.localPosition = new Vector3(targetLocalXPosition, characterSelector.localPosition.y, 0f);
    }

    public void UpdatePlayerName()
    {
        playerNameInput.text = playerNameInput.text.ToUpper();
        currentPlayer.playerName = playerNameInput.text;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ValidateRequiredFields();
    }

    private void ValidateRequiredFields()
    {
        if (characterSelector == null)
            Debug.LogError("Character selector is not assigned!", this);

        if (playerNameInput == null)
            Debug.LogError("Player name input is not assigned!", this);
    }
#endif
}

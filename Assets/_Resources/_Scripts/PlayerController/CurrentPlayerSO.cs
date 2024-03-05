using UnityEngine;

/// <summary>
/// ScriptableObject representing the current player.
/// </summary>
[CreateAssetMenu(fileName = "CurrentPlayer", menuName = "Scriptable Objects/Player/Current Player")]
public class CurrentPlayerSO : ScriptableObject
{
    /// <summary>
    /// Details of the current player.
    /// </summary>
    public PlayerDetailsSO playerDetails;

    /// <summary>
    /// Name of the current player.
    /// </summary>
    public string playerName;
}

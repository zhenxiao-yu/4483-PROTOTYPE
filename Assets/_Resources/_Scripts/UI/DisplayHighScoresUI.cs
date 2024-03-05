using UnityEngine;

public class DisplayHighScoresUI : MonoBehaviour
{
    [Header("Object References")]
    [Tooltip("Populate with the child Content GameObject's Transform component")]
    [SerializeField] private Transform contentAnchorTransform;

    private void Start()
    {
        DisplayScores();
    }

    private void DisplayScores()
    {
        HighScores highScores = HighScoreManager.Instance.GetHighScores();
        GameObject scoreGameObject;
        int rank = 0;

        foreach (Score score in highScores.scoreList)
        {
            rank++;
            scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
            ScorePrefab scorePrefab = scoreGameObject.GetComponent<ScorePrefab>();
            SetScoreDetails(scorePrefab, rank, score);
        }
    }

    private void SetScoreDetails(ScorePrefab scorePrefab, int rank, Score score)
    {
        scorePrefab.rankTMP.text = rank.ToString();
        scorePrefab.nameTMP.text = score.playerName;
        scorePrefab.levelTMP.text = score.levelDescription;
        scorePrefab.scoreTMP.text = score.playerScore.ToString("###,###0");
    }
}

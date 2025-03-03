using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public TMP_Text scoreText;

    private int killScore = 0;
    private float gameScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ResetScore();
        CalculateScore();
    }

    private void CalculateScore()
    {
        // Calculate the game score with a formula
        gameScore = Mathf.Pow(killScore, 1.1f) + (PlayerController.Instance == null ? 0f : PlayerController.Instance.GameplayTime);

        // Display the game score
        scoreText.text = "<b>SCORE</b>: " + gameScore.ToString("0");
    }

    public void AddScore(int amount)
    {
        killScore += amount;
        CalculateScore();
    }

    public void ResetScore()
    {
        killScore = 0;
        gameScore = 0f;
    }

    public int GetScore()
    {
        CalculateScore();
        return gameScore > 0 ? (int)gameScore : 0;
    }
}
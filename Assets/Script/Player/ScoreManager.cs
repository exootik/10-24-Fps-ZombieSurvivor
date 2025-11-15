using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ScoreBoard
{
    [JsonProperty] public List<int> Board = new();

    [JsonProperty] public int CurrentScore;


    public void Save()
    {
        global::Save.LocalSaveData("Score Board", this);
    }

    public void Load()
    {
        global::Save.LocalLoadData<ScoreBoard>("Score Board", out var temp);
        Board = temp.Board;
    }

    public void AddCurrentScore()
    {
        Board.Add(CurrentScore);
        Board = Board.OrderByDescending(score => score).ToList();
        if (Board.Count > 10)
        {
            Board.RemoveRange(10, Board.Count - 10);
            Board.TrimExcess();
        }
    }
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [JsonProperty] public ScoreBoard Score { get; private set; } = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Score.Load();
    }

    private void OnApplicationQuit()
    {
        Score.AddCurrentScore();
        Score.Save();
    }

    public event Action<int> OnScoreChanged;

    public void AddScore(int amount)
    {
        Score.CurrentScore += amount;
        OnScoreChanged?.Invoke(Score.CurrentScore);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager _instance;
    public GameObject PlayerController;
    public TextMeshProUGUI ScoreUI;

    private List<double> _brainScores;
    private double _currOveralScore;

    // Public property to access the singleton instance
    public static GameManager Instance
    {
        get
        {
            // If the instance is null, try to find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                // If it's still null, create a new GameObject with GameManager attached
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    // Ensure the instance is properly initialized even if it's not created via Instance property
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _brainScores = new List<double>();
    }

    // Example method to start the game
    public void SetPlayerEnabled(bool enabled)
    {
        PlayerController.SetActive(enabled);
    }

    public void RegisterBrainScore(double score)
    {
        _brainScores.Add(score);
        _currOveralScore = _brainScores.Average();
        ScoreUI.text = $"EFFICIENCY RATING: {(_currOveralScore * 100f).ToString("F2") + "%"}";
    }
}
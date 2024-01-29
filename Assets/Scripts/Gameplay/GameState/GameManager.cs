using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager _instance;
    public GameObject PlayerController;
    public GameObject MiniGame;

    public TextMeshProUGUI ScoreUI;

    public bool _debug = false;

    private List<double> _brainScores;
    private double _currOveralScore;

    public bool MiniGameOn;
    public InputActionAsset Actions;
    private string _exitInputKey;
    public TextMeshProUGUI MiniGameEscapeText;
    public BrainStatusUI BrainMonitor;

    private bool _resetNeeded;

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
        Actions.FindActionMap(InputMap.DEFAULT_CONTROL_MAP_KEY).Enable();

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

        HookupInputs();

        TurnOffMiniGame();
    }

    private void HookupInputs()
    {
        InputAction exit = Actions.FindActionMap(InputMap.DEFAULT_CONTROL_MAP_KEY).FindAction(InputMap.ESCAPE_CONTROL_INPUT_KEY);
        exit.performed += ExitButtonPressed;
        _exitInputKey = exit.bindings[0].path;
    }

    public void ExitButtonPressed(InputAction.CallbackContext context)
    {
        if(_debug)Debug.Log("Escape button pressed");
        if (MiniGameOn)
        {
            TurnOffMiniGame();
            GameManager.Instance.SetResetNeeded(true);
        }
    }

    public void TurnOnMiniGame()
    {
        if(_debug)Debug.Log("Mini game ON");
        SetPlayerEnabled(false);
        SetMiniGameEnabled(true);
        Cursor.lockState = CursorLockMode.None;
        MiniGameOn = true;
        MiniGameEscapeText.text = $"{_exitInputKey} - EXIT MINIGAME";
        BrainMonitor.SwitchMonitor(true);

    }

    public void TurnOffMiniGame()
    {
        if(_debug)Debug.Log("Mini game OFF");
        SetPlayerEnabled(true);
        SetMiniGameEnabled(false);
        Cursor.lockState = CursorLockMode.Locked;
        MiniGameOn = false;
        GameManager.Instance.SetResetNeeded(true);
        BrainMonitor.SwitchMonitor(false);
    }

    // Example method to start the game
    public void SetPlayerEnabled(bool enabled)
    {
        PlayerController.SetActive(enabled);
    }

    public void SetMiniGameEnabled(bool enabled)
    {
        MiniGame.SetActive(enabled);
    }

    public void RegisterBrainScore(double score)
    {
        _brainScores.Add(score);
        _currOveralScore = _brainScores.Average();
        ScoreUI.text = $"EFFICIENCY RATING: {(_currOveralScore * 100f).ToString("F2") + "%"}";
    }

    public bool IsResetNeeded()
    {
        return _resetNeeded;
    }

    public void SetResetNeeded(bool value)
    {
        _resetNeeded = value;
    }
}
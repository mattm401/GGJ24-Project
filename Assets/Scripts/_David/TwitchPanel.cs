using System;
using System.Collections;
using System.Collections.Generic;
using CommandTerminal;
using NaughtyAttributes;
using TMPro;
using TwitchChatConnect.Client;
using TwitchChatConnect.Config;
using TwitchChatConnect.Data;
using TwitchChatConnect.Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TwitchPanel : MonoBehaviour
{
    #region Link

    [Foldout("Link")] public GameObject gameObjectMainPanel;
    [Foldout("Link")] public TMP_InputField inputUserName;
    [Foldout("Link")] public TMP_InputField inputUserToken;
    [Foldout("Link")] public TMP_InputField inputChannelName;
    [Foldout("Link")] public Button buttonConnect;
    [Foldout("Link")] public TextMeshProUGUI textButtonConnect;
    
    [Foldout("Link")] public TMP_InputField inputMessage;

    #endregion

    #region Project File
    [Foldout("Object")]  public TwitchConnectData _initTwitchConnectData;
    #endregion

    #region Settings
    
    [BoxGroup("Setting")] public bool isUsingTimerAndCounter = false;
    [BoxGroup("Setting")] public float timerInterval = 5.0f;
    
    #endregion
    
    #region Private Var
    
    private Terminal _terminal;
    private bool _sendHelpCommand = false;
    private Dictionary<string, int> _dictionaryCommandCount = new Dictionary<string, int>();
  
    #endregion

    #region callback

    public void OnButtonClickConnect()
    {
        buttonConnect.interactable = false;

        SetButtonText("Connecting");
        
        _initTwitchConnectData.TwitchConnectConfig.username = inputUserName.text;
        _initTwitchConnectData.TwitchConnectConfig.userToken = inputUserToken.text;
        _initTwitchConnectData.TwitchConnectConfig.channelName = inputChannelName.text;

        
        TwitchChatClient.instance.Init(() =>
            {
                Debug.Log("Connected!");
                SetButtonText("Connected!");

                TwitchChatClient.instance.onChatMessageReceived += ShowMessage;
                TwitchChatClient.instance.onChatCommandReceived += ShowCommand;
                //TwitchChatClient.instance.onChatRewardReceived += ShowReward;

                TwitchUserManager.OnUserAdded += twitchUser =>
                {
                    Debug.Log($"{twitchUser.Username} has connected to the chat.");
                };

                TwitchUserManager.OnUserRemoved += username =>
                {
                    Debug.Log($"{username} has left the chat.");
                };
            },
            message =>
            {
                // Error when initializing.
                buttonConnect.interactable = true;
                SetButtonText("Connect");
                SetMessage(message);
                Debug.LogError(message);
            });
        
    }
    
    #endregion

    #region Twitch Functions
    
    private void ShowCommand(TwitchChatCommand chatCommand)
    {
        if (chatCommand.Command.ToLower() == "!help")
        {
            if (isUsingTimerAndCounter == false)
            {
                FireHelpCommand();
            }
            else
            {
                _sendHelpCommand = true;
            }
        }
        
    }

    private void FireHelpCommand()
    {
        string message = BuiltinCommands.GetListOfCommandAndInfo();
        TwitchChatClient.instance.SendChatMessage($"Enter these command:\n{message}");
        _terminal.EnterCommand("help");
    }

    private void ShowMessage(TwitchChatMessage chatMessage)
    {
        if (isUsingTimerAndCounter == false)
        {
            _terminal.EnterCommand(chatMessage.Message);
        }
        else
        {
            string message = chatMessage.Message;
            _dictionaryCommandCount.TryAdd(message, 0);
            _dictionaryCommandCount[message]++;
        }
  
    }
    #endregion

    #region Private Function

    private void SetButtonText(string message)
    {
        textButtonConnect.SetText(message);
    }
    
    private void SetMessage(string message)
    {
        inputMessage.text = inputMessage.text + "\n" + message;
    }
    

    #endregion
    
    #region Unity Function

    private void Awake()
    {
        _terminal = FindFirstObjectByType<Terminal>();
    }

    private void OnEnable()
    {
        if (isUsingTimerAndCounter == true)
        {
            StartCoroutine(EnterCommandAtInterval());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    private IEnumerator EnterCommandAtInterval()
    {
        for (;;)
        {
            yield return new WaitForSeconds(timerInterval);

            //help command
            if (_sendHelpCommand)
            {
                _sendHelpCommand = false;
                FireHelpCommand();
            }

            //message to fire
            string commandToFire = "";
            int countMax = 0;

            //iterate dictionary
            foreach (var VARIABLE in _dictionaryCommandCount)
            {
                string key = VARIABLE.Key;
                int value = VARIABLE.Value;

                if (value > countMax)
                {
                    countMax = value;
                    commandToFire = key;
                }
            }
            //clear for next time
            _dictionaryCommandCount.Clear();
            
            //if we have something to fire then fire it.
            if (string.IsNullOrEmpty(commandToFire) == false)
            {
                _terminal.EnterCommand(commandToFire);
            }
        }
        
        // ReSharper disable once IteratorNeverReturns
    }

    #endregion
}

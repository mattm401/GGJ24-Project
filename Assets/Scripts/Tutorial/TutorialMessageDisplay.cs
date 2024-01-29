using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TutorialMessageDisplay : MonoBehaviour
{
    public Animator MessageAnimator;
    public TMP_Text MessageText;
    public Image MessageImage;
    private TutorialMessage _currentMessage;

    private Coroutine _displayRoutine;
    private bool _isPlaying = false;
    private List<TutorialMessage> _messageQueue;

    private Coroutine _multiRoutine;
    public void DisplayMessage(TutorialMessage message)
    {
        _currentMessage= message;
        _displayRoutine = StartCoroutine(MessageRoutine());
    }

    public IEnumerator MessageRoutine()
    {
        _isPlaying = true;
        
        
        MessageImage.sprite = _currentMessage.Image;
        MessageAnimator.SetBool("On", true);
        foreach (string block in _currentMessage.Messages)
        {
            yield return new WaitForSeconds(.5f);
            MessageText.text = block;
            MessageAnimator.SetBool("DisplayText", true);
            yield return new WaitForSeconds(2f);
            for(int i=0; i<block.Length; i++)
            {
                yield return new WaitForSeconds(.05f);
            }
            MessageAnimator.SetBool("DisplayText", false);
        }
        MessageAnimator.SetBool("On", false);
        yield return new WaitForSeconds(2f);
        
        _isPlaying = false;
    }

    public IEnumerator MultiRoutine()
    {
        foreach(var message in _messageQueue)
        {
            while (_isPlaying)
                yield return null;
            DisplayMessage(message);
            yield return new WaitForSeconds(2);
        }
    }

    public bool IsPlaying()
    {
        return _isPlaying;
    }

    public void DisplayMultipleMessages(List<TutorialMessage> messages)
    {
        _messageQueue = messages;
        _multiRoutine = StartCoroutine(MultiRoutine());
    }

    public void StopAll()
    {
        if(_multiRoutine!=null)
        {
            StopCoroutine( _multiRoutine);
        }
        if(_displayRoutine!=null)
        {
            StopCoroutine(_displayRoutine);
        }
        MessageAnimator.SetBool("DisplayText", false);
        MessageAnimator.SetBool("On", false);
        _isPlaying = false;
    }
}

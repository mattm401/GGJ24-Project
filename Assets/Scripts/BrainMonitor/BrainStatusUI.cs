using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BrainStatusUI : MonoBehaviour
{
    public BrainStatus BrainStatus;
    public Slider JoyLevel;
    public Slider AngerLevel;
    public Slider TerrorLevel;
    public Slider ConfusionLevel;
    public Animator StatusChangeAnimator;
    public TMP_Text OverallStatus;
    public Image BrainStatusImage;

    public Sprite HealthyBrain;
    public Sprite WeakBrain;
    public Sprite DyingBrain;

    public Color Healthy;
    public Color Weak;
    public Color Dying;

    public AudioSource EKGSound;
    public AudioClip HealthyAudio;
    public AudioClip WeakAudio;
    public AudioClip DyingAudio;
    public AudioClip DeadAudio;

    public bool PlayAudio;
    private bool _audioPlaying;
    public bool Dead;

    public EKGController EKGController;

    private Color _currentColor;
    private int _currentImageLevel;

    public void Start()
    {
        _currentColor = Healthy;
        UpdateSliders();
        SetBrainImage();
        UpdateTextColors();
        UpdateAudio();
    }
    public void Update()
    {
        bool shouldbedead = BrainStatus.GetOverallStability() == 0;
        
        if(shouldbedead && !Dead)
        {
            SetToDead();
        }

        if (!Dead)
        {
            if(EKGController.Magnitude == 0)
            {
                EKGController.Magnitude = 200;
            }

            UpdateSliders();
            SetBrainImage();


        }

        if (PlayAudio)
        {
            if (!_audioPlaying)
            {
                EKGSound.Play();
                _audioPlaying = true;
            }

        }
        else
        {
            EKGSound.Stop();
            _audioPlaying = false;
        }

    }
    public void UpdateSliders()
    {
        JoyLevel.value = BrainStatus.GetJoy();
        AngerLevel.value = BrainStatus.GetAnger();
        TerrorLevel.value = BrainStatus.GetTerror();
        ConfusionLevel.value = BrainStatus.GetConfusion();
        OverallStatus.text =((int )(BrainStatus.GetOverallStability() * 100)).ToString() + "%";
    }

    public void SetBrainImage()
    {
        var level = BrainStatus.HealthLevel();

        if(level!=_currentImageLevel)
        {
            switch(level)
            {
                case 0:
                    BrainStatusImage.sprite = HealthyBrain;
                    _currentColor = Healthy;
                    break;
                case 1:
                    BrainStatusImage.sprite = WeakBrain;
                    _currentColor = Weak;
                    
                    break;
                case 2:
                    BrainStatusImage.sprite = DyingBrain;
                    _currentColor = Dying;
                    break;
            }
            
            _currentImageLevel = level;
            UpdateTextColors();
            UpdateAudio();
        }

        if(BrainStatus.GetOverallStability()==0)
        {
            UpdateAudio();
        }
    }

    public void UpdateTextColors()
    {
        StatusChangeAnimator.SetInteger("status", _currentImageLevel);
        OverallStatus.color = _currentColor;
    }

    public void UpdateAudio()
    {

        switch(_currentImageLevel)
        {
            case 0:
                EKGSound.clip = HealthyAudio;
                EKGController.Speed = 1;
                break;
            case 1:
                EKGSound.clip = WeakAudio;
                EKGController.Speed = 1.5f;
                break;
            case 2:
                EKGSound.clip = DyingAudio;
                EKGController.Speed = 2;
                break;
        }        
        if(_audioPlaying)
        {
            EKGSound.Play();
        }     

    }

    public void SetToDead()
    {
        _currentImageLevel = 2;
        UpdateTextColors();
        SetBrainImage();
        OverallStatus.text = "DEAD";
        JoyLevel.value = 0;
        TerrorLevel.value = 0;
        AngerLevel.value = 0;
        ConfusionLevel.value = 0;
        EKGController.Magnitude = 0;
        EKGSound.clip = DeadAudio;
        if (_audioPlaying)
        {
            EKGSound.Play();
        }
        Dead = true;
    }

    public void SwitchMonitor(bool monitorOn)
    {
        PlayAudio = monitorOn;
        StatusChangeAnimator.SetBool("monitor", monitorOn);
    }
}

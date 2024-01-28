using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Volume))]
public class PostProcessVolumeController : MonoBehaviour
{
    private Volume _postProcessVolume;
    private Coroutine _myCoroutine;

    public float timeToGoMax = 5.0f;
    public float timeToStayMax = 5.0f;
    public float timeToGoMin = 5.0f;

    private void Awake()
    {
        _postProcessVolume = GetComponent<Volume>();
    }


    public void Run()
    {
        if (_myCoroutine != null)
        {
            return;
        }

        _myCoroutine = StartCoroutine(CoRun());
    }


    private void OnDisable()
    {
        _postProcessVolume.enabled = false;
        _postProcessVolume.weight = 0.0f;
    }

    private IEnumerator CoRun()
    {
        //calculate speed
        float speedStart = 1.0f / timeToGoMax;
        yield return null;
        //float speedStayMax = 1.0f / timeToStayMax;
        //yield return null;
        float speedEnd = 1.0f / timeToGoMin;
        yield return null;

        //start going upwards
        _postProcessVolume.enabled = true;
        _postProcessVolume.weight = 0.0f;

        while (_postProcessVolume.weight < 1.0f)
        {
            yield return null;
            _postProcessVolume.weight += Time.deltaTime * speedStart;
        }

        //we should be done now. wait for a bit.
        yield return new WaitForSeconds(timeToStayMax);

        //we are done going backwards

        while (_postProcessVolume.weight > 0.0f)
        {
            yield return null;
            _postProcessVolume.weight -= Time.deltaTime * speedEnd;
        }

        yield return null;
        
        //we are done.
        _postProcessVolume.enabled = false;
        _postProcessVolume.weight = 0.0f;
    }
}

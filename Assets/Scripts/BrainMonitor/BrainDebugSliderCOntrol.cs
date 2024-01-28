using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrainDebugSliderCOntrol : MonoBehaviour
{
    public BrainStatus BrainStatus;
    public Slider EyeSlider;
    public Slider MouthSlider;
    public Slider GooSlider;
    public Slider NodeSlider;
    public Slider ConductSlider;

    public void Start()
    {
        InitializeSliders();
    }

    public void InitializeSliders()
    {
        EyeSlider.value = BrainStatus.GetEye();
        MouthSlider.value = BrainStatus.GetMouth();
        GooSlider.value = BrainStatus.GetGoo();
        NodeSlider.value = BrainStatus.GetNode();
        ConductSlider.value = BrainStatus.GetConduct();
    }


    public void UpdateEye()
    {        
        BrainStatus.ChangeEye(EyeSlider.value);
    }

    public void UpdateMouth()
    {
        BrainStatus.ChangeMouth(MouthSlider.value);

    }

    public void UpdateGoo()
    {
        BrainStatus.ChangeGoo(GooSlider.value);
    }

    public void UpdateNode()
    {
        BrainStatus.ChangeNode(NodeSlider.value);
    }

    public void UpdateConductivity()
    {
        BrainStatus.ChangeConductivity(ConductSlider.value);
    }

}

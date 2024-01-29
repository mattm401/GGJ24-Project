using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainPipeController : MonoBehaviour
{

    public GameObject BrainObjectReference;
    public Transform BrainSpawnLocation;
    private bool _playedTutorial;
    public TutorialMessageDisplay MessageUI;
    public List<TutorialMessage> FirstBrainPick;
    public void CreateBrain()
    {
        GameObject newBrain = GameObject.Instantiate(BrainObjectReference);
        newBrain.transform.position = BrainSpawnLocation.transform.position;

        if(!_playedTutorial)
        {
            MessageUI.StopAll();
            MessageUI.DisplayMultipleMessages(FirstBrainPick);
            _playedTutorial= true;
        }
    }

}

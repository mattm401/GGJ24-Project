using System.Collections;
using System.Collections.Generic;
using CommandTerminal;
using UnityEngine;

public class RandomSounds : MonoBehaviour
{
    public AudioSource[] allAudioSource;

    public AudioClip[] allAudioClip;

    

    public bool PlaySound(int location = -1, int clipSound = -1)
    {
        AudioSource audioSourceToUse = null;
        AudioClip audioClipToUse = null;
        
        if (location < 0)
        {
            //random location
            
            //is all audio souce being used
            bool allBeingUsed = true;

            foreach (var audioSource in allAudioSource)
            {
                if (audioSource.isPlaying == false)
                {
                    allBeingUsed = false;
                    break;
                }
            }

            if (allBeingUsed == true)
            {
                return false;
            }
            
            
            //now do random until we get something that's not playing
            do
            {
                int randomValue = Random.Range(0, allAudioSource.Length);

                AudioSource tempAudioSource = allAudioSource[randomValue];

                if (tempAudioSource.isPlaying == false)
                {
                    audioSourceToUse = tempAudioSource;
                }

            } while (audioSourceToUse == null);
            
        }
        else
        {
            if (location >= allAudioSource.Length)
            {
                Debug.LogWarning("audio source out of bound");
                return false;
            }

            audioSourceToUse = allAudioSource[location];
            if (audioSourceToUse.isPlaying)
            {
                return false;
            }
        }
        
        
        
        // now get the clip
        if (clipSound < 0)
        {
            //random Clip
            int randomValue = Random.Range(0, allAudioClip.Length);
            audioClipToUse = allAudioClip[randomValue];
        }
        else
        {
            if (clipSound >= allAudioClip.Length)
            {
                Debug.LogWarning("audio clip out of bound");
                return false;
            }

            audioClipToUse = allAudioClip[clipSound];

        }

        
        //we should have the source and the clip play them.
        audioSourceToUse.clip = audioClipToUse;
        audioSourceToUse.PlayOneShot(audioClipToUse);
        
        return false;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        Terminal.Shell.AddCommand("sound", PlaySound, 0, 999, "Play out of bound sounds: arg 1 (int : 1 - 8)= location, arg 2 (int 1 - 41)= sound type");   
        Terminal.Shell.AddCommand("soundall", PlaySoundAll, 0, 999, "play sound at all location random types");   
        
    }

    private void PlaySoundAll(CommandArg[] obj)
    {
        //throw new System.NotImplementedException();

        for (int i = 0; i < allAudioSource.Length; i++)
        {
            PlaySound(i, -1);
        }
        
    }

    private void PlaySound(CommandArg[] obj)
    {
        int location = obj.Length > 0 ? obj[0].Int : 0;
        int soundtype = obj.Length > 1 ? obj[1].Int : 0;

        location -= 1;
        soundtype -= 1;
        PlaySound(location, soundtype);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

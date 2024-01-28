using System;
using System.Collections;
using System.Collections.Generic;
using CommandTerminal;
using UnityEngine;
using Random = UnityEngine.Random;

public class BrainStorm : MonoBehaviour
{

    private Bounds boundsCube;
    
    public int spawnPerFrame = 5;
    public int totalToSpawn = 100;
    public float destroyAfterTimeInSeconds = 10.0f;
    public GameObject prefabToSummon;

    private void Awake()
    {
        boundsCube = GetComponent<Renderer>().bounds;
    }


    // Start is called before the first frame update
    void Start()
    {
        Terminal.Shell.AddCommand("brainstorm", StartBrainStorm, 0, 999, "Summon 1000 brains onto the game");   
  
    }

    private void StartBrainStorm(CommandArg[] obj)
    {
        StartCoroutine(CoBrainStorm());
    }

    private IEnumerator CoBrainStorm()
    {
        int brainSummon = 0;

        while (brainSummon < totalToSpawn)
        {
            yield return null;

            for (int i = 0; i < spawnPerFrame; i++)
            {
                SpawnPrefab();
                brainSummon++;
            }
            
        }
        
    }

    private void SpawnPrefab()
    {
        //find random position.
        Vector3 position = GetRandomPositionInCube();
        Quaternion randomRotation = Random.rotation;
        
        GameObject spawned = Instantiate(prefabToSummon, position, randomRotation, null);
        
        Destroy(spawned, destroyAfterTimeInSeconds);
        
    }

    
    Vector3 GetRandomPositionInCube()
    {
        if (boundsCube != null)
        {

            // Generate random coordinates within the cube's bounds
            float randomX = Random.Range(boundsCube.min.x, boundsCube.max.x);
            float randomY = Random.Range(boundsCube.min.y, boundsCube.max.y);
            float randomZ = Random.Range(boundsCube.min.z, boundsCube.max.z);

            // Create and return a Vector3 with the random coordinates
            return new Vector3(randomX, randomY, randomZ);
        }
        
        return Vector3.zero; // Return a default value (0, 0, 0) in case of an error
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

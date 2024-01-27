using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public string[] blendShapeNames; // Names of your blendshapes
    [Range(-100, 100)]
    public float[] blendShapeValues; // Slider values for each blendshape

    void Start()
    {
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }

        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("Skinned Mesh Renderer not found!");
            return;
        }

        // Initialize blendShapeValues array
        blendShapeValues = new float[blendShapeNames.Length];
    }

    void Update()
    {
        // Update blend shapes based on slider values
        for (int i = 0; i < blendShapeNames.Length; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, blendShapeValues[i]);
        }
    }
}

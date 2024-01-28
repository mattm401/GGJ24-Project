using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Decal : MonoBehaviour
{
    public DecalProjector Projector;
    public Material[] Materials;
    public float MaxLifeTime = 5f;
    public float _lifeTime;
    public float _currTime;

    // Start is called before the first frame update
    void Start()
    {
        Projector.material = Materials[Random.Range(0, Materials.Length)];
        _lifeTime = Random.Range(MaxLifeTime / 2, MaxLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        _currTime += Time.deltaTime;


        if (_currTime > _lifeTime / 2)
        {
            Projector.fadeFactor = 1 - Mathf.InverseLerp(_lifeTime / 2, _lifeTime, _currTime);
        }

        if (_currTime > _lifeTime)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}

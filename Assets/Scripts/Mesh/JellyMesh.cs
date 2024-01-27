using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyMesh : MonoBehaviour
{
    public float Intensity = 1f;
    public float Mass = 1f;
    public float Stiffness = 1f;
    public float Damping = 0.75f;

    private Mesh _originalMesh, _meshClone;
    private MeshRenderer _renderer;
    private JellyVertex[] _jv;
    private Vector3[] _vertexArray;

    // Start is called before the first frame update
    void Awake()
    {
        _originalMesh = GetComponent<MeshFilter>().sharedMesh;
        _meshClone = Instantiate(_originalMesh);
        GetComponent<MeshFilter>().sharedMesh = _meshClone;
        _renderer = GetComponent<MeshRenderer>();

        _jv = new JellyVertex[_meshClone.vertices.Length];
        for(int i = 0; i < _meshClone.vertices.Length; i++)
        {
            _jv[i] = new JellyVertex(i, transform.TransformPoint(_meshClone.vertices[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _vertexArray = _originalMesh.vertices;
        for(int i = 0; i < _jv.Length; i++) 
        {
            Vector3 target = transform.TransformPoint(_vertexArray[_jv[i].ID]);
            float intensity = (1 - (_renderer.bounds.max.y - target.y) / _renderer.bounds.size.y) * Intensity;
            _jv[i].Shake(target, Mass, Stiffness, Damping);
            target = transform.InverseTransformPoint(_jv[i].Position);
            _vertexArray[_jv[i].ID] = Vector3.Lerp(_vertexArray[_jv[i].ID], target, intensity);
        }

        _meshClone.vertices = _vertexArray;
    }

    public class JellyVertex
    {
        public int ID;
        public Vector3 Position;
        public Vector3 Velocity, Force;

        public JellyVertex(int id, Vector3 pos)
        {
            ID = id; 
            Position = pos; 
        }



        public void Shake(Vector3 target, float m, float s, float d)
        {
            Force = (target - Position) * s;
            Velocity = (Velocity + Force / m) * d;
            Position += Velocity;
            if((Velocity + Force + Force / m).magnitude < 0.001f)
            {
                Position = target;
            }
        }
    }

}

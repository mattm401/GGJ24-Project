using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.LockMiniGame
{
    public class LockObject : MonoBehaviour
    {
        private float _currentLevel;
        private float _xRange;
        private float _yRange;
        private Vector3 _originPos;
        private Quaternion _originRot;
        private int node_number;

        // Start is called before the first frame update
        [UsedImplicitly]
        void Start()
        {
            _currentLevel = Random.Range(0.4f, 0.7f);
            _xRange = Random.Range(-65.0f, 65.0f);
            _yRange = Random.Range(-65.0f, 65.0f);
            _originPos = transform.position;
            _originRot = transform.rotation;

            if (gameObject.name.Contains("1"))
            {
                node_number = 1;
            }
            else if (gameObject.name.Contains("2"))
            {
                node_number = 2;
            }
            else if (gameObject.name.Contains("3"))
            {
                node_number = 3;
            }
            else if (gameObject.name.Contains("4"))
            {
                node_number = 4;
            }
        }

        // Update is called once per frame
        [UsedImplicitly]
        void Update()
        {
        
        }

        public void SetLocked(bool locked)
        {
        }

        public float GetCurrentLevel()
        {
            return _currentLevel;
        }

        public void SetCurrentLevel(float level)
        {
            _currentLevel = level;
        }

        public (float, float) GetXy()
        {
            return (_xRange, _yRange);
        }

        public void ResetGame()
        {
            print('!');
            _currentLevel = Random.Range(0.4f, 0.7f);
            _xRange = Random.Range(-65.0f, 65.0f);
            _yRange = Random.Range(-65.0f, 65.0f);
            transform.position = _originPos;
            transform.rotation = _originRot;
        }

        public int GetNodeNumber()
        {
            return node_number;
        }

    }
}

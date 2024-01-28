using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.LockMiniGame
{
    public class LockMiniGame : MonoBehaviour
    {
        public GameObject LockTarget;
        public GameObject DisplayCanvas;
        public AudioClip laughClip;
        public AudioSource audioSource;
        

        public SkinnedMeshRenderer skinnedMeshRenderer; //MeshRenderer for Face
        public string[] blendShapeNames; // Names of face blendshapes
        [Range(-100, 100)]
        public float[] blendShapeValues;

        public bool _debug = false;
        public bool _andyDebug = true;

        
        private const float FadeTime = 0.75f;
        private const float HealthOn = 1.0f;
        private const float HealthOff = 0.0f;
        private const float HealthFillRate = 0.001f;
        private const float HealthDefillRate = 0.0001f;
        private const float MouseClampDist = 75.0f;
        private const int faceDistAmount = 5;
        
        private bool _displayActive;
        private bool _mouseOverSphere;
        private Vector2 _mousePosOrigin;

        private Image _border;
        private Image _background;
        private Image _health;

        private GameObject _currBrain;

        // Start is called before the first frame update
        [UsedImplicitly]
        void Start()
        {
            // Find the images for the health bar UI in the attached display object
            _border = DisplayCanvas.GetComponentsInChildren<Image>()[0];
            _background = DisplayCanvas.GetComponentsInChildren<Image>()[1];
            _health = DisplayCanvas.GetComponentsInChildren<Image>()[2];

            InitializeFace();
        }

        // Update is called once per frame
        [UsedImplicitly]
        void Update()
        {
            if (GameManager.Instance.IsResetNeeded())
            {
                Reset();
                GameManager.Instance.SetResetNeeded(false);
            }

            CheckForBrain();
            DetectLockContact();
            UpdateLockDisplayBar();

            // Determine how to change value of health bar
            if (_displayActive)
            {
                // Override mouse position for screen position (for testing purposes)
                var mousePosNew = Input.mousePosition;
                mousePosNew.x -= _mousePosOrigin.x;
                mousePosNew.y -= _mousePosOrigin.y;
                
                // Rotate object (in the reference frame of the top-down camera, hence why the next Vector3 has non-intuitive x,y,z usages) 
                LockTarget.transform.LookAt(new Vector3(mousePosNew.x, -Camera.main.transform.position.z, mousePosNew.y));
                (float mouseX, float mouseY) = ClampMouseValues(mousePosNew);
                (float targX, float targY) = LockTarget.GetComponentInChildren<LockObject>().GetXy();

                var yLeft = targY - 20.0f;
                var yRight = targY + 20.0f;
                var xLeft = targX - 20.0f;
                var xRight = targX + 20.0f;

                if (_health.fillAmount < 1.0f && mouseX > xLeft && mouseX < xRight && mouseY > yLeft && mouseY < yRight)
                {
                    _health.fillAmount += HealthFillRate;
                    LockTarget.GetComponentInChildren<LockObject>().SetCurrentLevel(_health.fillAmount);
                }
                else if (_health.fillAmount >= 1.0f && mouseX > xLeft && mouseX < xRight && mouseY > yLeft && mouseY < yRight)
                {
                    // Add particle effect (audio will play on object)
                    if (!LockTarget.transform.Find("ElectricitySphere").gameObject.activeSelf)
                    {
                        LockTarget.transform.Find("ElectricitySphere").gameObject.SetActive(true);
                       
                        // Hacky Brain Score Assignment (TODO: Clean this up).
                        switch (LockTarget.GetComponent<LockObject>().GetNodeNumber())
                        {
                            case 1:
                                SetFaceDistortion(0);
                                _currBrain.GetComponent<BrainController>().Node1Score = 1;
                                break;
                            case 2:
                                SetFaceDistortion(1);
                                SetFaceDistortion(7);
                                _currBrain.GetComponent<BrainController>().Node2Score = 1;
                                break;
                            case 3:
                                SetFaceDistortion(2);
                                _currBrain.GetComponent<BrainController>().Node3Score = 1;
                                break;
                            case 4:
                                SetFaceDistortion(4);
                                _currBrain.GetComponent<BrainController>().Node4Score = 1;
                                break;
                        }

                        
                        var locks = GameObject.FindGameObjectsWithTag("Lock");
                        var winCondition = true;
                        for (var i = 0; i < locks.Length; i++) 
                        {
                            if (!locks[i].transform.Find("ElectricitySphere").gameObject.activeSelf)
                            {
                                winCondition = false;
                            }
                        }
                        if (winCondition)
                        {
                            playLaugh();
                            if (Input.GetKeyDown(KeyCode.Escape))
                            {
                                for (var i = 0; i < locks.Length; i++)
                                {
                                    locks[i].transform.Find("ElectricitySphere").gameObject.SetActive(false);
                                    locks[i].GetComponentInChildren<LockObject>().SetCurrentLevel(0.3f);
                                    locks[i].GetComponentInChildren<LockObject>().ResetGame();
                                }

                                _currBrain = null;
                                _displayActive = false;
                                StartCoroutine(FadeTo(_border, HealthOff, FadeTime));
                                StartCoroutine(FadeTo(_background, HealthOff, FadeTime));
                                StartCoroutine(FadeTo(_health, HealthOff, FadeTime));
                                GameManager.Instance.TurnOffMiniGame();
                            }
                            
                        }
                    }
                }
                else
                {
                    _health.fillAmount -= HealthDefillRate;
                    LockTarget.GetComponentInChildren<LockObject>().SetCurrentLevel(_health.fillAmount);
                }
            }
            UpdateFace();
        }
        IEnumerator FadeTo(Image layer, float aValue, float aTime)
        {
            // Helper functions for fading in and out Images/Sprites
            float alpha = layer.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color newColor = new Color(layer.color.r, layer.color.g, layer.color.b, Mathf.Lerp(alpha, aValue, t));
                layer.color = newColor;
                yield return null;
            }
        }

        private void UpdateLockDisplayBar()
        {
            // Visibility of health bar UI based on mouse interaction
            if (Input.GetMouseButtonDown(0) && _health.color.a <= 1.0f && _mouseOverSphere)
            {
                if (_debug) Debug.Log("Mouse Down");

                _displayActive = true;
                StartCoroutine(FadeTo(_border, HealthOn, FadeTime));
                StartCoroutine(FadeTo(_background, HealthOn, FadeTime));
                StartCoroutine(FadeTo(_health, HealthOn, FadeTime));
                _mousePosOrigin = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0) && _health.color.a != 0.0f)
            {
                if (_debug) Debug.Log("Mouse Up");

                _displayActive = false;
                StartCoroutine(FadeTo(_border, HealthOff, FadeTime));
                StartCoroutine(FadeTo(_background, HealthOff, FadeTime));
                StartCoroutine(FadeTo(_health, HealthOff, FadeTime));
            }
        }

        private void DetectLockContact()
        {
            // Ray casting to determine if mouse is over object
            Ray ray;
            RaycastHit hit;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if(_debug) Debug.Log("Mouse Touching: " + hit.collider.name);
                if (hit.collider.name.Contains("LockPoint"))
                {
                    LockTarget = hit.collider.gameObject;
                    _health.fillAmount = LockTarget.GetComponent<LockObject>().GetCurrentLevel();
                    _mouseOverSphere = true;
                }
                else
                {
                    _mouseOverSphere = false;
                }
            }
            else
            {
                _mouseOverSphere = false;
            }
        }

        private (float, float) ClampMouseValues(Vector3 mousePosNew)
        {
            var mouseX = mousePosNew.x;
            var mouseY = mousePosNew.y;

            if (mousePosNew.x > MouseClampDist)
            {
                mouseX = MouseClampDist;
            }

            if (mousePosNew.x < -MouseClampDist)
            {
                mouseX = -MouseClampDist;
            }

            if (mousePosNew.y > MouseClampDist)
            {
                mouseY = MouseClampDist;
            }

            if (mousePosNew.y < -MouseClampDist)
            {
                mouseY = -MouseClampDist;
            }

            if (_debug) Debug.Log("Clamped Mouse Pos: (" + mouseX + ", " + mouseY + ")");

            return (mouseX, mouseY);
        }

        private void CheckForBrain()
        {
            if (_currBrain == null)
            {
                if(_debug)Debug.Log("Awake-Mini-Game");
                GameObject[] availableBrains = GameObject.FindGameObjectsWithTag("Interactable");
                for (int i = 0; i < availableBrains.Length; i++)
                {
                    if (availableBrains[i].GetComponent<BrainController>() != null)
                    {
                        if (availableBrains[i].GetComponent<BrainController>().BeingCarried)
                        {
                            _currBrain = availableBrains[i];
                            break;
                        }
                    }
                }
            }
        }

        private void InitializeFace()
        {
            GameObject faceObject = GameObject.FindGameObjectWithTag("Face");
            if (faceObject != null)
            {
                skinnedMeshRenderer = faceObject.GetComponent<SkinnedMeshRenderer>();
            }

            if (skinnedMeshRenderer == null)
            {
                if(_andyDebug) Debug.LogError("Skinned Mesh Renderer not found!");
                return;
            }

            // Initialize blendShapeValues array
            int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
            blendShapeValues = new float[blendShapeCount];

            PopulateBlendShapeNames();
        }

        private void PopulateBlendShapeNames()
        {
            int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
            blendShapeNames = new string[blendShapeCount];

            for (int i = 0; i < blendShapeCount; i++)
            {
                blendShapeNames[i] = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
            }
        }

        private void SetFaceDistortion(int nodeNumber)
        {
            blendShapeValues[nodeNumber] = 100f;
        }

        private void UpdateFace()
        {
            for (int i = 0; i < blendShapeValues.Length; i++)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(i, blendShapeValues[i]);
            }
        }

        private void playLaugh()
        {
            
            if (audioSource != null && laughClip != null)
            {
                audioSource.clip = laughClip;
                audioSource.volume = 0.5f;
                audioSource.Play();
            }
            else
            {
                if(_andyDebug)Debug.LogError("LaughAudio AudioSource and/or laugh clip not found!");
            }
        }

        private void Reset()
        {
            var locks = GameObject.FindGameObjectsWithTag("Lock");
            for (var i = 0; i < locks.Length; i++)
            {
                locks[i].transform.Find("ElectricitySphere").gameObject.SetActive(false);
                locks[i].GetComponentInChildren<LockObject>().SetCurrentLevel(0.3f);
                locks[i].GetComponentInChildren<LockObject>().ResetGame();
            }

            _currBrain = null;
            _displayActive = false;
            StartCoroutine(FadeTo(_border, HealthOff, FadeTime));
            StartCoroutine(FadeTo(_background, HealthOff, FadeTime));
            StartCoroutine(FadeTo(_health, HealthOff, FadeTime));

            if (_currBrain != null)
            {
                _currBrain.GetComponent<BrainController>().Node1Score = 0;
                _currBrain.GetComponent<BrainController>().Node2Score = 0;
                _currBrain.GetComponent<BrainController>().Node3Score = 0;
                _currBrain.GetComponent<BrainController>().Node4Score = 0;
            }
        }
    }

}

/* References
 
    Electric Effects: 
    https://www.youtube.com/watch?v=RdNnbozAPGQ&ab_channel=HovlStudio
    https://freesound.org/people/newlocknew/sounds/702908/

 */

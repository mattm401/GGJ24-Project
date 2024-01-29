using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace Assets.Scripts.LockMiniGame
{
    public class LockMiniGame : MonoBehaviour
    {
        public GameObject LockTarget;
        public GameObject DisplayCanvas;
        public AudioClip laughClip;
        public AudioSource audioSource;

        public AudioSource audioSourceOh;
        

        public BrainStatus BrainStatus;
        public TutorialMessageDisplay TutorialMessageDisplay;
        public List<TutorialMessage> MiniGameTutorialMessages;
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
        private bool _tutorialDisplayed;
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
                CheckForBrain();
                UpdateBrainStates();
            }
            if(!_tutorialDisplayed)
            {
                TutorialMessageDisplay.StopAll();
                TutorialMessageDisplay.DisplayMultipleMessages(MiniGameTutorialMessages);
                _tutorialDisplayed = true;
            }
            CheckForBrain();
            DetectLockContact();
            UpdateLockDisplayBar();

            // Determine how to change value of health bar
            if (LockTarget != null && _displayActive && !LockTarget.GetComponent<LockObject>().getLocked())
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
                    switch (LockTarget.GetComponent<LockObject>().GetNodeNumber())
                    {
                        case 1:
                            _currBrain.GetComponent<BrainController>().Node1Score = _health.fillAmount;
                            break;
                        case 2:
                            _currBrain.GetComponent<BrainController>().Node2Score = _health.fillAmount;
                            break;
                        case 3:
                            _currBrain.GetComponent<BrainController>().Node3Score = _health.fillAmount;
                            break;
                        case 4:
                            _currBrain.GetComponent<BrainController>().Node4Score = _health.fillAmount;
                            break;
                    }
                }
                else if (_health.fillAmount >= 1.0f && mouseX > xLeft && mouseX < xRight && mouseY > yLeft && mouseY < yRight)
                {
                    // Add particle effect (audio will play on object)
                    if (!LockTarget.transform.Find("ElectricitySphere").gameObject.activeSelf)
                    {
                        LockTarget.transform.Find("ElectricitySphere").gameObject.SetActive(true);
                        LockTarget.GetComponentInChildren<LockObject>().setLocked(true);


                        // Hacky Brain Score Assignment (TODO: Clean this up).
                        switch (LockTarget.GetComponent<LockObject>().GetNodeNumber())
                        {
                            case 1:
                                SetFaceDistortion(0);
                                _currBrain.GetComponent<BrainController>().Node1Score = 1;
                                _currBrain.GetComponent<BrainController>().Node1Works = false;
                                break;
                            case 2:
                                SetFaceDistortion(1);
                                SetFaceDistortion(7);
                                _currBrain.GetComponent<BrainController>().Node2Score = 1;
                                _currBrain.GetComponent<BrainController>().Node2Works = false;
                                break;
                            case 3:
                                SetFaceDistortion(2);
                                _currBrain.GetComponent<BrainController>().Node3Score = 1;
                                _currBrain.GetComponent<BrainController>().Node3Works = false;
                                break;
                            case 4:
                                SetFaceDistortion(4);
                                _currBrain.GetComponent<BrainController>().Node4Score = 1;
                                _currBrain.GetComponent<BrainController>().Node4Works = false;
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

                                _displayActive = false;
                                BrainStatus.EndDroppingIntegrity();
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
                    if (_health.fillAmount <= 0.0f)
                    {
                        playOuch();
                        LockTarget.GetComponentInChildren<LockObject>().setLocked(true);
                        switch (LockTarget.GetComponent<LockObject>().GetNodeNumber())
                        {
                            case 1:
                                _currBrain.GetComponent<BrainController>().Node1Score = _health.fillAmount;
                                _currBrain.GetComponent<BrainController>().Node1Works = false;
                                break;
                            case 2:
                                _currBrain.GetComponent<BrainController>().Node2Score = _health.fillAmount;
                                _currBrain.GetComponent<BrainController>().Node2Works = false;
                                break;
                            case 3:
                                _currBrain.GetComponent<BrainController>().Node3Score = _health.fillAmount;
                                _currBrain.GetComponent<BrainController>().Node3Works = false;
                                break;
                            case 4:
                                _currBrain.GetComponent<BrainController>().Node4Score = _health.fillAmount;
                                _currBrain.GetComponent<BrainController>().Node4Works = false;
                                break;
                        }
                    }
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


                if (!LockTarget.GetComponent<LockObject>().getLocked())
                {

                    _displayActive = true;
                    StartCoroutine(FadeTo(_border, HealthOn, FadeTime));
                    StartCoroutine(FadeTo(_background, HealthOn, FadeTime));
                    StartCoroutine(FadeTo(_health, HealthOn, FadeTime));
                    _mousePosOrigin = Input.mousePosition;
                    BrainStatus.BeginDroppingIntegrity();
                }
                else
                {
                    LockTarget.GetComponent<AudioSource>().Play();
                }

            }

            if (Input.GetMouseButtonUp(0) && _health.color.a != 0.0f)
            {
                if (_debug) Debug.Log("Mouse Up");

                _displayActive = false;
                StartCoroutine(FadeTo(_border, HealthOff, FadeTime));
                StartCoroutine(FadeTo(_background, HealthOff, FadeTime));
                StartCoroutine(FadeTo(_health, HealthOff, FadeTime));

                BrainStatus.EndDroppingIntegrity();
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

        private void ResetFace()
        {
            for (int i = 0; i < blendShapeValues.Length; i++)
            {
                blendShapeValues[i] = 0f;
                skinnedMeshRenderer.SetBlendShapeWeight(i, blendShapeValues[i]);
            }
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

        private void playOuch()
        {

            if (audioSourceOh != null)
            {
                //audioSourceOh.clip = laughClip;
                audioSourceOh.volume = 0.5f;
                audioSourceOh.Play();
            }
            else
            {
                if (_andyDebug) Debug.LogError("LaughAudio AudioSource and/or laugh clip not found!");
            }
        }

        private void Reset()
        {
            ResetFace();
            var locks = GameObject.FindGameObjectsWithTag("Lock");
            for (var i = 0; i < locks.Length; i++)
            {
                locks[i].transform.Find("ElectricitySphere").gameObject.SetActive(false);
                locks[i].GetComponentInChildren<LockObject>().SetCurrentLevel(0.3f);
                locks[i].GetComponentInChildren<LockObject>().ResetGame();
            }

            _currBrain = null;
            _displayActive = false;
            BrainStatus.EndDroppingIntegrity();
            StartCoroutine(FadeTo(_border, HealthOff, FadeTime));
            StartCoroutine(FadeTo(_background, HealthOff, FadeTime));
            StartCoroutine(FadeTo(_health, HealthOff, FadeTime));
            BrainStatus.SetBrain();
        }

        public void UpdateBrainStates()
        {
            print("!");

            if (_currBrain == null)
            {
                GameManager.Instance.SetResetNeeded(false);
                return;
            }

            print("!!");
            var locks = GameObject.FindGameObjectsWithTag("Lock");
            print(locks.Length);
            for (var i = 0; i < locks.Length; i++)
            {

                switch (locks[i].GetComponent<LockObject>().GetNodeNumber())
                {
                    case 1:
                        if (!_currBrain.GetComponent<BrainController>().Node1Works)
                        {
                            if (_currBrain.GetComponent<BrainController>().Node1Score == 1.0f)
                                locks[i].transform.Find("ElectricitySphere").gameObject.SetActive(true);
                            locks[i].GetComponent<LockObject>().setLocked(true);
                        }
                        locks[i].GetComponentInChildren<LockObject>().SetCurrentLevel(_currBrain.GetComponent<BrainController>().Node1Score);
                        break;
                    case 2:
                        if (!_currBrain.GetComponent<BrainController>().Node2Works)
                        {
                            if (_currBrain.GetComponent<BrainController>().Node2Score == 1.0f)
                                locks[i].transform.Find("ElectricitySphere").gameObject.SetActive(true);
                            locks[i].GetComponent<LockObject>().setLocked(true);

                        }
                        locks[i].GetComponentInChildren<LockObject>().SetCurrentLevel(_currBrain.GetComponent<BrainController>().Node1Score);
                        break;
                    case 3:
                        if (!_currBrain.GetComponent<BrainController>().Node3Works)
                        {
                            if (_currBrain.GetComponent<BrainController>().Node3Score == 1.0f)
                                locks[i].transform.Find("ElectricitySphere").gameObject.SetActive(true);
                            locks[i].GetComponent<LockObject>().setLocked(true);

                        }
                        locks[i].GetComponentInChildren<LockObject>().SetCurrentLevel(_currBrain.GetComponent<BrainController>().Node1Score);
                        break;
                    case 4:
                        if (!_currBrain.GetComponent<BrainController>().Node4Works)
                        {

                            if (_currBrain.GetComponent<BrainController>().Node4Score == 1.0f) 
                                locks[i].transform.Find("ElectricitySphere").gameObject.SetActive(true);

                            locks[i].GetComponent<LockObject>().setLocked(true);
                            print("Locking 4");
                        }
                        locks[i].GetComponentInChildren<LockObject>().SetCurrentLevel(_currBrain.GetComponent<BrainController>().Node1Score);
                        break;
                }
            }
        }
    }

}

/* References
 
    Electric Effects: 
    https://www.youtube.com/watch?v=RdNnbozAPGQ&ab_channel=HovlStudio
    https://freesound.org/people/newlocknew/sounds/702908/

 */

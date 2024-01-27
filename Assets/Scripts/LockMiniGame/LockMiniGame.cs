using System.Collections;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.LockMiniGame
{
    public class LockMiniGame : MonoBehaviour
    {
        public GameObject LockTarget;
        public GameObject DisplayCanvas;

        private readonly bool _debug = false;
        
        private readonly float _lockRotationSpeed = 0.01f;
        
        private const float FadeTime = 0.75f;
        private const float HealthOn = 1.0f;
        private const float HealthOff = 0.0f;
        private const float HealthFillRate = 0.001f;
        private const float HealthDefillRate = 0.002f;
        private const float MouseClampDist = 200.0f;
        
        private bool _displayActive;
        private bool _mouseOverSphere;
        private Vector2 _mousePosOrigin;

        private Image _border;
        private Image _background;
        private Image _health;

        // Start is called before the first frame update
        [UsedImplicitly]
        void Start()
        {
            // Find the images for the health bar UI in the attached display object
            _border = DisplayCanvas.GetComponentsInChildren<Image>()[0];
            _background = DisplayCanvas.GetComponentsInChildren<Image>()[1];
            _health = DisplayCanvas.GetComponentsInChildren<Image>()[2];
        }

        // Update is called once per frame
        [UsedImplicitly]
        void Update()
        {
            

            DetectLockContact();
            UpdateLockDisplayBar();

            // Determine how to change value of health bar
            if (_displayActive)
            {

                // Get mouse position
                float mouseX;
                float mouseY;

                // Override mouse position for screen position (for testing purposes)
                var mousePosNew = Input.mousePosition;
                mousePosNew.x -= _mousePosOrigin.x;
                mousePosNew.y -= _mousePosOrigin.y;
                mouseX = mousePosNew.x;
                mouseY = mousePosNew.y;

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

                // Rotate object
                LockTarget.transform.LookAt(new Vector3(-mousePosNew.x, mousePosNew.y, Camera.main.transform.position.z));


                if (_debug) Debug.Log("Clamped Mouse Pos: (" + mouseX + ", " + mouseY + ")");
                
                if (_health.fillAmount < 1.0f && mouseY > 0 && (mouseX >= -10.0f && mouseX <= 10.0f))
                {
                    _health.fillAmount += HealthFillRate;
                }
                else if (_health.fillAmount >= 1.0f && mouseY > 0 && (mouseX >= -10.0f && mouseX <= 10.0f))
                {
                    // Add particle effect (audio will play on object)
                    if (!LockTarget.transform.Find("ElectricitySphere").gameObject.activeSelf)
                    {
                        LockTarget.transform.Find("ElectricitySphere").gameObject.SetActive(true);
                    }
                }
                else
                {
                    _health.fillAmount -= HealthDefillRate;
                }
            }
            else
            {
                if (_health.fillAmount > 0.0f && _health.fillAmount < 1.0f)
                {
                    _health.fillAmount -= HealthDefillRate;
                }
            }
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
            // Raycasting to determine if mouse is over object
            var ray = new Ray();
            var hit = new RaycastHit();

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Mouse Touching: " + hit.collider.name);
                LockTarget = hit.collider.gameObject;
                _health.fillAmount = LockTarget.GetComponent<LockObject>().getCurrentLevel();
                _mouseOverSphere = true;
            }
            else
            {
                _mouseOverSphere = false;
            }
        }
    }
}

/* References
 
    Electric Effects: 
    https://www.youtube.com/watch?v=RdNnbozAPGQ&ab_channel=HovlStudio
    https://freesound.org/people/newlocknew/sounds/702908/

 */

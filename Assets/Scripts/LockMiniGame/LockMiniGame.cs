using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.LockMiniGame
{
    public class LockMiniGame : MonoBehaviour
    {
        public GameObject LockTarget;
        public GameObject DisplayCanvas;

        private readonly bool _debug = false;
        
        private readonly float _lockRotationSpeed = 4.0f;
        
        private const float FadeTime = 1.0f;
        private const float HealthOn = 1.0f;
        private const float HealthOff = 0.0f;
        private const float HealthFillRate = 0.001f;
        private bool _displayActive;
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

            // Spin the object around the target at 20 degrees/second; here this will only initialize the location and look direction of the camera
            transform.RotateAround(LockTarget.transform.position, Vector3.up, 20 * Time.deltaTime);
            this.transform.LookAt(LockTarget.transform.position);
        }

        // Update is called once per frame
        [UsedImplicitly]
        void Update()
        {
            // Visibility of health bar UI based on mouse interaction
            if (Input.GetMouseButtonDown(0) && _health.color.a <= 1.0f)
            {
                if (_debug) Debug.Log("Mouse Down");
                
                _displayActive = true;
                StartCoroutine(FadeTo(_border, HealthOn, FadeTime));
                StartCoroutine(FadeTo(_background, HealthOn, FadeTime));
                StartCoroutine(FadeTo(_health, HealthOn, FadeTime));
            }
        
            if (Input.GetMouseButtonUp(0) && _health.color.a != 0.0f)
            {
                if (_debug) Debug.Log("Mouse Up");
    
                _displayActive = false;
                StartCoroutine(FadeTo(_border, HealthOff, FadeTime));
                StartCoroutine(FadeTo(_background, HealthOff, FadeTime));
                StartCoroutine(FadeTo(_health, HealthOff, FadeTime));
            }

            // Determine how to change value of health bar
            if (_displayActive)
            {
                if (_health.fillAmount < 1.0f)
                {
                    _health.fillAmount += HealthFillRate;
                }
                else
                {
                    // Add particle effect (audio will play on object)
                    if (!LockTarget.transform.Find("EventSystem/ElectricitySphere").gameObject.activeSelf)
                    {
                        LockTarget.transform.Find("EventSystem/ElectricitySphere").gameObject.SetActive(true);
                    }
                }

                // Get the current mouse position
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // Rotate the object around its y-axis
                LockTarget.transform.Rotate(new Vector3(0, mouseY * _lockRotationSpeed, 0));

                // Rotate the object around its x-axis
                LockTarget.transform.Rotate(new Vector3(-mouseX * _lockRotationSpeed, 0, 0));

                // Place the object in front of the camera
                LockTarget.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 10.0f;

            }
            else
            {
                if (LockTarget.transform.Find("EventSystem/ElectricitySphere").gameObject.activeSelf)
                {
                    // Add particle effect (audio will stop playing on object)
                    LockTarget.transform.Find("EventSystem/ElectricitySphere").gameObject.SetActive(false);
                }

                if (_health.fillAmount > 0.0f)
                {
                    _health.fillAmount -= HealthFillRate;
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
    }
}

/* References
 
    Electric Effects: 
    https://www.youtube.com/watch?v=RdNnbozAPGQ&ab_channel=HovlStudio
    https://freesound.org/people/newlocknew/sounds/702908/

 */

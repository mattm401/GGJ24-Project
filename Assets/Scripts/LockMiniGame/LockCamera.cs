using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LockCamera : MonoBehaviour
{

    public GameObject LockTarget;
    public GameObject DisplayCanvas;

    private bool DisplayActive = false;
    private Image _border;
    private Image _background;
    private Image _health;

    // Start is called before the first frame update
    void Start()
    {
        _border = DisplayCanvas.GetComponentsInChildren<Image>()[0];
        _background = DisplayCanvas.GetComponentsInChildren<Image>()[1];
        _health = DisplayCanvas.GetComponentsInChildren<Image>()[2];
    }

    // Update is called once per frame
    void Update()
    {
        // Spin the object around the target at 20 degrees/second.
        transform.RotateAround(LockTarget.transform.position, Vector3.up, 20 * Time.deltaTime);
        this.transform.LookAt(LockTarget.transform.position);

        if (Input.GetMouseButtonDown(0) && _health.color.a <= 1.0f)
        {
            Debug.Log("Mouse Down");
            DisplayActive = true;

            StartCoroutine(FadeTo(_border, 1.0f, 1.0f));
            StartCoroutine(FadeTo(_background, 1.0f, 1.0f));
            StartCoroutine(FadeTo(_health, 1.0f, 1.0f));
        }
        
        if (Input.GetMouseButtonUp(0) && _health.color.a != 0.0f)
        {
            Debug.Log("Mouse Up");
            DisplayActive = false;

            StartCoroutine(FadeTo(_border, 0.0f, 1.0f));
            StartCoroutine(FadeTo(_background, 0.0f, 1.0f));
            StartCoroutine(FadeTo(_health, 0.0f, 1.0f));
        }

        if (DisplayActive)
        {
            if (_health.fillAmount < 1.0f)
            {
                _health.fillAmount += 0.001f;
            }
            else
            {
                if (!LockTarget.transform.Find("EventSystem/ElectricitySphere").gameObject.activeSelf)
                {
                    LockTarget.transform.Find("EventSystem/ElectricitySphere").gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (LockTarget.transform.Find("EventSystem/ElectricitySphere").gameObject.activeSelf)
            {
                LockTarget.transform.Find("EventSystem/ElectricitySphere").gameObject.SetActive(false);
            }

            if (_health.fillAmount > 0.0f)
            {
                _health.fillAmount -= 0.001f;
            }
        }
    }

    IEnumerator FadeTo(Image layer, float aValue, float aTime)
    {
        float alpha = layer.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(layer.color.r, layer.color.g, layer.color.b, Mathf.Lerp(alpha, aValue, t));
            layer.color = newColor;
            yield return null;
        }
    }
}

/* References
 
    Electric Effects: 
    https://www.youtube.com/watch?v=RdNnbozAPGQ&ab_channel=HovlStudio
    https://freesound.org/people/newlocknew/sounds/702908/

 */

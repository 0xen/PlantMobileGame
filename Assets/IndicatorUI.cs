using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorUI : MonoBehaviour
{

    public float bounceSpeed;
    public float bounceAmount;
    public float rotationSpeed;

    RectTransform recTransform;

    float scaleDelta;
    // Start is called before the first frame update
    void Start()
    {
        recTransform = GetComponent<RectTransform>();
        scaleDelta = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        scaleDelta += Time.deltaTime;

        recTransform.Rotate(new Vector3(0, 0, rotationSpeed));
        float scale = 1.0f + (Mathf.Sin(bounceSpeed * scaleDelta) * bounceAmount);
        recTransform.localScale = new Vector3(scale, scale, scale);
    }
}

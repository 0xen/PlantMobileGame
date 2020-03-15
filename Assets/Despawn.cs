using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    private Vector2 screenDim;
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        screenDim = new Vector2();
        screenDim.y = camera.orthographicSize * 2;
        screenDim.x = screenDim.y * camera.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if(camera.transform.position.y>transform.position.y+screenDim.magnitude)
        {
            Destroy(gameObject);
        }
    }
}

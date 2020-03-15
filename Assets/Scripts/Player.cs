using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameController controller;
    public float turnSpeed;
    public float growthSpeed;
    public GameObject flowerHead;
    public GameObject[] flowerHeadParts;
    public GameObject helm;

    private Vector2 screenDim;
    private Vector2 screenHalfDim;
    // Start is called before the first frame update
    void Start()
    {
        screenDim = new Vector2();
        screenDim.y = Camera.main.orthographicSize * 2;
        screenDim.x = screenDim.y * Camera.main.aspect;
        screenHalfDim = new Vector2(screenDim.x / 2, screenDim.y / 2);
    }


    public Vector2 BezierQuadratic(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        Vector2 p0 = Vector2.Lerp(a, b, t);
        Vector2 p1 = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(p0, p1, t);
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.GetState() == GameController.GameStates.Playing)
        {
            
            for(int i = 0; i < 3;i++)
            {
                flowerHeadParts[i].SetActive(i < controller.health);
            }

            Vector2 forwardDirection = new Vector2(turnSpeed * Time.deltaTime * Input.acceleration.x, growthSpeed * Time.deltaTime);
            transform.position += new Vector3(forwardDirection.x, forwardDirection.y, 0.0f);

            forwardDirection.Normalize();
            float angle = Vector3.Angle(forwardDirection, new Vector2(0, 1));
            flowerHead.transform.rotation = new Quaternion();
            flowerHead.transform.Rotate(0, 0, forwardDirection.x > 0 ? -angle : angle);

            // Flower Head Growing
            if (transform.position.y > 5 && transform.position.y < 10)
            {
                flowerHead.active = true;
                float scale = Mathf.Lerp(0.01f, 1.0f, (transform.position.y - 5) * 0.2f);
                flowerHead.transform.localScale = new Vector3(scale, scale, scale);
                Destroy(GetComponent<CircleCollider2D>());
            }
            // Helm Active
            if (transform.position.y > 420)
            {
                Destroy(flowerHead.GetComponent<BoxCollider2D>());
                helm.active = true;
            }
        }
    }
}

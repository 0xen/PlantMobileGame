using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : MonoBehaviour
{
    public float speed;
    public bool changeDirection;
    public float changeDirectionTimer;
    // is the facing direction of the sprite in such a way that it would be moving right
    public bool defaultRight;
    SpriteRenderer renderer;
    float changeDirectionDelta;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.flipX = Random.Range(0, 2) == 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(changeDirection)
        {
            changeDirectionDelta += Time.deltaTime;
            if(changeDirectionTimer < changeDirectionDelta)
            {
                changeDirectionDelta = 0.0f;
                renderer.flipX = !renderer.flipX;
            }
        }
        float newSpeed = speed * Time.deltaTime;
        if(defaultRight && renderer.flipX || !defaultRight && !renderer.flipX)
        {
            newSpeed = -newSpeed;
        }
        transform.position += new Vector3(newSpeed, 0.0f, 0.0f);
    }
}

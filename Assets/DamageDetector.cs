using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetector : MonoBehaviour
{
    public GameController controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == null || !col.gameObject.tag.Contains("Solid")) return;

        controller.DealDamage();
        Debug.Log("OnCollisionEnter2D");
        SFXOnHit sfx = col.gameObject.GetComponent<SFXOnHit>();
        if(sfx)
        {
            SFXController.instance.Play(sfx.sfx);
        }

        Destroy(col.gameObject);
    }
}

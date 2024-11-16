using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionController : MonoBehaviour
{
    public string _tagTargetDetection = "Player";

    public List<Collider2D> detectedObjs = new List<Collider2D>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == _tagTargetDetection)
        {
            detectedObjs.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == _tagTargetDetection)
        {
            detectedObjs.Remove(collision);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float bulletSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * bulletSpeed * Time.deltaTime);
    }
}

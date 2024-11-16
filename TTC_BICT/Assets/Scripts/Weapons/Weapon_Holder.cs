using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Holder : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private float offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleAiming();
    }

    private void HandleAiming()
    {
        // Rotation
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        // Position
        Vector3 playerToMouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.position;
        playerToMouseDir.z = 0;
        transform.position = player.position + (offset * playerToMouseDir.normalized);

        // Flip
        Vector3 localScale = Vector3.one;

        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }

        transform.localScale = localScale;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _playerRigidbody2D;
    private Animator    _playerAnimator;
    public float        _playerSpeed;
    private float       _playerInitialSpeed;
    public float        _playerRunSpeed;
    private Vector2     _playerDirection;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();

        _playerInitialSpeed = _playerSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        //Flip();

        PlayerRun();

    }

    void FixedUpdate()
    {
        _playerDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (_playerDirection.sqrMagnitude > 0.1) 
        {
            MovePlayer();
            _playerAnimator.SetFloat("AxisX", _playerDirection.x);
            _playerAnimator.SetFloat("AxisY", _playerDirection.y);
            _playerAnimator.SetInteger("Movimento", 1);
        }

        else
        {
            _playerAnimator.SetInteger("Movimento", 0);
        }
    }

    void MovePlayer()
    {
        _playerRigidbody2D.MovePosition(_playerRigidbody2D.position + _playerDirection * _playerSpeed * Time.fixedDeltaTime);
    }



    void Flip()
    {
        if (_playerDirection.x > 0)
        {
            transform.eulerAngles = new Vector2(0f, 0f);
        }

        else if (_playerDirection.x < 0)
        {
            transform.eulerAngles = new Vector2(0f, 180);
        }

    }

    void PlayerRun()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            _playerSpeed = _playerRunSpeed;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _playerSpeed = _playerInitialSpeed;
        }
    }
}
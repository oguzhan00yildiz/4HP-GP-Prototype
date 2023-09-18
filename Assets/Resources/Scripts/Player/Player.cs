using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement _movement;
    private Camera _playerCam;
    private CameraMouseLook _mouseLook;

    private int _health;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        _movement = GetComponent<PlayerMovement>();
        _playerCam = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        _mouseLook = _playerCam.GetComponent<CameraMouseLook>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

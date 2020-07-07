using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 offset;
    public GameObject Player;

    void Start()
    {
        offset = transform.position - Player.transform.position;
    }

    void Update()
    {
        transform.position = Player.transform.position + offset;

    }
    
    
}

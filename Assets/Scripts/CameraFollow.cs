using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _player;
    
    void Update () 
    {
        transform.position = _player.transform.position + new Vector3(0, 10, -10);
    }
}

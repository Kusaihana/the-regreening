using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] Vector3 _cameraOffset;
    void Update () 
    {
        transform.position = _player.transform.position + _cameraOffset;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Followable : MonoBehaviour
{
    public void OnMouseDown()
    {
        CameraController.instance.followTransform = transform;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnchor : MonoBehaviour
{
    public Vector3 posOffset = Vector3.zero;
    public Quaternion rotOffset = Quaternion.identity;

    public bool isSnapped = false;

    public float positionSpeed = 50f;
    public float rotationSpeed = 50f;

    public Vector3 GetPosition()
    {
        return transform.position + posOffset;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation * rotOffset;
    }
}

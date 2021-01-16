using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Anchor : MonoBehaviour
{
    public Block blockRef;
    public Rigidbody targetRB;
    public bool isUse = false;

    public Anchor linkedAnchor = null;

    private void Awake()
    {
        if(targetRB == null)
            targetRB = transform.parent.GetComponent<Rigidbody>();
    }
}

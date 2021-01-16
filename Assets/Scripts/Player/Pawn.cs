using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Pawn : MonoBehaviour
{
    public CameraAnchor camAchor;
    protected GameObject cam;

    public virtual void Possess()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main.gameObject;
        PlayerManager.Instance.cameraBehaviour.SetTarget(camAchor);
    }

    public abstract void CustomUpdate();

    public virtual void UnPossess()
    {
    }
}

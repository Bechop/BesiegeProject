using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public CameraAnchor target = null;

    public bool isSnapped = false;

    public AnimationCurve transitionLook;
    public Utility.Timer transitionTimer;

    // Update is called once per frame
    void LateUpdate()
    {
        if(transitionTimer.IsStarted && !transitionTimer.Timeout())
        {
            float coef = transitionLook.Evaluate(transitionTimer.Progress);
            transform.position = Vector3.Lerp(transform.position, target.GetPosition(), coef);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.GetRotation(), coef);
        }
        else if(!isSnapped && !target.isSnapped)
        {
            transform.position = Vector3.Lerp(transform.position, target.GetPosition(), target.positionSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.GetRotation(), target.rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = target.GetPosition();
            transform.rotation = target.GetRotation();
        }
    }

    public void SetTarget(CameraAnchor _cameraAnchor)
    {
        if (_cameraAnchor && target != _cameraAnchor)
        {
            target = _cameraAnchor;
            transitionTimer.Start();
        }
    }
}

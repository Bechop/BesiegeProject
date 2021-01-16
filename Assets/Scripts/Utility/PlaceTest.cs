using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTest : MonoBehaviour
{
    public Anchor blockAttach;
    public Anchor ownAttach;


    // Start is called before the first frame update
    void Start()
    {
        ReplaceMonObjet();
    }

    // Update is called once per frame
    void Update()
    {
        //ReplaceMonObjet();
    }

    public void ReplaceMonObjet()
    {
        Matrix4x4 TRS = blockAttach.transform.parent.localToWorldMatrix * Matrix4x4.TRS(blockAttach.transform.localPosition, blockAttach.transform.localRotation, Vector3.one);

        Quaternion blockRot = Quaternion.Euler(0f, 180f, 0f) * Quaternion.Inverse(ownAttach.transform.localRotation);

        transform.position = TRS.MultiplyPoint(blockRot * -ownAttach.transform.localPosition);
        transform.rotation = blockAttach.transform.parent.rotation * blockAttach.transform.localRotation * blockRot;
    }
}

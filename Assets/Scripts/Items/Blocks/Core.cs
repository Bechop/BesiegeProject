using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : Block
{
    public override void Place(Vector3 _pos, Quaternion _rot)
    {
        // Create vehicle
        GameObject parent = new GameObject("Vehicle");
        Vehicle vehicle = parent.AddComponent<Vehicle>();
        parent.transform.position = _pos;

        transform.SetParent(vehicle.transform);
        vehicle.rbs.Add(m_rb);

        transform.localRotation = _rot;
        transform.localPosition = Vector3.up * 1.5f;
    }

    public override void Remove()
    {
        Destroy(transform.parent.gameObject);
    }
}

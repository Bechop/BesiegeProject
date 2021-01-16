using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class Tool : Handleable
{
    public float actionRange = 5f;
    public LayerMask layer;

    public int damage = 20;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out hit, actionRange, layer))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Block"))
                {
                    IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                    damageable?.DoDamage(damage);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out hit, actionRange))
            {
                Vehicle vehicle = hit.transform.root.GetComponent<Vehicle>();

                if (vehicle)
                {
                    MenuManager.Instance.save.OpendDialog(vehicle);
                }
            }
        }
    }
}

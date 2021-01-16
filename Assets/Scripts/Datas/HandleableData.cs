using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Handleable ItemData", menuName = "ScriptableObjects/Handleable ItemData")]
public class HandleableData : ItemData
{
    //public virtual void Place()
    //{
    //    RaycastHit hit;
    //    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    //    int layer = 1 << LayerMask.NameToLayer("Block");

    //    if (Physics.Raycast(ray, out hit, actionRange, layer))
    //    {
    //        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Block"))
    //        {
    //            Destroy(hit.transform.gameObject);
    //        }
    //    }
    //}

    public override void OnSelected()
    {
        GameObject go = Instantiate(model);
        PlayerManager.Instance.SetObjectInHand(go);
    }
}

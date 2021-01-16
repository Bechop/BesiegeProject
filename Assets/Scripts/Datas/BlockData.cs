using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block ItemData", menuName = "ScriptableObjects/Block ItemData")]
public class BlockData : ItemData
{
    [Header("Extend")]
    public LayerMask layerMask;
    public float actionRange = 5f;

    public bool isFreePlacing = false;
    [Utility.VisibleIf("isFreePlacing", true)]
    public float spaceFromGround = 1f;

    public override void Place(int _anchorIndex = 0)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out hit, actionRange, layerMask))
        {
            if(isFreePlacing)
            {
                GameObject go = Instantiate(model);
                go.name = title;
                Block newBlock = go.GetComponent<Block>();
                newBlock.Place(hit.point, Quaternion.identity);
            }
            else
            {
                Transform transformHit = hit.collider.transform;
                Anchor anchor = transformHit.gameObject.GetComponent<Anchor>();

                if (anchor && !anchor.isUse)
                {
                    GameObject go = Instantiate(model);
                    go.name = title;
                    Block newBlock = go.GetComponent<Block>();
                    newBlock.Place(anchor.blockRef, anchor, _anchorIndex);
                }
            }
        }
    }
}

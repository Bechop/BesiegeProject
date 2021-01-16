using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class Canon : Block, IInteractable
{
    public GameObject muzzle;
    public GameObject bulletPrefab;
    public float force = 2000.0f;

    public GameObject model;

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (!vehicle.isUse) return;

        if(Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate<GameObject>(bulletPrefab);
            Destroy(bullet, 3.0f);
            bullet.transform.position = muzzle.transform.position;
            bullet.GetComponent<Rigidbody>().AddForce(muzzle.transform.forward * force);
        }
    }

    public void Interact()
    {
        model.transform.rotation = Quaternion.LookRotation(model.transform.right, model.transform.up);
    }

    public void Focus()
    {
        MenuManager.Instance.SetCrossair(MenuManager.CrossairStyle.INTERACTABLE);
    }

    public void Unfocus()
    {
        MenuManager.Instance.SetCrossair(MenuManager.CrossairStyle.DEFAULT);
    }

    public override void Read(BinaryReader binRead)
    {
        base.Read(binRead);

        Vector3 rot = new Vector3(binRead.ReadSingle(), binRead.ReadSingle(), binRead.ReadSingle());
        model.transform.rotation = Quaternion.Euler(rot);
    }

    public override void Write(BinaryWriter binWrite)
    {
        base.Write(binWrite);

        binWrite.Write(model.transform.rotation.eulerAngles.x);
        binWrite.Write(model.transform.rotation.eulerAngles.y);
        binWrite.Write(model.transform.rotation.eulerAngles.z);
    }
}

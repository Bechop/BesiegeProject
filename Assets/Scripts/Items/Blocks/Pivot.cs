using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Pivot : Block, IInteractable, IKeyBindable
{
    public GameObject arm;
    [Space]
    public float speed = 20f;
    public float maxAngle = 40f;

    float yRotation = 0f;
    Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    public HingeJoint rotateJoint = null;

    public override void Awake()
    {
        base.Awake();

        keys["Right"] = KeyCode.D;
        keys["Left"] = KeyCode.Q;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (!vehicle.isUse) return;

        int sens = 0;
        if (Input.GetKey(keys["Right"]))
            sens += 1;
        if (Input.GetKey(keys["Left"]))
            sens -= 1;


        yRotation += speed * sens * Time.deltaTime;
        yRotation = Mathf.Clamp(yRotation, -maxAngle, maxAngle);

        rotateJoint.spring = new JointSpring
        {
            spring = 300f,
            damper = 10f,
            targetPosition = yRotation,
        };

    }


    public void Focus()
    {
        MenuManager.Instance.SetCrossair(MenuManager.CrossairStyle.INTERACTABLE);
    }

    public void Interact()
    {
        UISettingInteractable UISettingInt = MenuManager.Instance.settingInteractable;

        UISettingInt.OpenDialog();

        foreach (var item in keys)
        {
            UISettingInt.AddKeyField(item.Key, item.Value, this);
        }
    }

    public void Unfocus()
    {
        MenuManager.Instance.SetCrossair(MenuManager.CrossairStyle.DEFAULT);
    }

    public void SetKey(string name, KeyCode keyCode)
    {
        keys[name] = keyCode;
    }

    public override void Read(BinaryReader binRead)
    {
        base.Read(binRead);

        int count = binRead.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            SetKey(binRead.ReadString(), (KeyCode)binRead.ReadInt32());
        }
    }

    public override void Write(BinaryWriter binWrite)
    {
        base.Write(binWrite);

        binWrite.Write(keys.Count);

        foreach (var item in keys)
        {
            binWrite.Write(item.Key);
            binWrite.Write((int)item.Value);
        }
    }
}

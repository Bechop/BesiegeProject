using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Wheel : Block, IInteractable, IKeyBindable
{
    [Header("Extend")]
    public float speed = 200.0f;
    public GameObject arrowDisplayed;

    Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    ConfigurableJoint joint;

    public override void Awake()
    {
        base.Awake();
        keys.Add("Forward", KeyCode.Z);
        keys.Add("Backward", KeyCode.S);
    }

    // Start is called before the first frame update
    void Start()
    {
        vehicle = transform.root.GetComponent<Vehicle>();

        arrowDisplayed = Instantiate<GameObject>(arrowDisplayed, transform);
        arrowDisplayed.transform.position += -transform.right * -0.25f;
        arrowDisplayed.transform.rotation = Quaternion.LookRotation(-transform.right, Vector3.up);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (!vehicle.isUse) return;

        int sens = 0;
        if (Input.GetKey(keys["Forward"]))
            sens += 1;
        if (Input.GetKey(keys["Backward"]))
            sens -= 1; 

        joint.targetAngularVelocity = new Vector3(sens * speed * Time.deltaTime, 0f, 0f);
    }

    public override void AttachBlock(Anchor _targetAnchor, Anchor _ownAnchor)
    {
        // Add Joint
        Rigidbody rb = _targetAnchor.targetRB;

        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        joint.connectedBody = rb;

        joint.angularXDrive = new JointDrive
        {
            positionSpring = 0f,
            positionDamper = 10f,
            maximumForce = float.MaxValue,
        };

        joints.Add(joint);

        // Setup Anchors
        _ownAnchor.isUse = true;
        _ownAnchor.gameObject.SetActive(false);

        _targetAnchor.isUse = true;
        _targetAnchor.gameObject.SetActive(false);

        _ownAnchor.linkedAnchor = _targetAnchor;
        _targetAnchor.linkedAnchor = _ownAnchor;

        // Delete Joint
        _targetAnchor.blockRef.onRemove += () => ReleaseBlock(joint);

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

    public void Focus()
    {
        MenuManager.Instance.SetCrossair(MenuManager.CrossairStyle.INTERACTABLE);
        arrowDisplayed?.SetActive(true);
    }

    public void Unfocus()
    {
        MenuManager.Instance.SetCrossair(MenuManager.CrossairStyle.DEFAULT);
        arrowDisplayed?.SetActive(false);
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

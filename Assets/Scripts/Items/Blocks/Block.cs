using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IDamageable
{
    void DoDamage(int damage);
}

public interface IKeyBindable
{
    void SetKey(string name, KeyCode keyCode);
}

public interface IInteractable
{
    void Interact();
    void Focus();
    void Unfocus();
}

public class Block : MonoBehaviour, IDamageable
{
    [Header("Behaviour")]
    public bool isRoot = false;
    [Utility.Locked]public List<Anchor> anchors = new List<Anchor>();

    [Space]
    bool isSelected = false;
    public bool IsSelected { get => isSelected; }

    protected List<Joint> joints = new List<Joint>();
    protected Rigidbody m_rb;

    public delegate void OnRemove();
    public OnRemove onRemove;

    public int life = 100;
    public Utility.Timer damageDelay = new Utility.Timer(0.5f);

    protected Vehicle vehicle = null;

    public virtual void Awake()
    {
        m_rb = gameObject.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        damageDelay.Timeout();
    }

    public virtual void Select()
    {
        isSelected = true;
    }

    public void Unselect()
    {
        isSelected = false;
    }

    public virtual void Remove()
    {
        onRemove?.Invoke();

        vehicle.rbs.Remove(m_rb);

        foreach (Anchor anchor in anchors)
        {
            if (anchor.isUse)
            {
                anchor.linkedAnchor.isUse = false;
                anchor.linkedAnchor.gameObject.SetActive(true);
                anchor.linkedAnchor = null;
            }
        }

        ((PlayerPawn)PlayerManager.Instance.controller.currentPawn).targetInteract = null;
        MenuManager.Instance.SetCrossair(MenuManager.CrossairStyle.DEFAULT);

        Destroy(gameObject);
    }

    public virtual void Place(Block _targetBlock, Anchor _targetAnchor, int _index)
    {
        vehicle = _targetAnchor.transform.root.GetComponent<Vehicle>();
        transform.SetParent(vehicle.transform);

        vehicle.rbs.Add(m_rb);

        Anchor ownAnchor = anchors[_index];
        Matrix4x4 TRS = _targetAnchor.transform.parent.localToWorldMatrix * Matrix4x4.TRS(_targetAnchor.transform.localPosition, _targetAnchor.transform.localRotation, Vector3.one);
        Quaternion blockRot = Quaternion.Euler(0f, 180f, 0f) * Quaternion.Inverse(ownAnchor.transform.localRotation);

        transform.position = TRS.MultiplyPoint(blockRot * -ownAnchor.transform.localPosition);
        transform.rotation = _targetAnchor.transform.parent.rotation * _targetAnchor.transform.localRotation * blockRot;

        Attach();
    }

    public virtual void Place(Vector3 _pos, Quaternion _rot)
    {
        transform.position = _pos;
        transform.rotation = _rot;
    }

    public virtual void AttachBlock(Anchor _targetAnchor, Anchor _ownAnchor)
    {
        // Add Joint
        Rigidbody rb = _targetAnchor.targetRB;

        Joint temp = gameObject.AddComponent<FixedJoint>();
        temp.connectedBody = rb;

        joints.Add(temp);

        // Setup Anchors
        _ownAnchor.isUse = true;
        _ownAnchor.gameObject.SetActive(false);

        _targetAnchor.isUse = true;
        _targetAnchor.gameObject.SetActive(false);

        _ownAnchor.linkedAnchor = _targetAnchor;
        _targetAnchor.linkedAnchor = _ownAnchor;

        // Delete Joint
        _targetAnchor.blockRef.onRemove += () => ReleaseBlock(temp);

    }

    public virtual void ReleaseBlock(Joint joint)
    {
        joints.Remove(joint);
        Destroy(joint);
    }

    public void Attach()
    {
        foreach (Anchor anchor in anchors)
        {
            if(!anchor.isUse)
            {
                Collider[] colliders = Physics.OverlapBox(anchor.transform.position, anchor.transform.localScale / 2, anchor.transform.rotation);
                foreach (Collider collider in colliders)
                {
                    Anchor targetAnchor = collider.gameObject.GetComponent<Anchor>();
                    if (targetAnchor && targetAnchor.blockRef != this)
                    {
                        AttachBlock(targetAnchor, anchor);
                    }
                }
            }
        }
    }

    public void DoDamage(int damage)
    { 
        if(!damageDelay.IsStarted)
        {
            damageDelay.Start();
            life -= damage;

            if (life <= 0)
                Remove();
        }
    }

    public virtual void Read(BinaryReader binRead)
    {
        vehicle = transform.root.GetComponent<Vehicle>();

        Vector3 pos = new Vector3(binRead.ReadSingle(), binRead.ReadSingle(), binRead.ReadSingle());
        transform.localPosition = pos;
        Vector3 rot = new Vector3(binRead.ReadSingle(), binRead.ReadSingle(), binRead.ReadSingle());
        transform.localRotation = Quaternion.Euler(rot);
    }

    public virtual void Write(BinaryWriter binWrite)
    {
        binWrite.Write(name);
        binWrite.Write(transform.localPosition.x);
        binWrite.Write(transform.localPosition.y);
        binWrite.Write(transform.localPosition.z);
        binWrite.Write(transform.localRotation.eulerAngles.x);
        binWrite.Write(transform.localRotation.eulerAngles.y);
        binWrite.Write(transform.localRotation.eulerAngles.z);
    }

#if UNITY_EDITOR
    [ContextMenu("Setup Anchor")]
    public void SetAnchors(MenuCommand menuCommand)
    {
        Block block = menuCommand.context as Block;
        block.anchors = new List<Anchor>( block.gameObject.GetComponentsInChildren<Anchor>());

        foreach (Anchor anchor in block.anchors)
        {
            anchor.blockRef = this;
        }
    }
#endif
}

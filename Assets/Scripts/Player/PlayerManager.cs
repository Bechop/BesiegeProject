using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : Utility.UniqueInstance<PlayerManager>
{

    // -- Global Data -- //
    public PlayerController controller;
    public CameraBehaviour cameraBehaviour;

    int indexMove = 0; 
    int indexRotate = 0; 
    int indexAction = 0; 

    [Space]
    [SerializeField] Transform handPos = null;

    public void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
       
    }

    public void SetObjectInHand(GameObject obj)
    {
        for (int i = 0; i < handPos.childCount; i++)
        {
            Destroy(handPos.GetChild(i).gameObject);
        }

        if(obj)
        {
            obj.transform.SetParent(handPos);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void SetFlag(PlayerController.Option option, bool value)
    {
        switch (option)
        {
            case PlayerController.Option.MOVE:
                indexMove += value ? 1 : -1;
                break;
            case PlayerController.Option.ROTATION:
                indexRotate += value ? 1 : -1;
                break;
            case PlayerController.Option.ACTION:
                indexAction += value ? 1 : -1;
                break;
            case PlayerController.Option.INTERACTION:
                break;
            case PlayerController.Option.MODIFICATION:
                break;
            default:
                break;
        }

        Utility.SetFlag( ref controller.option, (int)PlayerController.Option.MOVE, indexAction >= 0);
        Utility.SetFlag( ref controller.option, (int)PlayerController.Option.ROTATION, indexRotate >= 0);
        Utility.SetFlag( ref controller.option, (int)PlayerController.Option.ACTION, indexAction >= 0);
    }
}

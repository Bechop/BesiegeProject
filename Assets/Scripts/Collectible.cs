using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public LayerMask layerMask;
    bool isUse = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Block") && !isUse)
        {
            isUse = true;
            GameManager.Instance.Achieve();
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public bool isUse = false;

    public List<Rigidbody> rbs = new List<Rigidbody>();

    public void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    

    // Update is called once per frame
    void Update()
    {

    }

    public void SetReady()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
        }
    }
}

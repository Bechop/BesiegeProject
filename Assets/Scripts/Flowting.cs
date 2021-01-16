using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowting : MonoBehaviour
{
    public Vector3 direction;
    public float frequency = 1;

    Vector3 origin;

    public void Awake()
    {
        origin = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = origin + direction * Mathf.Sin(Time.time * frequency);
    }
}

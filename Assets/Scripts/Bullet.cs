using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1000;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Desctrucible dam = collision.gameObject.GetComponent<Desctrucible>();
        if(dam != null)
        {
            dam.DoDamage(damage);
            Destroy(gameObject);
        }
        
    }
}

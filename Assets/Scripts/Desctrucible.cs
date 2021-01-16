using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desctrucible : MonoBehaviour, IDamageable
{
    public int life = 100;
    bool isUse = false;

    public void DoDamage(int damage)
    {
        if(!isUse)
        {
            life -= damage;
            if (life <= 0)
            {
                GameManager.Instance.Achieve();
                Destroy(gameObject);
                isUse = true;
            }
        }
    }
}

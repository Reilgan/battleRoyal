using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapons : MonoBehaviour
{
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "player")
            {
                //тут наносим урон
            }
        }
    }
}

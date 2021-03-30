using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRangedWeapons : MonoBehaviour
{
    [SerializeField] private GameObject bullet;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    private void Fire()
    {
        //Instantiate(bullet, this.transform.position, Quaternion.identity);
        PhotonClient.PhotonInstantiate(bullet, this.transform);//типа сетевая функция
    }
}

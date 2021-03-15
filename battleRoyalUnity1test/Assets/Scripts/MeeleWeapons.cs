using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleWeapons : MonoBehaviour
{
    [SerializeField] private float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //ну тут надо взять и сообщить серверу кому мы там чо какой урон нанесли и чтобы он там отнимался принимался итп
        }
    }
}

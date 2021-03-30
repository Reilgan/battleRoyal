using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    void Start()
    {
        this.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * speed);
        StartCoroutine("DoDestroyBullet");
    }

    IEnumerator DoDestroyBullet()
    {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //ну тут надо взять и сообщить серверу кому мы там чо какой урон нанесли и чтобы он там отнимался принимался итп
        }
    }
}

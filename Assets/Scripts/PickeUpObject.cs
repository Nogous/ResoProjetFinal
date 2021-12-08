using Unity.Netcode;
using UnityEngine;

public class PickeUpObject : NetworkBehaviour
{
    public Vector3 localPos;
    public bool isPickedUp = false;

    public int idForme;

    private void Update()
    {
        if (isPickedUp)
        {
            transform.localPosition = localPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Tangrame tang = other.transform.gameObject.GetComponent<Tangrame>();

        if (tang.IntegratePice(idForme))
        {
            print("hit");
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}

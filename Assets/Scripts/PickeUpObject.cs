using Unity.Netcode;
using UnityEngine;

public class PickeUpObject : NetworkBehaviour
{
    public Vector3 localPos;
    public bool isPickedUp = false;

    private void Update()
    {
        if (isPickedUp)
        {
            transform.localPosition = localPos;
        }
    }
}

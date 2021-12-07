using Unity.Netcode;
using UnityEngine;

public class PickeUpObject : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        //transform.position += Vector3.up * Time.deltaTime;
    }
}

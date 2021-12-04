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
}

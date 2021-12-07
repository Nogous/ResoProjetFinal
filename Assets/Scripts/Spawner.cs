using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private GameObject[] picePrefabs;
    private bool asSpawn = false;

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
        if (!IsServer) return;
        if (!asSpawn)
        {
            SpawnPices();
            asSpawn = true;
        }
    }

    void SpawnPices()
    {
        if (picePrefabs.Length == 0) return;

        var newPice = Instantiate(picePrefabs[Random.Range(0, picePrefabs.Length)]);
        newPice.GetComponent<NetworkObject>().Spawn();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

public class ConnectionManager : MonoBehaviour
{
    public string _IPAddress = "127.0.0.1";

    private UNetTransport m_transport;
    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    public void Join()
    {
        m_transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        m_transport.ConnectAddress = _IPAddress;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("Password1234");
        NetworkManager.Singleton.StartClient();
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientID,
        NetworkManager.ConnectionApprovedDelegate callback)
    {
        //Check Password
        bool approve = System.Text.Encoding.ASCII.GetString(connectionData) == "Password1234";
        callback(true, null, approve, GetRandomBoardPosition(), Quaternion.identity);
    }

    private Vector3 GetRandomBoardPosition()
    {
        return new Vector3(Random.Range(0f, 10f), 2, Random.Range(0f, 10f));
    }

    public void OnIPAddressChanged(string newAddress)
    {
        _IPAddress = newAddress;
    }
}

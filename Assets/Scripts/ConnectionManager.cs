using System;
using System.Collections;
using System.Collections.Generic;
using ShapeFight;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class ConnectionManager : NetworkBehaviour
{
    

    public string _IPAddress = "127.0.0.1";
    public string _password = "";
    public string _playerName = "Player";
    public NetworkList<FixedString32Bytes> _playerNames = new NetworkList<FixedString32Bytes>();
    public NetworkVariable<int> _playerCount = new NetworkVariable<int>();
    public NetworkVariable<int> _readyPlayerCount = new NetworkVariable<int>();

    public void SetPassword(string password)
    {
        _password = password;
    }
    public void SetName(string name)
    {
        _playerName = name;
    }

    private UNetTransport m_transport;
    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        _playerNames.Add(_playerName);
        _playerCount.Value += 1;
        NetworkManager.Singleton.StartHost();
    }

    public void Join()
    {
        m_transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        m_transport.ConnectAddress = _IPAddress;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(_password + ";" + _playerName);
        NetworkManager.Singleton.StartClient();
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientID,
        NetworkManager.ConnectionApprovedDelegate callback)
    {
        //Check Password
        string[] datas = System.Text.Encoding.ASCII.GetString(connectionData).Split(';');
        bool approve = datas[0] == _password;
        if (approve)
        {
            _playerCount.Value += 1;
            _playerNames.Add(datas[1]);
        }
        callback(true, null, approve, GetPlayerPosition(), Quaternion.identity);
    }

    private Vector3 GetPlayerPosition()
    {
        return new Vector3(Random.Range(0f, 10f), 2, Random.Range(0f, 10f));
    }

    public void OnIPAddressChanged(string newAddress)
    {
        _IPAddress = newAddress;
    }

    public void PlayerReady(bool isReady)
    {
        if (isReady)
            _readyPlayerCount.Value += 1;
        else
            _readyPlayerCount.Value -= 1;

        CheckAllPlayerReady();
    }

    public void CheckAllPlayerReady()
    {
        if (_readyPlayerCount.Value == 2)
        {
            GameManager.instance.gameLaunched.Value = true;
        }
    }
}

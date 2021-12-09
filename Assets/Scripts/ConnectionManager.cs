using ShapeFight;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Random = UnityEngine.Random;

public class ConnectionManager : NetworkBehaviour
{
    public string _IPAddress = "127.0.0.1";
    public string _password = "";
    public string _playerName = "";
    NetworkList<FixedString32Bytes> _playerNames;
    public NetworkVariable<int> _playerCount = new NetworkVariable<int>();

    void Awake()
    {
        _playerNames = new NetworkList<FixedString32Bytes>();
    }
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

        if (_playerName == "")
            _playerName = "Player " + NetworkManager.LocalClientId;

        if (_password == "")
            _password = "default";

        _playerNames.Add(_playerName);
        _playerCount.Value = 1;

        NetworkManager.Singleton.StartHost();
    }

    public void Join()
    {
        m_transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        m_transport.ConnectAddress = _IPAddress;

        if (_playerName == "")
            _playerName = "Player " + NetworkManager.LocalClientId;

        if (_password == "")
            _password = "default";

        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(_password + ";" + _playerName);
        NetworkManager.Singleton.StartClient();
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientID,
        NetworkManager.ConnectionApprovedDelegate callback)
    {
        if(_playerCount.Value > 2)
            callback(false, null, false, Vector3.zero, Quaternion.identity);

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
}

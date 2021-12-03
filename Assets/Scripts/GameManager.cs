using Unity.Netcode;
using UnityEngine;

namespace ShapeFight
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public Transform camera;

        private void Awake()
        {
            if (!instance)
                instance = this;
            else
                Destroy(this);
        }

        //private void OnGUI()
        //{
        //    GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        //    if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        //    {
        //        StartButtons();
        //    }
        //    else
        //    {
        //        StatusLabels();

        //        SubmitNewPosition();
        //    }

        //    GUILayout.EndArea();
        //}

        //void StartButtons()
        //{
        //    if (GUILayout.Button("Host")) Host();
        //    if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        //    if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        //}
        //static void StatusLabels()
        //{
        //    var mode = NetworkManager.Singleton.IsHost ?
        //        "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        //    GUILayout.Label("Transport: " +
        //                    NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        //    GUILayout.Label("Mode: " + mode);
        //}

        //static void SubmitNewPosition()
        //{
        //    if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
        //    {
        //        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        //        var player = playerObject.GetComponent<Player>();
        //        player.Move();
        //    }
        //}

        //void Host()
        //{
        //    NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        //    NetworkManager.Singleton.StartHost();
        //}

        //void Join()
        //{

        //}
        //private void ApprovalCheck(byte[] connectionData, ulong clientID,
        //    NetworkManager.ConnectionApprovedDelegate callback)
        //{
        //    Debug.Log("Approving a connection");
        //    callback(true, null, true, Vector3.zero, Quaternion.identity);
        //}
    }
}
using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

namespace ShapeFight
{
    public class Player : NetworkBehaviour
    {
        public static int id = 0;
        int idcolor = 0;

        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private float playerSpeed = 2.0f;
        private float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;

        [SerializeField] private Transform lookTargetRay;

        //pickeup
        public NetworkVariable<bool> asObjectPickedUp = new NetworkVariable<bool>();

        private NetworkObject pickedUpObject;

        public NetworkVariable<bool> isReady = new NetworkVariable<bool>();

        private void Start()
        {
            if (IsOwner) GameManager.instance.player = this;


            idcolor = id;
            id++;

            if (IsLocalPlayer)
            {
                controller = gameObject.AddComponent<CharacterController>();
            }

            switch (idcolor)
            {
                case 0:
                    gameObject.GetComponent<Renderer>().material.color = Color.red;
                    break;
                case 1:
                    gameObject.GetComponent<Renderer>().materials[0].color = Color.blue;
                    break;
                default:
                    break;
            }
        }

        void Move()
        {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            controller.Move(move * Time.deltaTime * playerSpeed);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }

            // Changes the height position of the player..
            if (Input.GetButtonDown("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        void Update()
        {
            if (!GameManager.instance.gameLaunched.Value) return;
            if (IsLocalPlayer)
            {
                Move();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (asObjectPickedUp.Value)
                    {
                        // poser l'objet
                        DropObject();
                    }
                    else
                    {
                        // test de prise d'objet
                        PickupObject();
                    }
                }
            }
        }


        void PickupObject()
        {
            Debug.DrawLine(transform.position, transform.position + lookTargetRay.position - transform.position * 1f, Color.blue, 1f);
            RaycastHit[] hit = Physics.SphereCastAll(transform.position, .1f, lookTargetRay.position - transform.position);
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform)
                {
                    //Debug.Log(hit.transform.gameObject.name);
                    PickeUpObject obj = hit[i].transform.gameObject.GetComponent<PickeUpObject>();
                    if (obj != null)
                    {
                        //print(obj.gameObject.name);

                        if (IsServer)
                            PickupObjectClientRpc(obj.NetworkObjectId);
                        else
                            PickupObjectServerRpc(obj.NetworkObjectId);

                        return;
                    }
                }
            }
        }
        void DropObject()
        {
            if (IsServer)
                DropObjClientRpc();
            else
                DropObjServerRpc();
        }

        [ServerRpc]
        void PickupObjectServerRpc(ulong objectToPickupID)
        {
            PickupObjectClientRpc(objectToPickupID);
        }

        [ClientRpc]
        void PickupObjectClientRpc(ulong objectToPickupID)
        {
            //print("try to pickup");

            NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectToPickupID, out var objectToPickup);
            // l'objet et deja pris donc peut pas etre pris
            if (objectToPickup == null || objectToPickup.transform.parent != null) return;

            //print("object pickup");

            objectToPickup.GetComponent<Rigidbody>().isKinematic = true;
            if (IsServer)
                objectToPickup.transform.parent = transform;
            objectToPickup.transform.localPosition = Vector3.up;
            asObjectPickedUp.Value = true;
            pickedUpObject = objectToPickup;
            PickeUpObject pick = pickedUpObject.gameObject.GetComponent<PickeUpObject>();
            pick.isPickedUp = true;
            pick.localPos = pick.transform.localPosition;
            pickedUpObject.gameObject.GetComponent<ClientNetworkTransform>().InLocalSpace = true;
            pickedUpObject.gameObject.GetComponent<Collider>().enabled = false;
        }

        [ServerRpc]
        public void DropObjServerRpc()
        {
            DropObjClientRpc();
        }
        [ClientRpc]
        public void DropObjClientRpc()
        {
            if (pickedUpObject != null)
            {
                // can be null if enter drop zone while carying
                pickedUpObject.transform.localPosition = new Vector3(0, 0, 2);
                if (IsServer)
                    pickedUpObject.transform.parent = null;
                pickedUpObject.GetComponent<Rigidbody>().isKinematic = false;
                
                pickedUpObject.gameObject.GetComponent<ClientNetworkTransform>().InLocalSpace = false;
                PickeUpObject pick = pickedUpObject.gameObject.GetComponent<PickeUpObject>();
                pick.isPickedUp = false;
                pick.gameObject.GetComponent<Collider>().enabled = true;


                pickedUpObject = null;
            }

            asObjectPickedUp.Value = false;
        }



        public void AddPointsToPlayer(int idPlayer, int nbPoints = 1)
        {
            if (!(idPlayer == 0 || idPlayer == 1)) return;

            GameManager.instance.gamePoints[idPlayer] += nbPoints;
            UpdatePointsClientRpc();
        }

        public void PlayerReady()
        {
            PlayerReadyServerRpc();
        }

        [ClientRpc]
        public void UpdatePointsClientRpc()
        {
            GameManager.instance.scoreP1.text = GameManager.instance.gamePoints[0].ToString();
            GameManager.instance.scoreP2.text = GameManager.instance.gamePoints[1].ToString();
        }

        [ServerRpc]
        void PlayerReadyServerRpc()
        {
            PlayerReadyClientRpc();
        }

        [ClientRpc]
        void PlayerReadyClientRpc()
        {
            print(GameManager.instance.nbPlayerReady);

            GameManager.instance.nbPlayerReady++;
            if (GameManager.instance.nbPlayerReady == 2)
            {
                GameManager.instance.gameLaunched.Value = true;
            }
        }
    }
}
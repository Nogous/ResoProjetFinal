using Unity.Netcode;
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

        private void Start()
        {
            idcolor = id;
            id++;

            if (IsLocalPlayer)
            {
                controller = gameObject.AddComponent<CharacterController>();
            }
            else
            {

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
            if (IsLocalPlayer)
            {
                Move();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (asObjectPickedUp.Value)
                    {
                        // poser l'objet
                        DropObject();
                        //DropObjServerRpc();
                    }
                    else
                    {
                        // test de prise d'objet
                        PickupObject();
                    }
                }
            }
            else
            {
                
            }
        }


        void PickupObject()
        {

            RaycastHit hit;
            Debug.DrawLine(transform.position, transform.position + lookTargetRay.position - transform.position * 1f, Color.blue, 1f);
            Physics.SphereCast(transform.position, .1f, lookTargetRay.position - transform.position, out hit);
            if (hit.transform)
            {
                //Debug.Log(hit.transform.gameObject.name);
                PickeUpObject obj = hit.transform.gameObject.GetComponent<PickeUpObject>();
                if (obj != null)
                {
                    print(obj.gameObject.name);

                    if (IsServer)
                        PickupObjectClientRpc(obj.NetworkObjectId);
                    else
                        PickupObjectServerRpc(obj.NetworkObjectId);
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
            print("try to pickup");

            NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectToPickupID, out var objectToPickup);
            // l'objet et deja pris donc peut pas etre pris
            if (objectToPickup == null || objectToPickup.transform.parent != null) return;

            print("object pickup");

            objectToPickup.GetComponent<Rigidbody>().isKinematic = true;
            objectToPickup.transform.parent = transform;
            objectToPickup.transform.localPosition = Vector3.up;
            asObjectPickedUp.Value = true;
            pickedUpObject = objectToPickup;
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
                pickedUpObject.transform.localPosition = new Vector3(0, 0, 3);
                pickedUpObject.transform.parent = null;
                pickedUpObject.GetComponent<Rigidbody>().isKinematic = false;
                pickedUpObject = null;
            }

            asObjectPickedUp.Value = false;
        }
    }
}
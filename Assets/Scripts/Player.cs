using Unity.Netcode;
using UnityEngine;

namespace ShapeFight
{
    public class Player : NetworkBehaviour
    {
        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private float playerSpeed = 2.0f;
        private float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;

        //pickeup
        public NetworkVariable<bool> asObjectPickedUp = new NetworkVariable<bool>();

        private NetworkObject pickedUpObject;

        private void Start()
        {
            if (IsLocalPlayer)
            {
                controller = gameObject.AddComponent<CharacterController>();
            }
            else
            {

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
                        DropObjServerRpc();
                    }
                    else
                    {
                        // test de prise d'objet
                        var hit = Physics.OverlapSphere(transform.position, 5);
                        if (hit.Length > 0)
                        {
                            PickeUpObject obj = hit[0].gameObject.GetComponent<PickeUpObject>();
                            if (obj != null)
                            {
                                PickupObjectServerRpc(obj.NetworkObjectId);
                            }
                        }
                    }
                }
            }
            else
            {
                
            }
        }

        [ServerRpc]
        public void PickupObjectServerRpc(ulong objectToPickupID)
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
using System;
using Photon.Pun;
using UnityEngine;

public class FlashLight : MonoBehaviourPunCallbacks
{
    public GameObject closeLight;
    public GameObject closeVision;
    public GameObject flashVision;
    
    public SpriteRenderer spriteRendererBody; // Sprite renderer component of the player body
    public SpriteRenderer spriteRendererPart; // Sprite renderer component of the player part
    public Material notOwnedMaterial; // Material to use when the client is not the owner of the player

    private void Start()
    {
        if (!photonView.IsMine)
        {
            closeLight.SetActive(false);
            closeVision.SetActive(false);
            flashVision.SetActive(false);

            spriteRendererBody.material = notOwnedMaterial;
            spriteRendererPart.material = notOwnedMaterial;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Get the mouse position in world space
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculate the direction from the light to the mouse
            var direction = (mousePosition - transform.position).normalized;

            // Create a rotation that rotates the light to face the mouse cursor
            var rotation = Quaternion.LookRotation(direction, Vector3.forward);

            // Set the light's rotation
            flashVision.transform.rotation = rotation;
        }
    }
}
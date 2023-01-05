using System;
using Photon.Pun;
using UnityEngine;

public class FlashLight : MonoBehaviourPunCallbacks
{
    public GameObject closeLight;
    public GameObject closeVision;
    public GameObject flashVision;
    
    public SpriteRenderer spriteRendererBody; 
    public SpriteRenderer spriteRendererPart; 
    public Material notOwnedMaterial; 

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
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = (mousePosition - transform.position).normalized;
            var rotation = Quaternion.LookRotation(direction, Vector3.forward);
            flashVision.transform.rotation = rotation;
        }
    }
}
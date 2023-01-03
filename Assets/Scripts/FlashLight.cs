using Photon.Pun;
using UnityEngine;

public class FlashLight : MonoBehaviourPunCallbacks
{
    // The flashlight light component
    public GameObject light2D;

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
            light2D.transform.rotation = rotation;
        }
    }
}
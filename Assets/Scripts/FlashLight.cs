using UnityEngine;
using Photon.Pun;
using UnityEngine.Experimental.Rendering.Universal;

public class FlashLight : MonoBehaviourPunCallbacks
{
    // The flashlight range
    public float range = 6.0f;

    // The flashlight angle
    public float angle = 45.0f;

    // The flashlight intensity
    public float intensity = 1.0f;

    // The flashlight layer mask
    public LayerMask mask;

    // The flashlight light component
    public Light2D flashlightLight;

	void Update()
	{
		if (photonView.IsMine)
		{
			// This is the owner's flashlight - update the light based on the mouse position

			// Get the mouse position in screen space
			Vector3 mousePos = Input.mousePosition;

			// Convert the mouse position to world space
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z - Camera.main.transform.position.z));

			// Calculate the direction from the flashlight to the mouse
			Vector3 direction = (mouseWorldPos - transform.position).normalized;

			// Set the flashlight direction and intensity
			flashlightLight.transform.forward = direction;
			flashlightLight.intensity = intensity;

			// Set the flashlight range
			flashlightLight.pointLightOuterRadius = range;

			// Cast a ray from the flashlight in the direction of the mouse
			Ray2D ray = new Ray2D(transform.position, direction);

			// Perform the raycast
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, range, mask);
			if (hit.collider != null)
			{
				// Check if the ray hit something within the flashlight angle
				if (Vector3.Angle(direction, transform.forward) <= angle / 2)
				{
					// The ray hit something within the flashlight angle! Do something here, like illuminating the hit object.
					hit.collider.gameObject.GetComponent<Renderer>().enabled = true;
					GameObject child1 = hit.collider.gameObject.transform.GetChild(0).gameObject;
					child1.GetComponent<SpriteRenderer>().enabled = true;
					
					GameObject grandChild = child1.transform.GetChild(0).gameObject;
					grandChild.GetComponent<SpriteRenderer>().enabled = true;
					
					Debug.Log("Enable et");
					// Make all other players invisible
					GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
					foreach (GameObject player in players)
					{
						if (player != hit.collider.gameObject)
						{
							GameObject playerChild1 = player.transform.GetChild(0).gameObject;
							playerChild1.GetComponent<SpriteRenderer>().enabled = true;
							
							GameObject playerGrandChild = playerChild1.transform.GetChild(0).gameObject;
							playerGrandChild.GetComponent<SpriteRenderer>().enabled = true;
							Debug.Log("Enable et hepsi");
						}
					}
				}
				else
				{
					// The ray hit something outside of the flashlight angle! Do something here, like making all players visible.
					// Make all players visible
					GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
					foreach (GameObject player in players)
					{
						Debug.Log("Disable et hepsi");
						GameObject playerChild1 = player.transform.GetChild(0).gameObject;
						playerChild1.GetComponent<SpriteRenderer>().enabled = false;
						
						GameObject playerGrandChild = playerChild1.transform.GetChild(0).gameObject;
						playerGrandChild.GetComponent<SpriteRenderer>().enabled = false;
					}
				}
			}
		}
		else
		{
			// This is not the owner's flashlight - update the light based on network data
		}
	}
}
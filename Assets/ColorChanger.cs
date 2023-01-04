using Photon.Pun;
using UnityEngine;

public class ColorChanger : MonoBehaviourPunCallbacks
{
    public SpriteRenderer player;
    public Color[] colors;
    public Color selectedColor; // The selected color for the character
    
    

    // Updates the color of the character for all clients
    [PunRPC]
    void UpdateCharacterColor(Color color)
    {
        // Only update the color if the character is owned by the local player
        if (photonView.IsMine)
        {
            // Set the color of the character
            player.material.color = color;
            
            // Save the selected color locally
            PlayerPrefs.SetString("Color", color.ToString());
        }
    }

    public void ChangeColor(int index)
    {
        Color color = colors[index];
        
        // Call the UpdateCharacterColor function on all clients if the selected color has changed
        if (color != selectedColor)
        {
            photonView.RPC("UpdateCharacterColor", RpcTarget.All, color);
            selectedColor = color;
        }
    }
}

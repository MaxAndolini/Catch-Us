using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    public Text playerName;
    public Image playerImage;
    
    public void SetPlayerName(Player player)
    {
        playerName.text = player.NickName;
    }
    
    public void SetPlayerColor(Color color)
    {
        playerImage.color = color;
    }
}
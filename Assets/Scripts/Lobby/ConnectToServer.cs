using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public InputField nameInput;
    public Text buttonText;

    public void OnClickConnect()
    {
        if (nameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = nameInput.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster() //succesfully connected main server
    {
        PlayerPrefs.SetString("Username", nameInput.text);
        SceneManager.LoadScene("LobbyScene");
    }
}

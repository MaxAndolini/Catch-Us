using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomInput;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;
    
    public RoomItem roomItem;
    private List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObj;

    public float timeBetweenUpdates = 1.5f; //????????????
    private float nextUpdateTime;

    private List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItem;
    public Transform playerItemParent;

    public GameObject playButton;
    private void Start()
    {
        PhotonNetwork.JoinLobby(); //in order to create a room, join photon lobby
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }

    public void OnClickCreate() //create room
    {
        if (roomInput.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions(){MaxPlayers = 2});
        }
    }

    public override void OnJoinedRoom() //join to the room
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        
        UpdatePlayerlist();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime) //we need them because, sometimes OnRoomListUpdate update the list twice or more.
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    private void UpdateRoomList(List<RoomInfo> list) //when we add or delete the room from the list, it directly update
    {
        foreach (RoomItem item in roomItemsList) //destroy all the current list
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list) //add new room and give name
        {
            if (room.RemovedFromList)
            {
                return;
            }
            RoomItem newRoom = Instantiate(roomItem, contentObj);
            newRoom.SetRoomName(room.Name); //it comes from the roomItem script
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName) //enter the room when we click the roomName from the lobby list
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom() //but we dont turn the lobby, we turn the photon lobby. So wee need OnConnectedToMaster
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster() //because if we left the room, we cannot see the another created rooms before.
    {
        PhotonNetwork.JoinLobby();
    }

    public void UpdatePlayerlist()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();
        
        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            //keyvalue represents 2 list. int is a ID and Player is a photon player component
            PlayerItem newPlayerItem = Instantiate(playerItem, playerItemParent);
            newPlayerItem.SetPlayerName(player.Value); //it comes from PlayerItem class
            playerItemsList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerlist();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerlist();
    }

    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
    
}

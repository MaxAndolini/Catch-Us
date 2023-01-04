using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    public Transform[] playerSpawnPoints;
    private readonly List<int> takenSpawnPoints = new List<int>();
    private int whichPlayerIsImposter;

    private void Start()
    {
        var spawnPointIndex = ChooseRandomSpawnPoint();
        var spawnPointPlayer = playerSpawnPoints[spawnPointIndex];
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPointPlayer.position, Quaternion.identity);
        AssignImposters();
    }

    private int ChooseRandomSpawnPoint()
    {
        var randomPoint = Random.Range(0, playerSpawnPoints.Length);
        // If the chosen spawn point has already been taken, try again
        if (takenSpawnPoints.Contains(randomPoint)) return ChooseRandomSpawnPoint();
        // Mark the chosen spawn point as taken
        takenSpawnPoints.Add(randomPoint);
        return randomPoint;
    }

    public void AssignImposters()
    {
        var threshold = 1;

        if (PhotonNetwork.CurrentRoom.PlayerCount < threshold)
            return;

        var pvs = FindObjectsOfType<PhotonView>();
        var plyPV = new List<PhotonView>();

        foreach (var p in pvs)
        {
            var pc = p.GetComponent<PlayerMovement>();
            if (pc)
            {
                p.RPC("RPC_NeutralizeImposter", RpcTarget.All, p.ViewID);
                plyPV.Add(p);
            }
        }

        var rand = Random.Range(0, plyPV.Count);
        var pv = plyPV[rand];
        pv.RPC("RPC_MakeImposter", RpcTarget.All, pv.ViewID);
        plyPV.Remove(plyPV[rand]);
    }
}
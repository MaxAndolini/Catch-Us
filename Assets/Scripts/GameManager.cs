using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    public Transform[] playerSpawnPoints;
    private readonly List<int> takenSpawnPoints = new List<int>();

    private void Start()
    {
        var spawnPointIndex = ChooseRandomSpawnPoint();
        var spawnPointPlayer = playerSpawnPoints[spawnPointIndex];
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPointPlayer.position, Quaternion.identity);
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
}
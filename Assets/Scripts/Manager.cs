using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Manager : MonoBehaviour
{
    public string player_prefab_string;
    public GameObject player_prefab;
    public Transform[] spawn_points;

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {
        Transform t_spawn = spawn_points[Random.Range(0, spawn_points.Length)];

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(player_prefab_string, t_spawn.position, t_spawn.rotation);
        }

        else
        {
            GameObject newPlayer = Instantiate(player_prefab, t_spawn.position, t_spawn.rotation) as GameObject;
        }
    }
}

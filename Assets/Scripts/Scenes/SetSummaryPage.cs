using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Solu.Model;
using System.Linq;
using System;
using SoluDelegate;
using Constants;
using UnityEngine.SceneManagement;
using UnitySocketIO;
using UnitySocketIO.Events;
using SoluUtilities;

public class SetSummaryPage : MonoBehaviour
{
    public GameObject namePrefabs;
    public GameObject player;
    public GameObject board;
    public GameObject endButton;
    public GameObject cupImage;

    public SocketIOController io;
    public Sprite[] coin;
    private string nameJsonString;
    private int arrayIdx;
    private int index = 0;

    void Start()
    {
        InitiateListener();
        InitSocket();
    }

    void InitSocket()
    {
        io.On(SocketEvent.Connect, (SocketIOEvent e) =>
        {
            Debug.Log("-------");
            Debug.Log(UserDelegate.status);
            Debug.Log(UserDelegate.username);
            Debug.Log(RoomDelegate.password.ToString());
            io.Emit("joinSocket", RoomDelegate.password.ToString());
            if (UserDelegate.status == "banker")
            {
                Debug.Log(true);
                io.Emit("testSortScore", RoomDelegate.password.ToString(), (string data) =>
                {
                    Debug.Log("tetSortScore");
                    Debug.Log(data);

                });
            }
            io.On("checkSort", (SocketIOEvent playerUpdate) =>
            {
                Debug.Log("checkSort");
                Debug.Log(playerUpdate);
            });

            io.Emit(SocketEvent.SortScore, RoomDelegate.password.ToString(), (string data) =>
            {
                PlayerSortScore players = JsonUtility.FromJson<PlayerSortScore>(data);
                foreach (PlayersModel user in players.data)
                {
                    GameObject setPlayer = Instantiate(namePrefabs) as GameObject;
                    setPlayer.transform.SetParent(board.transform, false);
                    setPlayer.name = user.id;
                    if (user.type == "in")
                    {
                        setPlayer.transform.Find("name").gameObject.GetComponent<Text>().text = user.name;
                    } 
                    else
                    {
                        setPlayer.transform.Find("name").gameObject.GetComponent<Text>().text = user.name + "(out)";
                    }
                    setPlayer.transform.Find("score").gameObject.GetComponent<Text>().text = user.score.ToString();
                    if (index < 3)
                    {
                        setPlayer.transform.Find("image").gameObject.GetComponent<Image>().sprite = coin[index];
                    }
                    else
                    {
                        setPlayer.transform.Find("image").gameObject.GetComponent<Image>().sprite = coin[index];
                    }
                    index = index + 1;
                    SoluUtility.SetActiveDisplay(cupImage, true);

                    if (UserDelegate.status == "banker")
                    {
                        SoluUtility.SetActiveDisplay(endButton, true);
                    }

                }
            });
        });
        io.Connect();

        io.On(SocketEvent.ChangeLobbyScene, (SocketIOEvent changeStartScene) =>
        {
            SceneManager.LoadScene(SceneConstants.LobbyGame); //if (UserDelegate.status == "banker")
        });
    }
    void OnApplicationQuit() //summary
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        RoomDelegate.password = 123;
        io.Emit("aas", "sum", (string data) =>
        {

        });
    }

    void InitiateListener() {
        arrayIdx  = coin.Length;
        Sprite coinSprite = coin[arrayIdx - 1];

        if (UserDelegate.status == "banker")
        {
            //SoluUtility.SetActiveDisplay(endB utton, true);
            endButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneConstants.LobbyGame);

                //io.Emit("a123", RoomDelegate.password.ToString());
                io.Emit("reScore", RoomDelegate.password.ToString());
                //{
                    //Debug.Log(data);
                    //RoomData players = JsonUtility.FromJson<RoomData>(data);


                //});

            });
        }
        else
        {
            SoluUtility.SetActiveDisplay(endButton, false);

        }
    }
    void OnDestroy()
    {
        io.Emit("disconnectCliect");
        io.Close();
        Debug.Log("Disconnected");
    }
}

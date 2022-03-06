using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;
using UnityEngine.UI;
using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine.SceneManagement;
using SoluDelegate;
using System.Linq;
using System.IO;
using Solu.Model;
using SoluUtilities;
using Userdata;

public class SetResultGamePage : MonoBehaviour {
    public Text nameText;
    public Text scoreText;
    public GameObject nextRoundButton;
    public SocketIOController io;
    public GameObject namePrefabs;
    public GameObject nameContent;
    public GameObject Player;
    public GameObject board;
    private string nameJsonString;

    void Start () 
    {
        InitiateListener();
        InitSocket();
    }

    void InitSocket()
    {
        io.On(SocketEvent.Connect, (SocketIOEvent e) =>
        {
            Debug.Log("SocketIO connected");
            io.Emit(SocketEvent.SortScore, RoomDelegate.password.ToString(), (string data) =>
            {
                PlayerSortScore players = JsonUtility.FromJson<PlayerSortScore>(data);
                foreach (PlayersModel user in players.data)
                {
                    GameObject setPlayer = Instantiate(namePrefabs) as GameObject;
                    setPlayer.transform.SetParent(board.transform, false);
                    setPlayer.name = user.id;
                    if(user.type == "in")
                    {
                        setPlayer.transform.Find("name").gameObject.GetComponent<Text>().text = user.name;
                    } 
                    else
                    {
                        setPlayer.transform.Find("name").gameObject.GetComponent<Text>().text = user.name + "(out)";
                    }
                    setPlayer.transform.Find("score").gameObject.GetComponent<Text>().text = user.score.ToString();

                }
                if (UserDelegate.status == "banker")
                {
                    SoluUtility.SetActiveDisplay(nextRoundButton, true);
                }
            });
        });

        io.Connect();

        //io.On("changestartscene", (SocketIOEvent changeStartScene) =>
        //{
        //    SceneManager.LoadScene(SceneConstants.PlayGame);
        //});
        io.On(SocketEvent.ChangeStartScene, (SocketIOEvent changeStartScene) =>
        {
            SceneManager.LoadScene(SceneConstants.PlayGame);
        });
    }

    public void InitiateListener()
    {
       
        if (UserDelegate.status == "banker")
        {
            //SoluUtility.SetActiveDisplay(nextRoundButton, true);
            nextRoundButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                io.Emit(SocketEvent.BoardcastToScence, RoomDelegate.password.ToString());
            });
        }
        else
        {
            SoluUtility.SetActiveDisplay(nextRoundButton, false);
        }
    }

    void OnApplicationQuit() //result
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        UserId username = new UserId(UserDelegate._id, RoomDelegate.password, UserDelegate.status, "disconnect");
        io.Emit("playerout", JsonUtility.ToJson(username));
        io.Close();
    }

    void OnDestroy()
    {
        io.Emit("disconnectCliect");
        io.Close();
        Debug.Log("Disconnected");
    }
}

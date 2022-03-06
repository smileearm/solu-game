using UnityEngine;
using UnityEngine.UI;
using Constants;
using System.IO;
using UnityEngine.SceneManagement;
using UnitySocketIO;
using UnitySocketIO.Events;
using Solu.Model;
using SoluDelegate;
using System.Linq;
using Userdata;
using SoluUtilities;
using System.Collections;

public class SetLobbyGamePage : MonoBehaviour
{
    public InputField passwordText;
    public GameObject startGameButton;
    public GameObject cancelGameButton;
    public GameObject numberPlayers;
    public GameObject namePrefabs;
    public GameObject nameContent;
    public GameObject Player;
    public GameObject board;
    public GameObject popupContainer;
    public GameObject InputFieldPassword;
    public GameObject icon;
    public GameObject password;
    public SocketIOController io;

    void Start() {

        SoluUtility.SetActiveDisplay(InputFieldPassword, false);
        SoluUtility.SetActiveDisplay(startGameButton, false);
        SoluUtility.SetActiveDisplay(cancelGameButton, false);
        SoluUtility.SetActiveDisplay(icon, false);
        SoluUtility.SetActiveDisplay(password, false);
        SoluUtility.SetActiveDisplay(numberPlayers, true);

        SocketInit();
        Init();
        InitiateListener();
    }

    void Init() {
        QuestionDelegate.number = 1;
        QuestionDelegate.score = 0;
    }

    void SocketInit() {
        io.On(SocketEvent.Connect, (SocketIOEvent e) => {
            SoluUtility.SetActiveDisplay(InputFieldPassword, true);
            SoluUtility.SetActiveDisplay(icon, true);
            SoluUtility.SetActiveDisplay(password, true);

            if (UserDelegate.status == "banker") {
                roundGameBanker();
            } else {
                roundGamePlayer();
            }

            io.On(SocketEvent.PlayerUpdate, (SocketIOEvent playerUpdate) => {
                UserModel player = JsonUtility.FromJson<UserModel>(playerUpdate.data);
                RoomDelegate.numberPlayer = player.data.numberPlayer;
                numberPlayers.GetComponent<Text>().text = RoomDelegate.numberPlayer.ToString() + "/10 (จำนวนผู้เล่น)";
                if (player.data.type == "in") {
                    ShowName(player.data);
                } else if (player.data.type == "out") {
                    GameObject[] playContainers = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject obj in playContainers) {
                        if (player.data.id == obj.name) {
                            Destroy(obj);
                        }
                    }
                }             });

            io.On(SocketEvent.ChangeStartScene, (SocketIOEvent changeStartScene) => {
                SceneManager.LoadScene(SceneConstants.PlayGame);
            });

            io.On(SocketEvent.ChangeCancelScene, (SocketIOEvent changeStartScene) => {
                QuestionDelegate.firstGame = true;
                SceneManager.LoadScene(SceneConstants.StartGame);
            });

            io.On("bankerOut", (SocketIOEvent data) => {
                Status status = JsonUtility.FromJson<Status>(data.data);
                QuestionDelegate.firstGame = true;
                SoluUtility.SetActiveDisplay(popupContainer, true);
                popupContainer.GetComponentInChildren<Text>().text = status.message;

            });

        });

        io.Connect();
    }

    void roundGameBanker() {
        SoluUtility.SetActiveDisplay(startGameButton, true);
        SoluUtility.SetActiveDisplay(cancelGameButton, true);
        if (QuestionDelegate.firstGame) {
            RoomDelegate.numberPlayer = 0;
            numberPlayers.GetComponent<Text>().text = RoomDelegate.numberPlayer + " / 10 (จำนวนผู้เล่น)";
            Login login = new Login(UserDelegate.username, RoomDelegate.password, UserDelegate.status);
            io.Emit(SocketEvent.CreateRoom, JsonUtility.ToJson(login), (string data) => {
                bankerModel creator = JsonUtility.FromJson<bankerModel>(data);
                UserDelegate._id = creator.data.id;
            });
        } else {
            SetPlayerInRoom();
        }
    }

    void roundGamePlayer() {
        SoluUtility.SetActiveDisplay(cancelGameButton, true);
        if (QuestionDelegate.firstGame) {
            Login login = new Login(UserDelegate.username, RoomDelegate.password, UserDelegate.status);
            io.Emit(SocketEvent.JoinRoom, JsonUtility.ToJson(login), (string dataPlayer) => {
                RoomModel room = JsonUtility.FromJson<RoomModel>(dataPlayer);
                numberPlayers.GetComponent<Text>().text = room.data.numberPlayer.ToString() + " / 10  (จำนวนผู้เล่น)";
                foreach (PlayersModel player in room.data.players) {
                    if(UserDelegate.username == player.name) {
                        UserDelegate._id = player.id;
                    }
                    GameObject setPlayer = Instantiate(namePrefabs) as GameObject;
                    setPlayer.transform.SetParent(board.transform, false);
                    setPlayer.name = player.id;
                    setPlayer.transform.Find("Name").gameObject.GetComponent<Text>().text = player.name;
                }
            });
        } else {
            SetPlayerInRoom();
        }
    }

    void SetPlayerInRoom() {
        UserId username = new UserId(UserDelegate._id, RoomDelegate.password, UserDelegate.status, "");
        io.Emit(SocketEvent.GetPlayerInRoom, JsonUtility.ToJson(username), (string dataPlayer) => {
            RoomModel data = JsonUtility.FromJson<RoomModel>(dataPlayer);
            Debug.Log(data.data.players);
            RoomDelegate.numberPlayer = data.data.numberPlayer;
            numberPlayers.GetComponent<Text>().text = RoomDelegate.numberPlayer.ToString() + " / 10  (จำนวนผู้เล่น)";
            for (int i = 0; i < data.data.players.Count; i++) {
                if (data.data.players[i].type == "in") {
                    GameObject setPlayer = Instantiate(namePrefabs) as GameObject;
                    setPlayer.transform.SetParent(board.transform, false);
                    setPlayer.name = data.data.players[i].id;
                    setPlayer.transform.Find("Name").gameObject.GetComponent<Text>().text = data.data.players[i].name;
                }
            }
        });
    }

    void InitiateListener() {
        passwordText.text = RoomDelegate.password.ToString();

        cancelGameButton.GetComponent<Button>().onClick.AddListener(() => {
            UserId username = new UserId(UserDelegate._id, RoomDelegate.password, UserDelegate.status, "cancelgame");
            io.Emit("exitroom", JsonUtility.ToJson(username), (string data) =>
            {
                QuestionDelegate.firstGame = true;
                SceneManager.LoadScene(SceneConstants.StartGame);
            });
        });

        startGameButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            io.Emit(SocketEvent.StartGame, RoomDelegate.password.ToString(), (string data) => {
                Debug.Log(data);
                checkStatus status = JsonUtility.FromJson<checkStatus>(data);
                Debug.Log(status.status.code);
                if (status.status.code.Equals(0)) {
                    //SoluUtility.SetActiveDisplay(popupContainer, true);
                    //popupContainer.GetComponentInChildren<Text>().text = status.status.message;
                } else {
                    StatusDelegate.code = status.status.code;
                    SoluUtility.SetActiveDisplay(popupContainer, true);
                    popupContainer.GetComponentInChildren<Text>().text = status.status.message;
                }
            }); 
        });

        popupContainer.GetComponentInChildren<Button>().onClick.AddListener(() => {
            if (!StatusDelegate.code.Equals(5)) {
                SceneManager.LoadScene(SceneConstants.StartGame);
            } else {
                SoluUtility.SetActiveDisplay(popupContainer, false);
            }

        });

        Checkstatus checkstatus = new Checkstatus(RoomDelegate.password.ToString(), UserDelegate.status);
        //if (UserDelegate.status == "banker")
        //{
        //    SoluUtility.SetActiveDisplay(startGameButton, true);
        //    SoluUtility.SetActiveDisplay(startGameButton, true);
        //}
    }

    public void ShowName(UserData playerData) {
        GameObject setPlayer = Instantiate(namePrefabs) as GameObject;
        setPlayer.transform.SetParent(board.transform, false);
        setPlayer.name = playerData.id;
        setPlayer.transform.Find("Name").gameObject.GetComponent<Text>().text = playerData.name;
        //setPlayer.transform.Find("Name").gameObject.GetComponent<Text>().color = ""
    }

    void OnApplicationQuit() {
        Debug.Log("ssss");
        UserId username = new UserId(UserDelegate._id, RoomDelegate.password, UserDelegate.status, "disconnect");

        //io.Emit("exitroom", JsonUtility.ToJson(username), (string data) => { 
        //});
        //io.Close();
    }

    void OnDestroy() {
        io.Emit("disconnect");
        io.Close();
        Debug.Log("Disconnected");
    }
}
using UnityEngine;
using UnityEngine.UI;
using Constants;
using SoluDelegate;
using UnityEngine.SceneManagement;
using UnitySocketIO;
using UnitySocketIO.Events;
using Userdata;
using Solu.Model;
using SoluUtilities;
public class SetStartGamePage : MonoBehaviour
{
    public SocketIOController io;
    public InputField playerName;
    public GameObject playGameButton;
    public GameObject createGameButton;
    public GameObject popupContainer;
    public GameObject infoButton;
    public GameObject boardContainer;

    void Start() {
        InitiateListener();
        SocketInit();
    }

    void SocketInit() {
        io.On(SocketEvent.Connect, (SocketIOEvent e) => {
            Debug.Log("SocketIO connected");
        });
        io.Connect();
    }

    void InitiateListener() {
        if (UserDelegate.username == null) {
            playerName.text = "";
        } else {
            playerName.text = UserDelegate.username;
        }
        SoluUtility.SetActiveDisplay(popupContainer, false);

        //infoButton.GetComponent<Button>().onClick.AddListener(() =>
        //{

        //});

        playGameButton.GetComponent<Button>().onClick.AddListener(() => checkTypeUser("player"));
        createGameButton.GetComponent<Button>().onClick.AddListener(() => checkTypeUser("banker"));
        popupContainer.GetComponentInChildren<Button>().onClick.AddListener(() => { SoluUtility.SetActiveDisplay(popupContainer, false); });
    }

    void checkTypeUser(string userType) {
        //Username username = new Username(playerName.text, UserDelegate.checkUsername);
        Username username = new Username(playerName.text);
        io.Emit(SocketEvent.CheckPlayerName, JsonUtility.ToJson(username), (string data) => {
            CheckUsername checkUser = JsonUtility.FromJson<CheckUsername>(data);
            Debug.Log(checkUser.data.username);
            if (checkUser.status.code.Equals(0)) {
                UserDelegate.username = checkUser.data.username;
                if (userType == "player") {
                    UserDelegate.status = "player";
                    SceneManager.LoadScene(SceneConstants.LoginGame);
                } else {
                    UserDelegate.status = "banker";
                    RoomDelegate.password = Random.Range(10000, 100000);
                    SceneManager.LoadScene(SceneConstants.LobbyGame);
                }
            } else {
                SoluUtility.SetActiveDisplay(popupContainer, true);
                popupContainer.GetComponentInChildren<Text>().text = checkUser.status.message;
            }
        });
    }

    void OnDestroy() {
        io.Emit("disconnect");
        io.Close();
        Debug.Log("Disconnected");
    }
}



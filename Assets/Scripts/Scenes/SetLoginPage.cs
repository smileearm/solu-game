using UnityEngine;
using UnityEngine.UI;
using SoluDelegate;
using UnityEngine.SceneManagement;
using Constants;
using UnitySocketIO;
using UnitySocketIO.Events;
using Solu.Model;
using SoluUtilities;
using CheckNameJSON;


public class SetLoginPage : MonoBehaviour
{
    public InputField inputPassword;
    public Text checkPasswordText;
    public GameObject agreeButton;
    public GameObject cancelButton;
    public SocketIOController io;
    public GameObject popupContainer;

    void Start()
    {
        InitiateListener();
        SocketInit();
    }

    void InitiateListener()
    {
        SoluUtility.SetActiveDisplay(popupContainer, false);
        popupContainer.GetComponentInChildren<Button>().onClick.AddListener(() => { SoluUtility.SetActiveDisplay(popupContainer, false); });
        agreeButton.GetComponent<Button>().onClick.AddListener(() => Joinroom());
        cancelButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneConstants.StartGame);
        });
    }

    void SocketInit()
    {
        io.On(SocketEvent.Connect, (SocketIOEvent e) =>
        {
            Debug.Log("SocketIO connected");
        });
        io.Connect();
    }

    void Joinroom()
    {
        CheckName checkName = new CheckName(UserDelegate.username, checkPasswordText.text);
        Debug.Log(JsonUtility.ToJson(checkName));
        QuestionDelegate.firstGame = true;
        io.Emit(SocketEvent.CheckPassword, JsonUtility.ToJson(checkName), (string data) =>
        {
            CheckPasswordModel checkPassword = JsonUtility.FromJson<CheckPasswordModel>(data);
            if (checkPassword.status.code.Equals(0))
            {
                RoomDelegate.password = int.Parse(checkPasswordText.text);
                SceneManager.LoadScene(SceneConstants.LobbyGame);
            }
            else
            {
                SoluUtility.SetActiveDisplay(popupContainer, true);
                popupContainer.GetComponentInChildren<Text>().text = checkPassword.status.message;
            }
        });
    }

    void OnDestroy()
    {
        io.Emit("disconnect");
        io.Close();
        Debug.Log("Disconnected");
    }
}

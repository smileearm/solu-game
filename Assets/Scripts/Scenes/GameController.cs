using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
// using LitJson;
using Constants;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using SoluDelegate;
//using Userdata;
using AnswerJSON;
using UserIdJSON;
using UnitySocketIO;
using UnitySocketIO.Events;
using Solu.Model;
using SoluUtilities;


public class GameController : MonoBehaviour
{
    public GameObject anwserToggleContainer;

    public Text OneToggleText; // Text Toggle
    public Text TwoToggleText; // Text Toggle
    public Text ThreeToggleText; // Text Toggle
    public Text FourToggleText; // Text Toggle
    public Image[] CorrectImage; // Image Correct
    public Image[] CrossImage; // Image Cross

    public Toggle OneToggle;
    public Toggle TwoToggle;
    public Toggle ThreeToggle;
    public Toggle FourToggle;

    public GameObject ansContainer;
    public Button buttonOne;
    public Button buttonTwo;
    public Button buttonThree;
    public Button buttonFour;

    public Sprite spriteImg;

    public Text OneButtonText; // Text Toggle
    public Text TwoButtonText; // Text Toggle
    public Text ThreeButtonText; // Text Toggle
    public Text FourButtonText; // Text Toggle


    public SocketIOController io;
    private QuestionModel questionCollector;

    private string[] choiceRandom = new string[4];
    private string[] choiceText = new string[4];
    private int startTime = 10;

    private int numberWaitAnswer = 0;
    private int numberAnswer = 0;


    public GameObject questionAtContainer;
    public GameObject playerWaitAnswerContainer;
    public GameObject playerAnswerContainer;

    public GameObject questionContainer;
    public GameObject scoreContainer;
    public GameObject timerContainer;


    void Start()
    {
        QuestionDelegate.firstGame = false;
        for (int i = 0; i <= 3; i++)
        {
            CorrectImage[i].enabled = false;
            CrossImage[i].enabled = false;
        }

        anwserToggleContainer.GetComponent<ToggleGroup>().ActiveToggles();
        io.On(SocketEvent.Connect, (SocketIOEvent e) =>
        {
            StartQuestion();
            if (UserDelegate.status == "banker")
            {
                io.Emit("gamecontroller", RoomDelegate.password.ToString());
            }
            else
            {
                io.Emit("soketJoinroom", RoomDelegate.password.ToString());
            }

            io.On("playerAnswer", (SocketIOEvent playerUpdate) =>
            {
                numberAnswer = numberAnswer + 1;
                numberWaitAnswer = numberWaitAnswer - 1;
                playerWaitAnswerContainer.transform.Find("numberPlayerWaitAnswerText").gameObject.GetComponent<Text>().text = numberWaitAnswer.ToString();
                playerAnswerContainer.transform.Find("numberPlayerAnswerText").gameObject.GetComponent<Text>().text = numberAnswer.ToString();
            });

            io.On("skipallplayer", (SocketIOEvent playerUpdate) =>
            {
                StartCoroutine(RouteToLoading());
            });
        });
        io.Connect();

        buttonOne.onClick.AddListener(() => ToggleButtonAnswer(0));
        buttonTwo.onClick.AddListener(() => ToggleButtonAnswer(1));
        buttonThree.onClick.AddListener(() => ToggleButtonAnswer(2));
        buttonFour.onClick.AddListener(() => ToggleButtonAnswer(3));
       
        io.On("countdownTime", (SocketIOEvent socket) =>
        {
            TimerModel timeController = JsonUtility.FromJson<TimerModel>(socket.data); //Time Model
            timerContainer.transform.Find("TimeText").gameObject.GetComponent<Text>().text = timeController.data.countdown.ToString();
            if (RoomDelegate.numberPlayer == numberAnswer && UserDelegate.status == "banker")
            {
                io.Emit("skipGame", RoomDelegate.password.ToString());
            }
            if (timeController.data.countdown == 0)
            {
                timerContainer.transform.Find("TimeText").gameObject.GetComponent<Text>().text = 0.ToString();
                checkAnswer();
            }
        });
    }

    void checkAnswer()
    {
        if (UserDelegate.status == "player")
        {
            for (int i = 0; i <= 3; i++)
            {
                if (questionCollector.data.questionList.choiceList[i].id == questionCollector.data.questionList.answer && UserDelegate.status == "player")
                {
                    CorrectImage[i].enabled = true;
                }
                else if (UserDelegate.status == "player")
                {
                    CrossImage[i].enabled = true;
                }
            }
        }
        StartCoroutine(RouteToLoading());

    }

    IEnumerator RouteToLoading()
    {   
        yield return new WaitForSeconds(3);
        EndRound();
    }

    private void setContrainer(bool data)
    {
        SoluUtility.SetActiveDisplay(questionAtContainer, data);
        SoluUtility.SetActiveDisplay(playerWaitAnswerContainer, data);
        SoluUtility.SetActiveDisplay(playerAnswerContainer, data);
    }
   

    private void EndRound()
    {
        QuestionDelegate.number++;
        if (QuestionDelegate.number < 4)
        {
            SceneManager.LoadScene(SceneConstants.ResultGame);
        }
        else
        {
            SceneManager.LoadScene(SceneConstants.SummaryGame);
        }
    }

    public void StartQuestion()
    {
        io.Emit("question", RoomDelegate.password.ToString(), (string data) =>
        {
            Debug.Log(data);
            SoluUtility.SetActiveDisplay(timerContainer, true);
            timerContainer.transform.Find("TimeText").gameObject.GetComponent<Text>().text = startTime.ToString();
            if (UserDelegate.status == "banker")
            {
                setContrainer(true);
                numberWaitAnswer = RoomDelegate.numberPlayer;
                questionAtContainer.transform.Find("questionAtText").gameObject.GetComponent<Text>().text = "คำถามข้อที่";
                questionAtContainer.transform.Find("numberQuestionText").gameObject.GetComponent<Text>().text = QuestionDelegate.number.ToString();
                playerWaitAnswerContainer.transform.Find("playerWaitAnswerText").gameObject.GetComponent<Text>().text = "จำนวนผู้เล่นที่ยังไม่ตอบ";
                playerWaitAnswerContainer.transform.Find("numberPlayerWaitAnswerText").gameObject.GetComponent<Text>().text = numberWaitAnswer.ToString();
                playerAnswerContainer.transform.Find("playerAnswerText").gameObject.GetComponent<Text>().text = "จำนวนผู้เล่นที่ตอบแล้ว";
                playerAnswerContainer.transform.Find("numberPlayerAnswerText").gameObject.GetComponent<Text>().text = numberAnswer.ToString();

                Button[] ansButtons = ansContainer.GetComponentsInChildren<Button>();
                for (int i = 0; i < ansButtons.Length; i++)
                {
                    ansButtons[i].gameObject.SetActive(false);
                }
            }
            else
            {
                questionCollector = JsonUtility.FromJson<QuestionModel>(data);
                SoluUtility.SetActiveDisplay(questionContainer, true);
                QuestionDelegate.numberQuestion = questionCollector.data.numberProblems;
                for (int i = 0; i < 4; i++)
                {
                    string choice = questionCollector.data.questionList.choiceList[i].id;
                    string number = questionCollector.data.questionList.choiceList[i].text;
                    int randomIndex = Random.Range(i, questionCollector.data.questionList.choiceList.Count);
                    questionCollector.data.questionList.choiceList[i].id = questionCollector.data.questionList.choiceList[randomIndex].id;
                    questionCollector.data.questionList.choiceList[i].text = questionCollector.data.questionList.choiceList[randomIndex].text;
                    questionCollector.data.questionList.choiceList[randomIndex].id = choice;
                    questionCollector.data.questionList.choiceList[randomIndex].text = number;
                    choiceRandom[i] = questionCollector.data.questionList.choiceList[i].id;
                    choiceText[i] = questionCollector.data.questionList.choiceList[i].text;
                }

                questionContainer.transform.Find("QuestionText").gameObject.GetComponent<Text>().text = questionCollector.data.questionList.question;
                buttonOne.gameObject.SetActive(true);
                buttonTwo.gameObject.SetActive(true);
                buttonThree.gameObject.SetActive(true);
                buttonFour.gameObject.SetActive(true);
                if (UserDelegate.status == "player")
                {
                    SoluUtility.SetActiveDisplay(scoreContainer, true);
                    scoreContainer.transform.Find("ScoreText").gameObject.GetComponent<Text>().text = QuestionDelegate.score.ToString();
                }
                OneToggleText.text = choiceText[0];
                OneButtonText.text = choiceText[0];

                TwoToggleText.text = choiceText[1];
                TwoButtonText.text = choiceText[1];

                ThreeToggleText.text = choiceText[2];
                ThreeButtonText.text = choiceText[2];

                FourToggleText.text = choiceText[3];
                FourButtonText.text = choiceText[3];

            }
        });
    }

    void ToggleButtonAnswer(int ans)
    {
        int ansTemp = ans;
        Button[] ansButtons = ansContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < ansButtons.Length ; i++)
        {
            if (i == ans)
            {
                QuestionDelegate.checkAnswer = choiceRandom[i];
                ansButtons[i].enabled = false;
                ansButtons[i].interactable = true;
                Answer answer = new Answer(QuestionDelegate.checkAnswer, QuestionDelegate.numberQuestion, RoomDelegate.password, UserDelegate.username);
                
                io.Emit("checkanswer", JsonUtility.ToJson(answer), (string answer1) =>
                {
                    RoomModel room = JsonUtility.FromJson<RoomModel>(answer1);
                    Debug.Log(i);
                    Debug.Log(room.data.players.Count);
                    for (int j = 0; j < room.data.players.Count; j++)
                    {
                        Debug.Log(room.data.players[j].score);

                        QuestionDelegate.score = room.data.players[j].score;
                    }
                });
               
            }
            else
            {
                ansButtons[i].image.sprite = spriteImg;
                ansButtons[i].enabled = false;
                ansButtons[i].interactable = false;
            }
        }
    }

    void OnApplicationQuit()
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
    }
}

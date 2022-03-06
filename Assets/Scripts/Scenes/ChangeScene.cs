using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Constants;
public class ChangeScene : MonoBehaviour {
    public void StartGame ()
    {
        SceneManager.LoadScene(SceneConstants.StartGame);
    }

    public void CreateGame ()
    {
        SceneManager.LoadScene(SceneConstants.LobbyGame);
    }

    public void LoginGame()
    {
        SceneManager.LoadScene(SceneConstants.LoginGame);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneConstants.PlayGame);
    }

    public void ResultGame()
    {
        SceneManager.LoadScene(SceneConstants.ResultGame);
    }

    public void SummaryGame()
    {
        SceneManager.LoadScene(SceneConstants.SummaryGame);
    }
}

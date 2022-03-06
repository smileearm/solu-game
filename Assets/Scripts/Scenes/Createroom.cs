using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Constants;
using UnityEngine.SceneManagement;


public class Createroom : MonoBehaviour {
    public InputField inputField;
    public Text nameText;


    // Use this for initialization
    void Start () {
        nameText.text = PlayerPrefs.GetString(PersistentKey.Username);
    }

    public void GoCreateRoom()
    {
        SceneManager.LoadScene("CreateGame");

    }
    public void OnSubmit()
    {
        PlayerPrefs.SetString(PersistentKey.Username, inputField.text);
        Debug.Log(inputField.text);
    }
}

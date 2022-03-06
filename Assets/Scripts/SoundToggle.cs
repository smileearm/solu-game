using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    public GameObject SoundOn, SoundOff;

    void Start()
    {
        SoundOff.SetActive(false);
    }

    public void SoundValue()
    {
        bool soundOnOff = gameObject.GetComponent<Toggle>().isOn;

        if (soundOnOff)
        {
            SoundOn.SetActive(true);
            SoundOff.SetActive(false);
        }
        else
        {
            SoundOn.SetActive(false);
            SoundOff.SetActive(true);
        }
    }
}

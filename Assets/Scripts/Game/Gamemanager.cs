using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    [SerializeField] GameObject firstPerson;
    [SerializeField] GameObject secondPerson;
    [SerializeField] GameObject mycam;

    void Start()
    {
        DisablePerson();
        //SetCamera();
    }

    private void SetCamera()
    {
        if (PlayerPrefs.GetString("activePerson") == "2")
        {
            mycam.transform.position = secondPerson.transform.position;
        }
        else if (PlayerPrefs.GetString("activePerson") == "1")
        {
            mycam.transform.position = firstPerson.transform.position;
        }
    }

    private void DisablePerson()
    {
        if (PlayerPrefs.GetString("activePerson") == "1")
        {
            firstPerson.SetActive(false);
            secondPerson.SetActive(true);
        } else if (PlayerPrefs.GetString("activePerson") == "2")
        {
            firstPerson.SetActive(true);
            secondPerson.SetActive(false);
        }
    }
}

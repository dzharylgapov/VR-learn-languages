using Assets.Scripts.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CreateConstructorButton : MonoBehaviour
{
    [SerializeField] GameObject nameOfDialogue;
    [SerializeField] GameObject difficulty;
    [SerializeField] GameObject location;
    [SerializeField] GameObject voice1;
    [SerializeField] GameObject voice2;
    [SerializeField] GameObject question;
    [SerializeField] GameObject rightAnswer;
    [SerializeField] GameObject wrongAnswer;
    [SerializeField] GameObject dialog;

    [SerializeField] GameObject database;
    [SerializeField] GameObject tts;

    public void CreateScenario()
    {
        database.GetComponent<Database>().ExecuteCommand(
            "insert into Exercise(question, rightAnswer, wrongAnswer) " +
            "values ('" + question.GetComponent<TMP_InputField>().text +
                "','" + rightAnswer.GetComponent<TMP_InputField>().text +
                "','" + wrongAnswer.GetComponent<TMP_InputField>().text + "');");
        database.GetComponent<Database>().ExecuteCommand(
            "SELECT id FROM Exercise WHERE ID = (SELECT MAX(ID) FROM Exercise);"
        );
        long exerciseId = (long)(database.GetComponent<Database>().Result[0]);
        var dialogText = dialog.GetComponent<TMP_InputField>().text.Replace("'", "|");
        database.GetComponent<Database>().ExecuteCommand(
            "insert into Dialogue(" +
                "nameOfDialogue, exercise, location, voice1, voice2, difficulty, dialogueText) " +
            "values (" +
                "'" + nameOfDialogue.GetComponent<TMP_InputField>().text + 
                "','" + exerciseId +
                "','" + location.GetComponent<TMP_Dropdown>().value + 
                "','" + voice1.GetComponent<TMP_Dropdown>().value +
                "','" + voice2.GetComponent<TMP_Dropdown>().value +
                "','" + difficulty.GetComponent<TMP_Dropdown>().value +
                "','" + dialogText + "');"
        );
        database.GetComponent<Database>().ClearResult();

        var dialoguePhrases = dialogText.Split("--").Skip(1).ToArray();

        var numOfPhrase = 0;
        foreach (var kv in dialoguePhrases)
        {
            tts.GetComponent<TextToSpeach>().CreateDialogue(
                nameOfDialogue.GetComponent<TMP_InputField>().text, 
                kv, 
                GetDropdownItem(voice1), 
                GetDropdownItem(difficulty),
                numOfPhrase
            );
            numOfPhrase++;
        }
    }

    private string GetDropdownItem(GameObject dropdown)
    {
        //get the selected index
        int menuIndex = dropdown.GetComponent<TMP_Dropdown>().value;
        //get all options available within this dropdown menu
        List<TMP_Dropdown.OptionData> menuOptions = dropdown.GetComponent<TMP_Dropdown>().options;
        //get the string value of the selected index
        string value = menuOptions[menuIndex].text;
        return value;
    }
}

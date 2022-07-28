using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseScenarioButton : MonoBehaviour
{
    [SerializeField] GameObject database;
    [SerializeField] GameObject scenarioButton;

    void Start()
    {
        if (gameObject.name == "chooseScenarios")
        {
            float buttonYposition = scenarioButton.transform.position.y-200;
            int i = 0;
            foreach (var dialogue in database.GetComponent<Database>().NameOfDialogues)
            {
                buttonYposition -= 200;
                GameObject button = Instantiate(scenarioButton,
                    scenarioButton.transform.position + new Vector3(0, buttonYposition, 0),
                    scenarioButton.transform.rotation
                );
		
                button.transform.parent = scenarioButton.transform.parent;
                button.GetComponentInChildren<Text>().text = dialogue;
                button.SetActive(true);
                button.name = database.GetComponent<Database>().IdOfDialogues[i].ToString();
                i += 1;
            }
        }
        
    }

    public void ChooseScenario()
    {
        PlayerPrefs.SetString("nameOfDialogue", GetComponentInChildren<Text>().text);
        PlayerPrefs.SetString("constructor", "yes");

        database.GetComponent<Database>().ExecuteCommand(
            "SELECT dialogueText FROM Dialogue WHERE ID=" + gameObject.name + ";"
        );
        string dialogueText = database.GetComponent<Database>().Result[0].ToString();
        PlayerPrefs.SetString("dialogueText", dialogueText);
        database.GetComponent<Database>().ClearResult();


        database.GetComponent<Database>().ExecuteCommand(
            "SELECT exercise FROM Dialogue WHERE ID=" + gameObject.name + ";"
        );
        var exerciseId = database.GetComponent<Database>().Result[0];
        database.GetComponent<Database>().ClearResult();
        

        database.GetComponent<Database>().ExecuteCommand(
            "SELECT question FROM Exercise WHERE ID=" + exerciseId + ";"
        );
        PlayerPrefs.SetString("question", database.GetComponent<Database>().Result[0].ToString());
        database.GetComponent<Database>().ClearResult();


        database.GetComponent<Database>().ExecuteCommand(
            "SELECT wrongAnswer FROM Exercise WHERE ID=" + exerciseId + ";"
        );
        PlayerPrefs.SetString("wrongAnswer", database.GetComponent<Database>().Result[0].ToString());
        database.GetComponent<Database>().ClearResult();


        database.GetComponent<Database>().ExecuteCommand(
            "SELECT rightAnswer FROM Exercise WHERE ID=" + exerciseId + ";"
        );
        PlayerPrefs.SetString("rightAnswer", database.GetComponent<Database>().Result[0].ToString());
        database.GetComponent<Database>().ClearResult();

        SceneManager.LoadScene(0);
    }

}

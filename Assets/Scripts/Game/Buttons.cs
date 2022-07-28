using Assets.Scripts.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject chooseDialogNumCanvas;
    [SerializeField] GameObject choosePersonCanvas;
    [SerializeField] GameObject chooseToShowHintCanvas;

    [SerializeField] GameObject dialogueManager;
    [SerializeField] GameObject dialogueHint;
    [SerializeField] GameObject showDialogueText;
    [SerializeField] GameObject startDialogue;

    Dictionary<int, List<string>> scenes = new Dictionary<int, List<string>>();

    // Start is called before the first frame update
    void Start()
    {
        scenes.Add(1, new List<string>()
        {
            "dialogue1",
        });
        scenes.Add(2, new List<string>()
        {
            "dialogue2", "dialogue3", "dialogue4", "dialogue5",
        });
        if (PlayerPrefs.GetString("constructor") == "yes")
        {
            PlayerPrefs.SetString("constructor", "no");
            choosePersonCanvas.SetActive(true);
            menuCanvas.SetActive(false);
        }
    }

    public void StartGame()
    {
        chooseDialogNumCanvas.SetActive(true);
        menuCanvas.SetActive(false);
    }

    public void StartConstructor()
    {
        SceneManager.LoadScene(3);
    }

    public void ChooseFirstPerson()
    {
        PlayerPrefs.SetString("activePerson", "1");
        chooseToShowHintCanvas.SetActive(true);
        choosePersonCanvas.SetActive(false);
    }

    public void ChooseSecondPerson()
    {
        PlayerPrefs.SetString("activePerson", "2");
        chooseToShowHintCanvas.SetActive(true);
        choosePersonCanvas.SetActive(false);
    }

    public void StartDialogue()
    {
        dialogueHint.SetActive(false);
        showDialogueText.SetActive(false);
        dialogueManager.GetComponent<DialogueManager>().StartDialogue();
        startDialogue.SetActive(false);
    }

    public void ShowDialogueText()
    {
        dialogueHint.SetActive(true);
        startDialogue.SetActive(false);
        showDialogueText.SetActive(false);
    }

    public void HideDialogueText()
    {
        startDialogue.SetActive(true);
        showDialogueText.SetActive(true);
        dialogueHint.SetActive(false);
    }

    private void FindScene()
    {
        int sceneNum = 1;
        foreach (KeyValuePair<int, List<string>> entry in scenes)
        { 
            if (entry.Value.Contains(PlayerPrefs.GetString("needShowPhraseHint")))
            {
                sceneNum = entry.Key;
                break;
            }
        }
        SceneManager.LoadScene(sceneNum);
    }

    public void NeedShowPhraseHint()
    {
        PlayerPrefs.SetString("needShowPhraseHint", "true");
        FindScene();
    }

    public void DontNeedShowPhraseHint()
    {
        PlayerPrefs.SetString("needShowPhraseHint", "false");
        FindScene();
    }

    public void ChooseDialogue1()
    {
        PlayerPrefs.SetString("nameOfDialogue", "dialogue1");
        choosePersonCanvas.SetActive(true);
        chooseDialogNumCanvas.SetActive(false);
    }

    public void ChooseDialogue2()
    {
        PlayerPrefs.SetString("nameOfDialogue", "dialogue2");
        choosePersonCanvas.SetActive(true);
        chooseDialogNumCanvas.SetActive(false);
    }

    public void ChooseDialogue3()
    {
        PlayerPrefs.SetString("nameOfDialogue", "dialogue3");
        choosePersonCanvas.SetActive(true);
        chooseDialogNumCanvas.SetActive(false);
    }

    public void ChooseDialogue4()
    {
        PlayerPrefs.SetString("nameOfDialogue", "dialogue4");
        choosePersonCanvas.SetActive(true);
        chooseDialogNumCanvas.SetActive(false);
    }

    public void ChooseDialogue5()
    {
        PlayerPrefs.SetString("nameOfDialogue", "dialogue5");
        choosePersonCanvas.SetActive(true);
        chooseDialogNumCanvas.SetActive(false);
    }
}

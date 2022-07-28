using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Api;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Net;
using Assets.Scripts.Game;
using UnityEngine.Networking;
using TMPro;
using System.Linq;

namespace Assets.Scripts.Gameplay
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] GameObject tts;
        [SerializeField] GameObject stt;

        [SerializeField] GameObject botIfFirstPlayerActive;
        [SerializeField] GameObject botIfSecondPlayerActive;
        [SerializeField] GameObject activeFirstPerson;
        [SerializeField] GameObject activeSecondPerson;
        
        [SerializeField] GameObject textForFirstPerson;
        [SerializeField] GameObject textForSecondPerson;
        [SerializeField] GameObject phraseHintForFirstPerson;
        [SerializeField] GameObject phraseHintForSecondPerson;
        [SerializeField] GameObject exerciseForFirstPersonCanvas;
        [SerializeField] GameObject exerciseForSecondPersonCanvas;

        private string dialogueName;
        private Dictionary<string, string> dialogue = new Dictionary<string, string>();
        private GameObject phraseHint;
        private GameObject textDialog;
        private GameObject exerciseCanvas;

        private bool isDialogueStarted = false;
        bool botFirstPhrasePlayed = false;
        int countOfDonePhrases = 0;
        string activePerson;
        bool phraseIsDone = true;
        bool speechSuccess = true;
        bool phraseIsPlaying = false;
        string recognizeResult = "";
        int countFails = 0;

        private string path;

        // Use this for initialization
        void Start()
        {
            //katherine (или katherine-hifigan) — по умолчанию; maria (или maria-serious); pavel (или pavel-hifigan)
            string modelName = "pavel-hifigan";
            //от 0.75 до 1.75
            string tempo = "1";
            dialogueName = PlayerPrefs.GetString("nameOfDialogue", "dialogue1");
            path = "Assets/Dialogues/" + dialogueName + "/";
            InitDialogueText(dialogueName);

            activePerson = PlayerPrefs.GetString("activePerson", "2");

            

            if (activePerson == "1")
            {
                textDialog = textForSecondPerson;
                phraseHint = phraseHintForSecondPerson;
                exerciseCanvas = exerciseForSecondPersonCanvas;
            } else if (activePerson == "2")
            {
                textDialog = textForFirstPerson;
                phraseHint = phraseHintForFirstPerson;
                exerciseCanvas = exerciseForFirstPersonCanvas;
            }
            
            CreateDialogueTextHint();
        }

        private void InitDialogueText(string dialogueName)
        {
            switch (dialogueName)
            {
                case "dialogue1":
                    dialogue = new Dictionary<string, string>()
                    {
                        { "0", "Please, lay your bags flat on the conveyor belt, and use the bins for small objects." +
                        "please, step back. Take off your hat, your belt and your shoes, too." +
                        "Do you have anything in your pockets- keys, cell phone?" },
                        { "1", "just a minute. well, I don’t think so. Let me try taking off my belt." },
                        { "2", "Come on through. you’re all set! Have a nice flight." },
                    };
                    break;
                case "dialogue2":
                    dialogue = new Dictionary<string, string>()
                    {
                        { "0", "How can I help you?" },
                        { "1", "I want to fly to Rome at the beginning of November. Can I buy tickets?" },
                        { "2", "Of course. What day exactly?" },
                        { "3", "The 4th of November."},
                        { "4", "All right. Let me check. Yes,there is a flight from Moscow to Rome that day, at 11 a.m." },
                        { "5", "great! And I would like a return ticket too, for the 17 th of November." },
                        { "6", "I see.We have a flight at half past 4."},
                        { "7", "oh, thank you.I’ll take it."},
                    };
                    break;
                case "dialogue3":
                    dialogue = new Dictionary<string, string>()
                    {
                        { "0", "Hello! I would like to check in for the flight." },
                        { "1", "Can I see your documents and the ticket, please ?" },
                        { "2", "Here you are." },
                        { "3", "Thank you, mister Ivanov.Do you have any luggage ?"},
                        { "4", "Only hand luggage." },
                        { "5", "Here’s your boarding pass.Have a safe flight!" },
                    };
                    break;
                case "dialogue4":
                    dialogue = new Dictionary<string, string>()
                    {
                        { "0", "Hello! How can I help you?" },
                        { "1", "Are there any flights to Sochi tomorrow?" },
                        { "2", "Just a second. Yes, there are two: one leaves at 7 am and the other one at 7 pm. " },
                        { "3", "We’d like to buy two tickets for tomorrow at 7 pm."},
                        { "4", " Economy, business class or first class?" },
                        { "5", "Economy, please. " },
                        { "6", "Give me your documents, please… Alright, 300 dollars, please. Would you like to pay in cash or by credit card?" },
                        { "7", "By card, thank you!" }
                    };
                    break;
                case "dialogue5":
                    dialogue = new Dictionary<string, string>()
                    {
                        { "0", "How can I help you?" },
                        { "1", "I would like to exchange my tickets." },
                        { "2", "It’s to Saint-Petersburg. But it turned out I can’t leave, so I want to exchange them or make a refund." },
                        { "3", "Let me see. I’m sorry, sir, we can’t exchange it or make a refund. We only exchange business class tickets, and this is economy."},
                    };
                    break;
                default:
                    var dialogueText = PlayerPrefs.GetString("dialogueText").Split("--").Skip(1).ToArray();
                    int i = 0;
                    foreach (var phrase in dialogueText)
                    {
                        dialogue.Add(i.ToString(), phrase.Replace("|", "'"));
                        i += 1;
                    }
                    break;
            }
        }

        /*public void CheckIfNeedCreateDialgoue(string dialogueName, string modelName, string tempo)
        {
            
        }*/

        public void CreateDialogueTextHint()
        {
            foreach (var key in dialogue.Values)
            {
                textDialog.GetComponent<TextMeshProUGUI>().text += key + "\n";
            }
            
        }

        public void StartDialogue()
        {
            isDialogueStarted = true;
            if (PlayerPrefs.GetString("needShowPhraseHint", "false") == "true")
            {
                phraseHint.SetActive(true);
            }
        }

        public void ChangePhraseHint(string phrase)
        {
            phraseHint.GetComponentInChildren<TextMeshProUGUI>().text = phrase;
        }

        private void Update()
        {
            if (isDialogueStarted && countOfDonePhrases < dialogue.Count && !phraseIsPlaying)
            {
                phraseIsPlaying = true;
                
                recognizeResult = "";
                
                if (activePerson == "1")
                {
                    ChangePhraseHint(dialogue[countOfDonePhrases.ToString()]);
                    if (phraseIsDone)
                    {
                        StartCoroutine(
                            HumanSpeak((string ReturnResult) =>
                            {
                                recognizeResult = ReturnResult;
                                speechSuccess = stt.GetComponent<SpeachToText>().CompareAnswer(
                                    recognizeResult,
                                    dialogue[countOfDonePhrases.ToString()]
                                    );
                                if (countFails == 3)
                                {
                                    speechSuccess = true;
                                }
                                if (speechSuccess)
                                {
                                    countOfDonePhrases += 1;
                                    ChangePhraseHint(dialogue[countOfDonePhrases.ToString()]);
                                    // TODO: use bot script
                                    StartCoroutine(PlayDialogue(
                                    countOfDonePhrases,
                                    (bool ReturnResult) =>
                                        {
                                            
                                            phraseIsDone = ReturnResult;
                                            if (phraseIsDone)
                                            {
                                                countOfDonePhrases += 1;
                                            }
                                            phraseIsPlaying = false;
                                        }
                                    ));
                                }
                                else
                                {
                                    countFails += 1;
                                    StartCoroutine(AskToRepeat(
                                    (bool ReturnResult) =>
                                        {
                                            phraseIsDone = ReturnResult;
                                            phraseIsPlaying = false;
                                        }
                                    ));
                                }
                            })
                        );
                    }
                }
                else if (activePerson == "2")
                {
                    Debug.Log("countOfDonePhrases " + countOfDonePhrases);
                    ChangePhraseHint(dialogue[countOfDonePhrases.ToString()]);
                    if (!botFirstPhrasePlayed)
                    {
                        StartCoroutine(PlayDialogue(
                            countOfDonePhrases,
                            (bool ReturnResult) =>
                            {
                                phraseIsDone = ReturnResult;
                                if (phraseIsDone)
                                {
                                    countOfDonePhrases += 1;
                                    botFirstPhrasePlayed = true;
                                }
                                phraseIsPlaying = false;
                            }
                        ));
                    }
                    else
                    {
                        if (phraseIsDone)
                        {
                            StartCoroutine(
                                HumanSpeak((string ReturnResult) =>
                                {
                                    recognizeResult = ReturnResult;
                                    speechSuccess = stt.GetComponent<SpeachToText>().CompareAnswer(
                                        recognizeResult,
                                        dialogue[countOfDonePhrases.ToString()]
                                        );
                                    if (countFails == 3)
                                    {
                                        speechSuccess = true;
                                    }
                                    if (speechSuccess)
                                    {
                                        countOfDonePhrases += 1;
                                        ChangePhraseHint(dialogue[countOfDonePhrases.ToString()]);
                                        // TODO: use bot script
                                        StartCoroutine(PlayDialogue(
                                            countOfDonePhrases,
                                            (bool ReturnResult) =>
                                            {
                                                countOfDonePhrases += 1;
                                                phraseIsDone = ReturnResult;
                                                phraseIsPlaying = false;
                                            }
                                        ));
                                    }
                                    else
                                    {
                                        countFails += 1;
                                        StartCoroutine(AskToRepeat(
                                        (bool ReturnResult) =>
                                        {
                                            phraseIsDone = ReturnResult;
                                            phraseIsPlaying = false;
                                        }
                                        ));
                                    }
                                })
                            );
                        }
                    }
                    /*if (speechSuccess)
                    {
                        speechSuccess = false;
                        StartCoroutine(PlayDialogue(
                            countOfDonePhrases,
                            (bool ReturnResult) =>
                            {
                                countOfDonePhrases += 1;
                                phraseIsDone = ReturnResult;
                                phraseIsPlaying = false;

                                if (phraseIsDone)
                                {
                                    string recognizeResult = "";
                                    StartCoroutine(
                                        HumanSpeak((string ReturnResult) =>
                                        {
                                            recognizeResult = ReturnResult;
                                            speechSuccess = stt.GetComponent<SpeachToText>().CompareAnswer(
                                                recognizeResult,
                                                dialogue[countOfDonePhrases.ToString()]
                                                );
                                            if (!speechSuccess)
                                            {
                                                StartCoroutine(AskToRepeat(
                                                    (bool ReturnResult) =>
                                                    {
                                                        phraseIsDone = ReturnResult;
                                                        phraseIsPlaying = false;
                                                    }
                                                ));
                                            }
                                            else
                                            {
                                                countOfDonePhrases += 1;
                                            }
                                        })
                                    );
                                }
                            }
                        ));
                    }*/
                    
                    
                }
            }
            else if (isDialogueStarted && countOfDonePhrases == dialogue.Count)
            {
                phraseHint.SetActive(false);
                exerciseCanvas.SetActive(true);
            }
        }

        public IEnumerator PlayDialogue(int numOfPhrase, Action<bool> FinishDelegate)
        {
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
                "file:///" + path + numOfPhrase + ".mp3", AudioType.MPEG
            );
            yield return www.SendWebRequest();
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length + 0.1f);
            if (!GetComponent<AudioSource>().isPlaying)
            {
                FinishDelegate(true);
            }
        }

        public IEnumerator AskToRepeat(Action<bool> FinishDelegate)
        {
            Debug.Log("file:///" + path + "repeatpls" + ".mp3");
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
                "file:///" + path + "repeatpls" + ".mp3", AudioType.MPEG
            );
            yield return www.SendWebRequest();
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length + 0.1f);
            if (!GetComponent<AudioSource>().isPlaying)
            {
                FinishDelegate(true);
            }
        }


        public IEnumerator HumanSpeak(Action<string> FinishDelegate)
        {

            string phraseFilePath = "";
            string recognizeResult = "";
            yield return StartCoroutine(stt.GetComponent<SpeachToText>().RecordFromMic(
                dialogueName, 
                (string ReturnResult) => phraseFilePath = ReturnResult
                ));
            
            
            if (!Microphone.IsRecording(null))
            {
                StartCoroutine(
                    stt.GetComponent<SpeachToText>().MicRecognizeAsync(
                        phraseFilePath, 
                        (string ReturnResult) =>
                        {
                            recognizeResult = ReturnResult;
                            Debug.Log(recognizeResult);
                            FinishDelegate(recognizeResult);
                        })
                );
                
            }

            
        }
    }

}
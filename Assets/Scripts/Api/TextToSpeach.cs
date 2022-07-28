using System.Collections;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Net.Http;
using Assets.Scripts.Dialogue;
using System.Threading.Tasks;
using System.Net;
using CloudConvert.API;
using CloudConvert.API.Models.ExportOperations;
using CloudConvert.API.Models.ImportOperations;
using CloudConvert.API.Models.JobModels;
using CloudConvert.API.Models.TaskOperations;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

namespace Assets.Scripts.Api
{
    public class TextToSpeach: MonoBehaviour
    {
        [SerializeField] GameObject dialogueManager;

        private string path;

        // Use this for initialization
        void Start()
        {
            /*StartCoroutine(Auth.PostRequest(
                "https://iam.api.cloud.yandex.net/iam/v1/tokens",
                "{'yandexPassportOauthToken' : '" + Auth.YandexPassportOauthToken + "'}"));*/

            path = Application.dataPath + "/Dialogues/";
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                path = path.Substring(0, path.LastIndexOf('/') - 1).Substring(0, path.LastIndexOf('/') - 1).Substring(0, path.LastIndexOf('/') - 1);
                path = path.Substring(0, path.LastIndexOf('/'));
            }
            
        }

        public void CreateDialogue(string dialogueName, string textPhrase, string modelName, string tempo, int numOfPhrase)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Auth.VkSolutionAccessToken);

            textPhrase.Replace(" ", "%20");
            var response = client.GetAsync(
                "https://voice.mcs.mail.ru/tts?encoder=mp3&text=" + textPhrase + "&model_name=" + modelName + "&tempo=" + tempo
                );
            if (response.Result.StatusCode.ToString() == "OK")
            {
                bool exists = Directory.Exists("Assets/Dialogues/" + dialogueName + "/");
                if (!exists)
                {
                    Directory.CreateDirectory("Assets/Dialogues/" + dialogueName + "/");
                }
                // TODO; make appropriate naming
                File.WriteAllBytes("Assets/Dialogues/" + dialogueName + "/" + numOfPhrase + ".mp3", response.Result.Content.ReadAsByteArrayAsync().Result);
            }
            CreateRepeatPhrase(dialogueName, modelName, tempo);
        }

        public void CreateRepeatPhrase(string dialogueName, string modelName, string tempo)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Auth.VkSolutionAccessToken);

            string textPhrase = "Can you repeat, please";
            textPhrase.Replace(" ", "%20");
            var response = client.GetAsync(
                "https://voice.mcs.mail.ru/tts?encoder=mp3&text=" + textPhrase + "&model_name=" + modelName + "&tempo=" + tempo
                );
            if (response.Result.StatusCode.ToString() == "OK")
            {
                bool exists = Directory.Exists("Assets/Dialogues/" + dialogueName + "/");
                if (!exists)
                {
                    Directory.CreateDirectory("Assets/Dialogues/" + dialogueName + "/");
                }
                // TODO; make appropriate naming
                File.WriteAllBytes("Assets/Dialogues/" + dialogueName + "/repeatpls.mp3", response.Result.Content.ReadAsByteArrayAsync().Result);
            }
        }

        private string CreateDirectory()
        {

            Debug.Log("path=" + path);
            // Make sure directory exists if user is saving to sub dir.
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            return path;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
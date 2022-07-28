using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Dialogue
{
    public class Auth
    {
        // todo: hide these tokens
        private static string iamToken = "";
        private const string yandexPassportOauthToken = "";
        private const string folderId = "b1gunvgrdhm1kafkt9qs";

        private const string cloudConverAccessToken =
            "";
        private const string vkSolutionAccessToken = "";

        public static string IamToken { get => iamToken; set => iamToken = value; }
        public static string YandexPassportOauthToken { get => yandexPassportOauthToken; }
        public static string FolderId { get => folderId; }
        public static string CloudConverAccessToken => cloudConverAccessToken;
        public static string VkSolutionAccessToken { get => vkSolutionAccessToken; }

        public struct Token
        {
            public string iamToken;
            public string expiresAt;
        }

        public static IEnumerator PostRequest(string url, string json)
        {
            var uwr = new UnityWebRequest(url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                IamToken = JsonUtility.FromJson<Token>(uwr.downloadHandler.text).iamToken;
            }
        }
    }
}
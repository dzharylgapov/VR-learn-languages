using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class Bot : MonoBehaviour
    {
        private string path = "Assets/Dialogues/" + "temp" + "/";

        // Use this for initialization
        void Start()
        {

        }

        /*public IEnumerator PlayDialogue(int numOfPhrase, Action<bool> FinishDelegate)
        {
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
                "file:///" + path + numOfPhrase + ".mp3", AudioType.MPEG
            );
            yield return www.SendWebRequest();
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
            if (!GetComponent<AudioSource>().isPlaying)
            {
                FinishDelegate(true);
            }
        }

        public IEnumerator AskToRepeat(Action<bool> FinishDelegate)
        {
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
                "file:///" + path + "repeatpls" + ".mp3", AudioType.MPEG
            );
            yield return www.SendWebRequest();
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            GetComponent<AudioSource>().clip = clip;
            if (!GetComponent<AudioSource>().isPlaying)
            {
                FinishDelegate(true);
            }
            else
            {
                FinishDelegate(false);
            }
        }*/
    }
}
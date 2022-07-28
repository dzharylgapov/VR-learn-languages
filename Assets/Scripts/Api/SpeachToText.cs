using UnityEngine;
using System;
using System.Threading.Tasks;
using System.IO;
using ITCC.YandexSpeechKitClient;
using ITCC.YandexSpeechKitClient.Enums;
using ITCC.YandexSpeechKitClient.Models;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Dialogue;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Net;

namespace Assets.Scripts.Api
{
    public class SpeachToText : MonoBehaviour
    {
        private static int HEADER_SIZE = 44;
        //[SerializeField] GameObject dialogueManager;

        private AudioSource audioSource;

        private int similarityBoundary = 60;
        private bool isSpeaking = false;
        private bool startedSpeaking = false;
        private string lastRecognizedText;

        string[] reductions =
        {
            "'m","'d","'ll","'ve","'re","'s","'t"
        };
        string[] interjections =
        {
            "Oh", "Ah", "Hey"
        };

        int sample_size = 128;
        string micro_;
        private float level_;

        private float lastTimeSpeaking;

        public AudioSource AudioSource { get => audioSource; set => audioSource = value; }

        private void Start()
        {
            AudioSource = GetComponent<AudioSource>();
            micro_ = Microphone.devices[0];
        }

        private void Update()
        {
            if (Microphone.IsRecording(null))
            {
                SpeakingOrNot();
                //Debug.Log(isSpeaking);
            }
            if (isSpeaking && Time.time > lastTimeSpeaking + 3f)
            {
                isSpeaking = false;
            }
        }

        #region Speach detection
        public void SpeakingOrNot()
        {
            float[] spectrum = new float[sample_size];
            level_ = 0;
            int mic_pos = Microphone.GetPosition(null) - (sample_size + 1);

            if (mic_pos < 0)
            {

                return;
            }
            AudioSource.clip.GetData(spectrum, mic_pos);
            for (int i = 0; i < spectrum.Length; i++)
            {
                float peak = spectrum[i] * spectrum[i];
                if (level_ < peak)
                {
                    level_ = peak;
                    isSpeaking = true;
                    lastTimeSpeaking = Time.time;
                }
                /*else
                {
                    isSpeaking = false;
                }*/
            }
        }

        #endregion

        public bool CompareAnswer(string recognizeResult, string phrase)
        {
            float countSilmillarWords = 0;
            recognizeResult = recognizeResult.ToLower();
            string[] allPhrases = phrase.Replace("'", " ").ToLower().Split(' ');
            for (int i = 0; i < allPhrases.Length; i++)
            {
                if (recognizeResult.Contains(allPhrases[i]))
                {
                    countSilmillarWords += 1;
                }
            }

            int lenPhrases = allPhrases.Length;
            for (int i = 0; i < reductions.Length; i++)
            {
                if (phrase.Contains(reductions[i]))
                {
                    lenPhrases -= 1;
                }
            }
            for (int i = 0; i < interjections.Length; i++)
            {
                if (phrase.Contains(interjections[i]))
                {
                    lenPhrases -= 1;
                }
            }

            //Debug.Log("countSilmillarWords/allPhrases.Length = " + countSilmillarWords / allPhrases.Length);

            float limit = 0.70f;
            if (lenPhrases >= 10)
            {
                limit = 0.60f;
            }
            Debug.Log("countSilmillarWords = " + countSilmillarWords);
            Debug.Log("lenPhrases = " + lenPhrases);
            Debug.Log("countSilmillarWords/lenPhrases = " + countSilmillarWords / lenPhrases);
            if (countSilmillarWords/lenPhrases >= limit)
            {
                return true;
            }

            return false;
        }


        #region async recognize

        public IEnumerator MicRecognizeAsync(string path, Action<string> FinishDelegate)
        {
            string url = "https://voice.mcs.mail.ru/asr";
            if (Microphone.devices.Length < 1)
            {
                Console.WriteLine("No microphone!");
                yield return null;
            }
            WWWForm form = new WWWForm();
            string allPath = Application.dataPath + "\\Records\\" + path + ".wav";
            byte[] bytes = File.ReadAllBytes(allPath);
            form.AddBinaryData("file", bytes, "file.wav", "audio/wave");
            //form.AddBinaryData("file", File.ReadAllBytes(Application.dataPath + "\\Records\\dialogue in airport//ss.ogg"), "file.ogg", "audio/ogg; codecs=opus");
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            UploadHandler customUploadHandler = new UploadHandlerRaw(bytes);
            customUploadHandler.contentType = "audio/wave";
            www.uploadHandler = customUploadHandler;
            www.SetRequestHeader("Authorization", "Bearer " + Auth.VkSolutionAccessToken);
            www.SetRequestHeader("Content-Type", "audio/wave");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.responseCode);
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                FinishDelegate(www.downloadHandler.text);
            }
        }
        #endregion

        public IEnumerator RecordFromMic(string dialogueName, Action<string> FinishDelegate)
        {
            string microphoneName = "";

            if (Microphone.devices.Length < 1)
            {
                Debug.Log("No microphone!");
            }
            else
            {
                microphoneName = Microphone.devices[0];
                //Debug.Log(microphoneName);
            }

            AudioSource.clip = Microphone.Start(microphoneName, true, 10, 44100);
            Debug.Log("Speak now.");

            yield return new WaitForSeconds(3);
            if (isSpeaking)
            {
                yield return new WaitForSeconds(3);
            }

            Microphone.End(microphoneName);
            Debug.Log("delay end");

            //TODO: ������� ����������� ������������ ���� ������
            string phraseFilePath = dialogueName + "/" + DateTime.Now.ToString("dd/MM/yyyy") + "_" + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            Save(phraseFilePath, AudioSource.clip);
            FinishDelegate(phraseFilePath);
        }

        public static bool Save(string filename, AudioClip clip)
        {
            if (!filename.ToLower().EndsWith(".wav"))
            {
                filename += ".wav";
            }

            var _path = Application.dataPath + "/Records/";
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                _path = _path.Substring(0, _path.LastIndexOf('/') - 1).Substring(0, _path.LastIndexOf('/') - 1).Substring(0, _path.LastIndexOf('/') - 1);
                _path = _path.Substring(0, _path.LastIndexOf('/'));
            }
            var filepath = _path + filename;

            Debug.Log(filepath);

            //if (!Application.isEditor)
            {
                // Make sure directory exists if user is saving to sub dir.
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));

                using (var fileStream = CreateEmpty(filepath))
                {
                    ConvertAndWrite(fileStream, clip);
                    WriteHeader(fileStream, clip);
                }
            }
            return true; // TODO: return false if there's a failure saving the file
        }

        #region Process audio

        //Returns data from an AudioClip as a byte array.
        public static byte[] GetClipData(AudioClip _clip)
        {
            var samples = new float[_clip.samples];

            _clip.GetData(samples, 0);

            Int16[] intData = new Int16[samples.Length];

            Byte[] bytesData = new Byte[samples.Length * 2];

            int rescaleFactor = 32767;

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * rescaleFactor);
                Byte[] byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            return bytesData;
        }

        static FileStream CreateEmpty(string filepath)
        {
            var fileStream = new FileStream(filepath, FileMode.Create);

            byte emptyByte = new byte();

            for (int i = 0; i < HEADER_SIZE; i++)
            { //preparing the header

                fileStream.WriteByte(emptyByte);

            }

            return fileStream;

        }

        static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
        {
            var samples = new float[clip.samples];
            clip.GetData(samples, 0);
            Int16[] intData = new Int16[samples.Length];

            //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

            Byte[] bytesData = new Byte[samples.Length * 2];

            //bytesData array is twice the size of

            //dataSource array because a float converted in Int16 is 2 bytes.

            int rescaleFactor = 32767; //to convert float to Int16

            for (int i = 0; i < samples.Length; i++)
            {

                intData[i] = (short)(samples[i] * rescaleFactor);

                Byte[] byteArr = new Byte[2];

                byteArr = BitConverter.GetBytes(intData[i]);

                byteArr.CopyTo(bytesData, i * 2);

            }

            fileStream.Write(bytesData, 0, bytesData.Length);

        }

        static void WriteHeader(FileStream fileStream, AudioClip clip)
        {
            var hz = clip.frequency;

            var channels = clip.channels;

            var samples = clip.samples;

            fileStream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");

            fileStream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);

            fileStream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");

            fileStream.Write(wave, 0, 4);


            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");

            fileStream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);

            fileStream.Write(subChunk1, 0, 4);

            //UInt16 two = 2;

            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);

            fileStream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(channels);

            fileStream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(hz);

            fileStream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2

            fileStream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(channels * 2);

            fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;

            Byte[] bitsPerSample = BitConverter.GetBytes(bps);

            fileStream.Write(bitsPerSample, 0, 2);

            Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");

            fileStream.Write(datastring, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);

            fileStream.Write(subChunk2, 0, 4);
            //        fileStream.Close();
        }

        #endregion

    }
}

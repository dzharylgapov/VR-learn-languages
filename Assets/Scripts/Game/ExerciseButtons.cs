using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExerciseButtons : MonoBehaviour
{
    [SerializeField] GameObject exerciseCanvas;
    [SerializeField] GameObject endCanvas;

    public void RightAnswer()
    {
        endCanvas.SetActive(true);
        endCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "Great";
        exerciseCanvas.SetActive(false);
    }

    public void WrongAnswer()
    {
        endCanvas.SetActive(true);
        endCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "Bad";
        exerciseCanvas.SetActive(false);
    }
}

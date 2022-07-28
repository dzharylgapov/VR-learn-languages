using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructorButtons : MonoBehaviour
{
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject createScenarioCanvas;
    [SerializeField] GameObject chooseScenarioCanvas;

    public void ViewAllScenarios()
    {
        chooseScenarioCanvas.SetActive(true);
        menuCanvas.SetActive(false);
    }

    public void OpenConstructor()
    {
        createScenarioCanvas.SetActive(true);
        menuCanvas.SetActive(false);
    }
}

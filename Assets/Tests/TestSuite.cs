using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Assets.Scripts.Api;

public class TestSuite
{
    // A Test behaves as an ordinary method
    [Test]
    public void IsVrObjectsExist()
    {
        // Use the Assert class to test conditions
        GameObject OVRCameraRig = GameObject.Find("OVRCameraRig");
        Assert.AreEqual("OVRCameraRig", OVRCameraRig.name);

        GameObject localAvatar = GameObject.Find("LocalAvatar");
        Assert.AreEqual("LocalAvatar", localAvatar.name);

        GameObject uiHelpers = GameObject.Find("UIHelpers");
        Assert.AreEqual("UIHelpers", uiHelpers.name);

        GameObject uiHelpers2 = GameObject.Find("UIHelpers (1)");
        Assert.AreEqual("UIHelpers (1)", uiHelpers2.name);
    }

    [Test]
    public void CheckCanvasPreferences()
    {
        GameObject menuCanvas = GameObject.Find("Canvas");
        Assert.AreEqual("WorldSpace", menuCanvas.GetComponent<Canvas>().renderMode.ToString());
    }

    [Test]
    public void NotVrObjectsDontExist()
    {
        // Use the Assert class to test conditions
        GameObject MainCamera = GameObject.Find("MainCamera");
        Assert.AreEqual(null, MainCamera);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        
        yield return null;
    }
}

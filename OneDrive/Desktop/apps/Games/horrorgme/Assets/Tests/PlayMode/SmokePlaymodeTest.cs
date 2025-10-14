using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmokePlaymodeTest
{
    [Test]
    public void Load_Game_Scene_Smoke()
    {
        SceneManager.LoadScene("Game");
        Assert.Pass();
    }
}



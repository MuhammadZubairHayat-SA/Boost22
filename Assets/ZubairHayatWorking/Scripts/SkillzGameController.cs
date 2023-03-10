using SkillzSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SkillzGameController : SkillzMatchDelegate
{
    // Called when a player chooses a tournament and the match countdown expires
    public void OnMatchWillBegin(Match matchInfo)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene_Skillz");
    }

    // Called when a player clicks the Progression entry point or side menu. Explained in later steps
    public void OnProgressionRoomEnter()
    {

    }

    // Called when a player chooses Exit Skillz from the side menu
    public void OnSkillzWillExit()
    {
        Screen.orientation=ScreenOrientation.LandscapeLeft;
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScreen_Skillz");
    }
}

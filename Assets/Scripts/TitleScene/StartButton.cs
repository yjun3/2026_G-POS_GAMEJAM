using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void GoToMainScene()
    {
        SceneManager.LoadScene("Mainscene");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private string gameSceneName;

    public void StartGameScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
        Debug.Log("게임 시작됨 아자스");
    }
}
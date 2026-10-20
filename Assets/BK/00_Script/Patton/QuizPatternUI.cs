using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 전투 내내 유지되는 Canvas입니다. 퀴즈가 끝나도 보스 이름과 체력바는 숨기지 않습니다.
public class QuizPatternUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button oButton;
    [SerializeField] private Button xButton;
    [SerializeField] private Slider bossHealthBar;

    public bool IsConfigured => questionText != null && oButton != null &&
                                xButton != null && bossHealthBar != null;

    private void Awake()
    {
        HideQuiz();
        BindBoss(FindFirstObjectByType<BossHealthSystem>());
    }

    public void BindBoss(BossHealthSystem boss)
    {
        if (boss != null && bossHealthBar != null)
            boss.BindHealthSlider(bossHealthBar);
    }

    public void ShowQuestion(string question, UnityAction onO, UnityAction onX)
    {
        questionText.text = question;
        questionText.gameObject.SetActive(true);
        questionText.raycastTarget = false;
        BindButton(oButton, onO);
        BindButton(xButton, onX);
    }

    public void ShowResult(string result)
    {
        questionText.text = result;
        oButton.gameObject.SetActive(false);
        xButton.gameObject.SetActive(false);
    }

    public void HideQuiz()
    {
        if (questionText != null) questionText.gameObject.SetActive(false);
        HideButton(oButton);
        HideButton(xButton);
    }

    private static void BindButton(Button button, UnityAction action)
    {
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(action);
        button.interactable = true;
        button.gameObject.SetActive(true);
    }

    private static void HideButton(Button button)
    {
        if (button == null) return;
        button.onClick = new Button.ButtonClickedEvent();
        button.gameObject.SetActive(false);
    }
}

using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuizPatton : AbstractPatton
{
    [Tooltip("BossCanvas의 QuizPatternUI. 씬에 배치된 UI를 우선 사용합니다.")]
    [SerializeField] private QuizPatternUI _uiPrefab;
    [Tooltip("기존 Quiz2 캐릭터 프리팹")]
    [SerializeField] private GameObject _quizVisualPrefab;
    [SerializeField] private GameObject _beforBoss;
    [SerializeField] private PlayerHealth _player1Health;
    [SerializeField] private PlayerHealth _player2Health;
    [SerializeField] private BossHealthSystem _bossHealthSystem;

    public string[] questions;

    private QuizPatternUI ui;
    private GameObject visualInstance;
    private Transform quizVisual;
    private Tween visualTween;
    private Tween bossTween;
    private Sequence answerSequence;
    private bool isRunning;
    private bool hasAnswered;
    private bool hasBossStartPosition;
    private float bossStartX;

    // Start/OnEnable에서는 실행하지 않고 기존 BossPattonManger의 호출을 기다립니다.
    public override void Patton()
    {
        if (isRunning || !isActiveAndEnabled) return;
        isRunning = true;
        hasAnswered = false;
        ResolveBattleTargets();
        ui = FindFirstObjectByType<QuizPatternUI>();

        // 다른 전투 씬에서도 한 번만 생성하고 이후 패턴에서 같은 HUD를 사용합니다.
        if (ui == null && _uiPrefab != null)
            ui = Instantiate(_uiPrefab);

        if (ui == null || !ui.IsConfigured || questions == null ||
            questions.Length == 0 || _bossHealthSystem == null)
        {
            Debug.LogError("QuizPatton: BossCanvas의 QuizPatternUI, Questions, 씬의 BossHealthSystem을 확인해 주세요.", this);
            BossPattonManger.instance?.CompleteCurrentPattern(this);
            return;
        }

        ui.BindBoss(_bossHealthSystem);
        SetQuestion();

        if (_beforBoss != null)
        {
            bossStartX = _beforBoss.transform.localPosition.x;
            hasBossStartPosition = true;
            bossTween = _beforBoss.transform.DOLocalMoveX(12f, 0.5f);
        }

        if (_quizVisualPrefab != null)
        {
            visualInstance = Instantiate(_quizVisualPrefab);
            visualInstance.transform.SetParent(transform, true);
            // Quiz2의 패턴 스크립트는 실행하지 않고 외형과 Animator만 사용합니다.
            QuizPatton visualPattern = visualInstance.GetComponent<QuizPatton>();
            if (visualPattern != null) visualPattern.enabled = false;
            quizVisual = visualInstance.transform;
        }
        else if (GetComponentInChildren<SpriteRenderer>() != null)
        {
            quizVisual = transform;
        }

        // 기존 캐릭터 등장 연출을 유지하고 문제 텍스트의 배치는 움직이지 않습니다.
        if (quizVisual != null)
            visualTween = quizVisual.DOMoveY(1.2f, 0.5f);
    }

    private void ResolveBattleTargets()
    {
        if (_bossHealthSystem == null || !_bossHealthSystem.gameObject.scene.IsValid())
            _bossHealthSystem = FindFirstObjectByType<BossHealthSystem>();
        if (_beforBoss == null || !_beforBoss.scene.IsValid())
            _beforBoss = _bossHealthSystem != null ? _bossHealthSystem.gameObject : null;

        bool findPlayer1 = _player1Health == null || !_player1Health.gameObject.scene.IsValid();
        bool findPlayer2 = _player2Health == null || !_player2Health.gameObject.scene.IsValid();
        if (findPlayer1) _player1Health = null;
        if (findPlayer2) _player2Health = null;
        foreach (PlayerHealth player in FindObjectsByType<PlayerHealth>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (findPlayer1 && player.Tag == PlayerTag.Plusy) _player1Health = player;
            if (findPlayer2 && player.Tag == PlayerTag.Minusy) _player2Health = player;
        }
    }

    public void GoodAnswer() => Answer(true);
    public void BadAnswer() => Answer(false);

    private void Answer(bool correct)
    {
        if (!isRunning || hasAnswered || ui == null) return;
        hasAnswered = true;
        ui.ShowResult(correct ? "정답!!" : "오답!!!!!!!");
        visualTween?.Kill();
        bossTween?.Kill();

        answerSequence = DOTween.Sequence();
        answerSequence.AppendInterval(0.5f);
        if (quizVisual != null)
            answerSequence.Join(quizVisual.DOMoveY(-12f, 0.5f));
        if (_beforBoss != null && hasBossStartPosition)
            answerSequence.Join(_beforBoss.transform.DOLocalMoveX(bossStartX, 0.5f));
        answerSequence.AppendCallback(() => BossPattonManger.instance?.CompleteCurrentPattern(this));

        if (correct)
        {
            _bossHealthSystem.Damage(30);
        }
        else
        {
            DamagePlayer(_player1Health);
            if (_player2Health != _player1Health) DamagePlayer(_player2Health);
        }
    }

    private static void DamagePlayer(PlayerHealth player)
    {
        if (player != null && player.isActiveAndEnabled && player.Health > 0)
            player.Damage(10);
    }

    public IEnumerator Co()
    {
        yield return new WaitForSeconds(0.5f);
        SetQuestion();
    }

    public void SetQuestion()
    {
        if (!isRunning || hasAnswered || ui == null || questions == null || questions.Length == 0) return;
        ui.ShowQuestion(questions[Random.Range(0, questions.Length)], GoodAnswer, BadAnswer);
    }

    private void OnDisable()
    {
        isRunning = false;
        hasAnswered = true;
        StopAllCoroutines();
        visualTween?.Kill();
        bossTween?.Kill();
        answerSequence?.Kill();
        visualTween = null;
        bossTween = null;
        answerSequence = null;

        if (_beforBoss != null && hasBossStartPosition)
        {
            Vector3 position = _beforBoss.transform.localPosition;
            position.x = bossStartX;
            _beforBoss.transform.localPosition = position;
        }
        hasBossStartPosition = false;

        if (ui != null) ui.HideQuiz();
        ui = null;
        if (visualInstance != null)
        {
            visualInstance.SetActive(false);
            Destroy(visualInstance);
            visualInstance = null;
        }
        quizVisual = null;
    }
}

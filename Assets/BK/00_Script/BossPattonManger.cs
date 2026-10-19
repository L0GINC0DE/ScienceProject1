using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattonManger : MonoBehaviour
{
    public static BossPattonManger instance;

    public IngameOpening ingameOpening;

    public List<BossPatternSO> bosses = new();

    private Transform patternParent;

    public AbstractPatton ActivePattern { get; private set; }
    public int NextPatternIndex { get; private set; }

    private TurnManager turnManager;
    private Coroutine patternRoutine;
    private GameObject activePatternObject;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnEnable()
    {
        if (instance != this) return;

        turnManager = TurnManager.Instance;
        if (turnManager == null)
        {
            return;
        }

        turnManager.OnEnemyTurn += PlayNextPattern;
        turnManager.OnPlusyTurn += ClearCurrentPattern;
    }

    private void OnDisable()
    {
        if (turnManager != null)
        {
            turnManager.OnEnemyTurn -= PlayNextPattern;
            turnManager.OnPlusyTurn -= ClearCurrentPattern;
        }

        ClearCurrentPattern();
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private void PlayNextPattern()
    {
        ClearCurrentPattern();

        if (bosses.Count == 0)
        {
            patternRoutine = StartCoroutine(ReturnToPlayerTurn());
            return;
        }

        NextPatternIndex %= bosses.Count;
        BossPatternSO pattern = bosses[NextPatternIndex];
        NextPatternIndex = (NextPatternIndex + 1) % bosses.Count;

        if (pattern == null || pattern.patternPrefab == null ||
            !pattern.patternPrefab.gameObject.activeSelf || !pattern.patternPrefab.enabled)
        {
            patternRoutine = StartCoroutine(ReturnToPlayerTurn());
            return;
        }

        if (ingameOpening != null && pattern.areaSize.x > 0f && pattern.areaSize.y > 0f)
            ingameOpening.SetIngameSize(pattern.areaSize);

        ActivePattern = Instantiate(pattern.patternPrefab, patternParent);
        activePatternObject = ActivePattern.gameObject;
        patternRoutine = StartCoroutine(RunPattern(ActivePattern, pattern.duration));
    }

    private IEnumerator RunPattern(AbstractPatton pattern, float duration)
    {
        yield return null;

        if (pattern != null) pattern.Patton();

        if (turnManager.CurrentTurnState != TurnState.EnemyTurn ||
            !ReferenceEquals(pattern, ActivePattern)) yield break;

        if (duration <= 0f)
        {
            patternRoutine = null;
            yield break;
        }

        yield return new WaitForSeconds(duration);
        patternRoutine = null;
        CompleteCurrentPattern(pattern);
    }

    public void CompleteCurrentPattern(AbstractPatton pattern)
    {
        if (ReferenceEquals(pattern, null) || !ReferenceEquals(pattern, ActivePattern) || turnManager == null) return;
        if (turnManager.CurrentTurnState != TurnState.EnemyTurn) return;

        ClearCurrentPattern();
        turnManager.CompleteEnemyTurn();
    }

    private IEnumerator ReturnToPlayerTurn()
    {
        yield return null;
        patternRoutine = null;
        if (turnManager != null) turnManager.CompleteEnemyTurn();
    }

    private void ClearCurrentPattern()
    {
        if (patternRoutine != null)
        {
            StopCoroutine(patternRoutine);
            patternRoutine = null;
        }

        ActivePattern = null;
        if (activePatternObject != null)
        {
            activePatternObject.SetActive(false);
            Destroy(activePatternObject);
        }

        activePatternObject = null;
    }
}

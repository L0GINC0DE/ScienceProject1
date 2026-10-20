using UnityEngine;

public class SoulHitFeedback : MonoBehaviour
{
    [SerializeField, Min(0.02f)] private float blinkDuration = 0.5f;
    [SerializeField, Min(0.02f)] private float blinkInterval = 0.15f;

    private SpriteRenderer[] renderers;
    private bool[] originalRenderingStates;
    private Rigidbody2D soulBody;
    private float remainingTime;
    private float toggleTimer;
    private bool isBlinking;
    private bool isHidden;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalRenderingStates = new bool[renderers.Length];
        soulBody = GetComponent<Rigidbody2D>();
    }

    public static void PlayOn(PlayerMovement soul)
    {
        if (soul == null || !soul.isActiveAndEnabled) return;

        SoulHitFeedback feedback = soul.GetComponent<SoulHitFeedback>();
        if (feedback == null) feedback = soul.gameObject.AddComponent<SoulHitFeedback>();
        if (!feedback.isActiveAndEnabled || (feedback.soulBody != null && !feedback.soulBody.simulated)) return;

        if (!feedback.isBlinking)
            for (int i = 0; i < feedback.renderers.Length; i++)
                if (feedback.renderers[i] != null)
                    feedback.originalRenderingStates[i] = feedback.renderers[i].forceRenderingOff;

        feedback.isBlinking = true;
        feedback.remainingTime = Mathf.Max(0.02f, feedback.blinkDuration);
        feedback.toggleTimer = 0f;
        feedback.SetHidden(true);
    }

    private void LateUpdate()
    {
        if (!isBlinking) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f || (soulBody != null && !soulBody.simulated))
        {
            StopBlinking();
            return;
        }

        toggleTimer += Time.deltaTime;
        if (toggleTimer >= Mathf.Max(0.02f, blinkInterval))
        {
            toggleTimer %= Mathf.Max(0.02f, blinkInterval);
            SetHidden(!isHidden);
        }
    }

    private void SetHidden(bool hidden)
    {
        isHidden = hidden;
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null)
                renderers[i].forceRenderingOff = hidden || originalRenderingStates[i];
    }

    private void StopBlinking()
    {
        if (!isBlinking) return;
        SetHidden(false);
        isBlinking = false;
    }

    private void OnDisable()
    {
        StopBlinking();
    }
}

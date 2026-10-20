using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : AbstractPatton
{
    [Header("Prefabs")]
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject cloudPrefab;

    [Header("Placement")]
    [SerializeField] private Vector3 cloudPosition = new Vector3(0f, 2f, 0f);
    [SerializeField] private float lightningY = 0.76f;
    [SerializeField, Min(0.1f)] private float fallbackCloudWidth = 4f;
    [SerializeField, Min(0f)] private float edgePadding = 0.1f;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float startDelay = 1f;
    [SerializeField, Min(0f)] private float warningDuration = 0.6f;
    [SerializeField, Min(0.02f)] private float strikeDuration = 0.2f;
    [SerializeField, Min(0.02f)] private float strikeInterval = 0.4f;

    [Header("Warning")]
    [SerializeField] private Color warningColor = new Color(1f, 0.5f, 0.15f, 0.25f);
    [SerializeField] private Color chargedCloudColor = new Color(0.45f, 0.5f, 0.65f, 1f);

    [Header("Damage")]
    [SerializeField, Min(0)] private int damage = 10;
    [SerializeField]
    private LayerMask hitLayers = ~0;

    private Coroutine patternRoutine;
    private GameObject cloudInstance;
    private readonly GameObject[] lightningInstances = new GameObject[2];
    private SpriteRenderer[] cloudRenderers;
    private SpriteRenderer[] lightningRenderers;
    private Color[] cloudColors;
    private Color[] lightningColors;
    private Collider2D[] lightningColliders;
    private bool[] colliderEnabledStates;
    private float lightningLeftOffset;
    private float lightningRightOffset;
    private PlayerHealth[] healthTargets;
    private readonly List<Collider2D> hitResults = new List<Collider2D>();
    private ContactFilter2D hitFilter;
    private bool isStriking;
    private bool hasHitThisStrike;

    public override void Patton()
    {
        if (patternRoutine != null) return;

        if (lightningPrefab == null || cloudPrefab == null)
        {
            BossPattonManger.instance?.CompleteCurrentPattern(this);
            return;
        }

        healthTargets = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        hitFilter = new ContactFilter2D { useTriggers = true };
        hitFilter.SetLayerMask(hitLayers);

        cloudInstance = Instantiate(cloudPrefab, cloudPosition, cloudPrefab.transform.rotation, transform);
        cloudRenderers = cloudInstance.GetComponentsInChildren<SpriteRenderer>();
        cloudColors = CaptureColors(cloudRenderers);

        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        List<Collider2D> colliders = new List<Collider2D>();
        for (int i = 0; i < lightningInstances.Length; i++)
        {
            lightningInstances[i] = Instantiate(lightningPrefab,
                new Vector3(cloudPosition.x, lightningY, cloudPosition.z), lightningPrefab.transform.rotation, transform);
            SpriteRenderer[] boltRenderers = lightningInstances[i].GetComponentsInChildren<SpriteRenderer>();
            renderers.AddRange(boltRenderers);
            colliders.AddRange(lightningInstances[i].GetComponentsInChildren<Collider2D>());

            if (i == 0 && TryGetBounds(boltRenderers, out Bounds bounds))
            {
                lightningLeftOffset = bounds.min.x - lightningInstances[i].transform.position.x;
                lightningRightOffset = bounds.max.x - lightningInstances[i].transform.position.x;
            }
        }

        lightningRenderers = renderers.ToArray();
        lightningColors = CaptureColors(lightningRenderers);
        lightningColliders = colliders.ToArray();
        colliderEnabledStates = new bool[lightningColliders.Length];
        for (int i = 0; i < lightningColliders.Length; i++)
            colliderEnabledStates[i] = lightningColliders[i].enabled;

        SetCollidersEnabled(false);
        foreach (GameObject lightning in lightningInstances) lightning.SetActive(false);
        patternRoutine = StartCoroutine(SpawnLightning());
    }

    private IEnumerator SpawnLightning()
    {
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        while (cloudInstance != null && lightningInstances[0] != null && lightningInstances[1] != null)
        {
            if (!TryGetBounds(cloudRenderers, out Bounds bounds))
                bounds = new Bounds(cloudInstance.transform.position, new Vector3(fallbackCloudWidth, 1f, 0f));

            float minX = bounds.min.x + edgePadding - lightningLeftOffset;
            float maxX = bounds.max.x - edgePadding - lightningRightOffset;
            float leftX;
            float rightX;
            if (minX >= maxX)
            {
                leftX = bounds.center.x - (lightningLeftOffset + lightningRightOffset) * 0.5f;
                rightX = leftX;
            }
            else
            {
                float separation = Mathf.Min(maxX - minX,
                    lightningRightOffset - lightningLeftOffset + edgePadding);
                leftX = Random.Range(minX, maxX - separation);
                rightX = Random.Range(leftX + separation, maxX);
            }
            lightningInstances[0].transform.position = new Vector3(leftX, lightningY, cloudPosition.z);
            lightningInstances[1].transform.position = new Vector3(rightX, lightningY, cloudPosition.z);
            ApplyWarningColors(0f);
            foreach (GameObject lightning in lightningInstances) lightning.SetActive(true);

            float elapsed = 0f;
            while (elapsed < warningDuration)
            {
                ApplyWarningColors(elapsed / warningDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            RestoreColors(cloudRenderers, cloudColors);
            RestoreColors(lightningRenderers, lightningColors);
            hasHitThisStrike = false;
            SetCollidersEnabled(true);
            Physics2D.SyncTransforms();
            CheckForHit();
            yield return new WaitForSeconds(Mathf.Max(0.02f, strikeDuration));

            SetCollidersEnabled(false);
            foreach (GameObject lightning in lightningInstances) lightning.SetActive(false);
            yield return new WaitForSeconds(Mathf.Max(0.02f, strikeInterval));
        }

        patternRoutine = null;
    }

    private void FixedUpdate()
    {
        CheckForHit();
    }

    private void CheckForHit()
    {
        if (!isStriking || hasHitThisStrike || damage <= 0) return;

        foreach (Collider2D lightningCollider in lightningColliders)
        {
            if (lightningCollider == null || !lightningCollider.isActiveAndEnabled) continue;

            hitResults.Clear();
            lightningCollider.Overlap(hitFilter, hitResults);
            foreach (Collider2D hit in hitResults)
            {
                PlayerMovement soul = hit.GetComponentInParent<PlayerMovement>();
                if (soul == null || !soul.isActiveAndEnabled) continue;

                PlayerHealth target = null;
                int candidateCount = 0;
                foreach (PlayerHealth candidate in healthTargets)
                {
                    if (candidate == null || !candidate.isActiveAndEnabled || candidate.Health <= 0) continue;

                    candidateCount++;
                    if (Random.Range(0, candidateCount) == 0) target = candidate;
                }
                if (target == null) return;

                hasHitThisStrike = true;
                target.Damage(damage);
                SoulHitFeedback.PlayOn(soul);
                return;
            }
        }
    }

    private void ApplyWarningColors(float progress)
    {
        for (int i = 0; i < lightningRenderers.Length; i++)
        {
            Color color = warningColor;
            color.a *= lightningColors[i].a * Mathf.Lerp(0.4f, 1f, progress);
            lightningRenderers[i].color = color;
        }

        for (int i = 0; i < cloudRenderers.Length; i++)
            cloudRenderers[i].color = Color.Lerp(cloudColors[i], cloudColors[i] * chargedCloudColor, progress);
    }

    private void SetCollidersEnabled(bool enabled)
    {
        isStriking = enabled;
        for (int i = 0; i < lightningColliders.Length; i++)
            lightningColliders[i].enabled = enabled && colliderEnabledStates[i];
    }

    private static Color[] CaptureColors(SpriteRenderer[] renderers)
    {
        Color[] colors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) colors[i] = renderers[i].color;
        return colors;
    }

    private static void RestoreColors(SpriteRenderer[] renderers, Color[] colors)
    {
        for (int i = 0; i < renderers.Length; i++) renderers[i].color = colors[i];
    }

    private static bool TryGetBounds(SpriteRenderer[] renderers, out Bounds bounds)
    {
        bounds = default;
        bool hasBounds = false;
        foreach (SpriteRenderer renderer in renderers)
        {
            if (!renderer.enabled || renderer.sprite == null) continue;
            if (hasBounds) bounds.Encapsulate(renderer.bounds);
            else bounds = renderer.bounds;
            hasBounds = true;
        }

        return hasBounds;
    }

    private void OnDisable()
    {
        isStriking = false;
        hitResults.Clear();
        if (patternRoutine != null) StopCoroutine(patternRoutine);
        patternRoutine = null;

        if (cloudInstance != null)
        {
            cloudInstance.SetActive(false);
            Destroy(cloudInstance);
        }

        for (int i = 0; i < lightningInstances.Length; i++)
        {
            if (lightningInstances[i] == null) continue;
            lightningInstances[i].SetActive(false);
            Destroy(lightningInstances[i]);
            lightningInstances[i] = null;
        }

        cloudInstance = null;
    }
}

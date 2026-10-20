using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ElectricField : AbstractPatton
{
    [Header("Prefabs")]
    [SerializeField] private GameObject plusPrefab;
    [SerializeField] private GameObject minusPrefab;
    [SerializeField] private GameObject electronPrefab;

    [Header("Placement")]
    [SerializeField] private Transform ingameArea;
    [SerializeField] private Vector2 fallbackAreaCenter = new Vector2(0f, 1.11f);
    [SerializeField] private Vector2 fallbackAreaSize = new Vector2(5.2f, 5.2f);
    [SerializeField, Min(0f)] private float bottomInset = 0.05f;
    [SerializeField, Min(0f)] private float spawnHeight = 0.2f;
    [SerializeField, Min(0f)] private float edgePadding = 0.1f;
    [SerializeField, Min(0f)] private float despawnMargin = 0.5f;
    [SerializeField] private string sortingLayerName = "Pattern";

    [Header("Timing")]
    [SerializeField, Min(0f)] private float startDelay = 1f;
    [SerializeField, Min(0.02f)] private float polarityDuration = 2f;
    [SerializeField, Min(0.02f)] private float electronInterval = 0.45f;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float fieldSpeed = 1.25f;
    [SerializeField, Min(0.01f)] private float electronSpeed = 3f;

    [Header("Damage")]
    [SerializeField, Min(0)] private int damage = 10;
    [SerializeField, Min(0.02f)] private float hitCooldown = 0.5f;
    [SerializeField] private LayerMask hitLayers = ~0;

    private sealed class Hazard
    {
        public GameObject Instance;
        public Collider2D[] Colliders;
        public SpriteRenderer[] Renderers;
    }

    private readonly List<Hazard> electrons = new List<Hazard>();
    private readonly List<Collider2D> overlaps = new List<Collider2D>();
    private readonly List<RaycastHit2D> sweepHits = new List<RaycastHit2D>();
    private SpriteRenderer[] areaRenderers;
    private PlayerMovement[] souls;
    private PlayerHealth[] healthTargets;
    private ContactFilter2D hitFilter;
    private Coroutine patternRoutine;
    private Hazard plus;
    private Hazard minus;
    private bool isPlus;
    private bool isRunning;
    private float nextDamageTime;

    public override void Patton()
    {
        if (patternRoutine != null || isRunning) return;
        if (plusPrefab == null || minusPrefab == null || electronPrefab == null)
        {
            BossPattonManger.instance?.CompleteCurrentPattern(this);
            return;
        }

        if (ingameArea == null && BossPattonManger.instance?.ingameOpening != null)
            ingameArea = BossPattonManger.instance.ingameOpening.IngameTransform;

        if (ingameArea != null) areaRenderers = ingameArea.GetComponentsInChildren<SpriteRenderer>();
        souls = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        healthTargets = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        hitFilter = new ContactFilter2D { useTriggers = true };
        hitFilter.SetLayerMask(hitLayers);
        nextDamageTime = 0f;
        patternRoutine = StartCoroutine(RunPattern());
    }

    private IEnumerator RunPattern()
    {
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        plus = CreateHazard(plusPrefab);
        minus = CreateHazard(minusPrefab);
        isPlus = true;
        minus.Instance.SetActive(false);
        PositionPole(plus, GetAreaBounds());
        isRunning = true;

        float polarityTimer = 0f;
        float electronTimer = 0f;
        while (isRunning)
        {
            if (polarityTimer >= Mathf.Max(0.02f, polarityDuration))
            {
                polarityTimer = 0f;
                isPlus = !isPlus;
                plus.Instance.SetActive(isPlus);
                minus.Instance.SetActive(!isPlus);
                PositionPole(isPlus ? plus : minus, GetAreaBounds());
            }

            if (electronTimer <= 0f)
            {
                Hazard electron = CreateHazard(electronPrefab);
                Bounds area = GetAreaBounds();
                TryGetBounds(electron.Renderers, out Bounds bounds);
                float minX = area.min.x + edgePadding + bounds.extents.x;
                float maxX = area.max.x - edgePadding - bounds.extents.x;
                float x = minX < maxX ? Random.Range(minX, maxX) : area.center.x;
                Vector3 target = new Vector3(x, area.max.y + spawnHeight + bounds.extents.y,
                    electron.Instance.transform.position.z);
                electron.Instance.transform.position += target - bounds.center;
                electrons.Add(electron);
                electronTimer = Mathf.Max(0.02f, electronInterval);
            }

            yield return null;
            polarityTimer += Time.deltaTime;
            electronTimer -= Time.deltaTime;
        }
    }

    private Hazard CreateHazard(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, transform);
        instance.SetActive(true);
        SpriteRenderer[] renderers = instance.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers) 
            renderer.sortingLayerName = sortingLayerName;

        Collider2D[] colliders = instance.GetComponentsInChildren<Collider2D>();
        if (colliders.Length == 0)
        {
            BoxCollider2D box = instance.AddComponent<BoxCollider2D>();
            SpriteRenderer rootRenderer = instance.GetComponent<SpriteRenderer>();
            if (rootRenderer != null && rootRenderer.sprite != null)
            {
                box.offset = rootRenderer.sprite.bounds.center;
                box.size = rootRenderer.sprite.bounds.size;
            }
            colliders = new Collider2D[] { box };
        }

        foreach (Collider2D collider in colliders) 
            collider.isTrigger = true;
        return new Hazard { Instance = instance, Colliders = colliders, Renderers = renderers };
    }

    private Bounds GetAreaBounds()
    {
        if (TryGetBounds(areaRenderers, out Bounds bounds) && bounds.size.x > 0.01f && bounds.size.y > 0.01f)
            return bounds;

        return new Bounds((Vector3)fallbackAreaCenter,
            new Vector3(Mathf.Max(0.1f, fallbackAreaSize.x), Mathf.Max(0.1f, fallbackAreaSize.y), 0f));
    }

    private void PositionPole(Hazard pole, Bounds area)
    {
        if (!TryGetBounds(pole.Renderers, out Bounds bounds)) return;
        Vector3 scale = pole.Instance.transform.localScale;
        scale.x *= area.size.x / Mathf.Max(0.001f, bounds.size.x);
        pole.Instance.transform.localScale = scale;
        TryGetBounds(pole.Renderers, out bounds);
        Vector3 target = new Vector3(area.center.x, area.min.y + bottomInset + bounds.extents.y,
            pole.Instance.transform.position.z);
        pole.Instance.transform.position += target - bounds.center;
    }

    private void FixedUpdate()
    {
        if (!isRunning) return;

        Bounds area = GetAreaBounds();
        Hazard pole = isPlus ? plus : minus;
        PositionPole(pole, area);
        foreach (PlayerMovement soul in souls)
        {
            if (soul == null) continue;
            bool canMove = soul.isActiveAndEnabled && soul._rb != null && soul._rb.simulated;
            soul.ExternalVelocity = canMove ? Vector2.up * (isPlus ? fieldSpeed : -fieldSpeed) : Vector2.zero;
        }

        Physics2D.SyncTransforms();
        CheckOverlap(pole);
        if (!isRunning) return;
        float distance = Mathf.Max(0.01f, electronSpeed) * Time.fixedDeltaTime;
        for (int i = electrons.Count - 1; i >= 0; i--)
        {
            Hazard electron = electrons[i];
            bool hit = CheckOverlap(electron);
            if (!hit)
            {
                foreach (Collider2D collider in electron.Colliders)
                {
                    if (!collider.isActiveAndEnabled) continue;
                    Bounds bounds = collider.bounds;
                    sweepHits.Clear();
                    Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, hitFilter, sweepHits, distance);
                    foreach (RaycastHit2D sweepHit in sweepHits)
                    {
                        if (!TryHitSoul(sweepHit.collider)) continue;
                        hit = true;
                        break;
                    }
                    if (hit) break;
                }
            }
            if (!isRunning) return;
            electron.Instance.transform.position += Vector3.down * distance;
            if (hit || electron.Instance.transform.position.y < area.min.y - despawnMargin)
            {
                RemoveHazard(electron);
                electrons.RemoveAt(i);
            }
        }
    }

    private bool CheckOverlap(Hazard hazard)
    {
        foreach (Collider2D collider in hazard.Colliders)
        {
            if (!collider.isActiveAndEnabled) continue;
            overlaps.Clear();
            collider.Overlap(hitFilter, overlaps);
            foreach (Collider2D other in overlaps)
                if (TryHitSoul(other)) return true;
        }
        return false;
    }

    private bool TryHitSoul(Collider2D other)
    {
        PlayerMovement soul = other.GetComponentInParent<PlayerMovement>();
        if (soul == null || !soul.isActiveAndEnabled || soul._rb == null || !soul._rb.simulated) return false;
        if (damage <= 0 || Time.time < nextDamageTime) return true;

        PlayerHealth target = null;
        int count = 0;
        foreach (PlayerHealth candidate in healthTargets)
        {
            if (candidate == null || !candidate.isActiveAndEnabled || candidate.Health <= 0) continue;
            if (Random.Range(0, ++count) == 0) target = candidate;
        }

        if (target != null)
        {
            nextDamageTime = Time.time + Mathf.Max(0.02f, hitCooldown);
            target.Damage(damage);
            SoulHitFeedback.PlayOn(soul);
        }
        return true;
    }

    private static bool TryGetBounds(SpriteRenderer[] renderers, out Bounds bounds)
    {
        bounds = default;
        bool found = false;
        if (renderers == null) return false;
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer == null || !renderer.enabled || renderer.sprite == null) continue;
            if (found) bounds.Encapsulate(renderer.bounds);
            else bounds = renderer.bounds;
            found = true;
        }
        return found;
    }

    private void RemoveHazard(Hazard hazard)
    {
        if (hazard?.Instance == null) return;
        hazard.Instance.SetActive(false);
        Destroy(hazard.Instance);
    }

    private void OnDisable()
    {
        isRunning = false;
        if (patternRoutine != null) StopCoroutine(patternRoutine);
        patternRoutine = null;
        if (souls != null)
            foreach (PlayerMovement soul in souls)
                if (soul != null) soul.ExternalVelocity = Vector2.zero;

        RemoveHazard(plus);
        RemoveHazard(minus);
        plus = null;
        minus = null;
        foreach (Hazard electron in electrons) RemoveHazard(electron);
        electrons.Clear();
        overlaps.Clear();
        sweepHits.Clear();
    }
}

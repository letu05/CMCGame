using UnityEngine;

// BulletType enum nằm trong BulletType.cs (cùng thư mục)

/// <summary>
/// Đặc tính đạn được SET SẴN trong từng prefab qua Inspector.
/// PlayerFire chỉ gọi Init() sau khi spawn – không override bulletType.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    // ────────────────────────────── Inspector ──────────────────────────────
    [Header("Loại đạn (set trong prefab, không đổi runtime)")]
    public BulletType bulletType = BulletType.Dart;

    [Header("Chung")]
    [SerializeField] private float speed    = 12f;
    [SerializeField] private float lifetime = 4f;       // Tự huỷ sau n giây
    [SerializeField] private int   damage   = 1;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask playerLayer;

    [Header("Bomb – 1/4 Hình Tròn")]
    [SerializeField] private float arcRadius        = 3f;
    [SerializeField] private float bombDetectRadius = 2.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Boomerang – Quay lại")]
    [SerializeField] private float rotateSpeed = 360f;     [SerializeField] private float returnDelay  = 0.35f;    

    // ────────────────────────────── Runtime ────────────────────────────────
    private Rigidbody2D rb;
    [SerializeField]private Transform   owner;          

    // Bomb
    private bool      isBombArcing;
    private bool      isBombHoming;     // Đang bay về phía quái
    private Transform bombTargetEnemy;  // Quái bị lock
    private Vector2   bombCenter;
    private float     arcRadius_rt;
    private float     arcDirX;
    private float     arcTheta;

    // Boomerang
    private bool  isReturning;
    private float returnTimer;

   
    public void Init(Vector2 direction, Transform ownerTransform)
    {
        rb              = GetComponent<Rigidbody2D>();
        owner           = ownerTransform;
        rb.gravityScale = 0f;

        switch (bulletType)
        {
            case BulletType.Dart:      InitDart(direction);      break;
            case BulletType.Bomb:      InitBomb(direction);      break;
            case BulletType.Boomerang: InitBoomerang(direction); break;
        }

        Destroy(gameObject, lifetime);
    }

    // ───────────────────────────── Update ──────────────────────────────────
    private void Update()
    {
        switch (bulletType)
        {
            case BulletType.Bomb:      UpdateBomb();      break;
            case BulletType.Boomerang: UpdateBoomerang(); break;
            // Dart: velocity set 1 lần, physics tự lo
        }
    }

    // ═══════════════════════════════ DART ══════════════════════════════════
    private void InitDart(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    // ════════════════════════════════ BOMB ═════════════════════════════════
    private void InitBomb(Vector2 direction)
    {
        isBombArcing    = true;
        isBombHoming    = false;
        bombTargetEnemy = null;
        arcRadius_rt    = arcRadius;
        arcDirX         = direction.x >= 0f ? 1f : -1f;
        bombCenter      = (Vector2)transform.position + new Vector2(0f, -arcRadius_rt);
        arcTheta        = Mathf.PI * 0.5f;
    }

    private void UpdateBomb()
    {
        // ── Chế độ homing: bay thẳng tới quái ──────────────────────────────
        if (isBombHoming)
        {
            if (bombTargetEnemy == null) { Destroy(gameObject); return; }

            Vector2 toEnemy   = ((Vector2)bombTargetEnemy.position
                                 - (Vector2)transform.position).normalized;
            rb.linearVelocity = toEnemy * speed;
            transform.rotation = Quaternion.Euler(0f, 0f,
                Mathf.Atan2(toEnemy.y, toEnemy.x) * Mathf.Rad2Deg);
            return;
        }

        // ── Chế độ arc: bay 1/4 vòng tròn xuống ───────────────────────────
        if (!isBombArcing) return;

        // Check quái trong bán kính → chuyển homing
        Collider2D hit = Physics2D.OverlapCircle(transform.position, bombDetectRadius, enemyLayer);
        if (hit != null)
        {
            isBombArcing    = false;
            isBombHoming    = true;
            bombTargetEnemy = hit.transform;
            return;
        }

        float angularSpeed = speed / arcRadius_rt;
        arcTheta -= angularSpeed * Time.deltaTime;

        if (arcTheta <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        float px = bombCenter.x + arcDirX * arcRadius_rt * Mathf.Cos(arcTheta);
        float py = bombCenter.y + arcRadius_rt * Mathf.Sin(arcTheta);
        transform.position = new Vector3(px, py, 0f);

        float tx = arcDirX * arcRadius_rt * Mathf.Sin(arcTheta);
        float ty = -arcRadius_rt * Mathf.Cos(arcTheta);
        transform.rotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(ty, tx) * Mathf.Rad2Deg);
    }

    // ══════════════════════════════ BOOMERANG ══════════════════════════════
    private void InitBoomerang(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        isReturning       = false;
        returnTimer       = 0f;
    }

    private void UpdateBoomerang()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime); // Xoay sprite

        if (!isReturning)
        {
            returnTimer += Time.deltaTime;
            if (returnTimer >= returnDelay) isReturning = true;
        }
        else
        {
            if (owner == null) { Destroy(gameObject); return; }

            Vector2 toOwner   = ((Vector2)owner.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = toOwner * speed;

            if (Vector2.Distance(transform.position, owner.position) < 0.4f)
                Destroy(gameObject);
        }
    }

    // ══════════════════════════════ HIT ════════════════════════════════════
    private void OnTriggerEnter2D(Collider2D other)
    {
        int layer = 1 << other.gameObject.layer;

        // ── Boomerang về đến owner → chỉ destroy, KHÔNG gây damage ──────────
        if (bulletType == BulletType.Boomerang && owner != null
            && other.transform == owner)
        {
            Destroy(gameObject);
            return;
        }

        // ── Chạm đất → mất ngay (chỉ Bomb mới set groundLayer) ──────────────
        if ((layer & groundLayer) != 0)
        {
            Destroy(gameObject);
            return;
        }

        // ── Chạm quái hoặc player → gây damage qua IcanTakeDamage ───────────
        bool hitEnemy  = (layer & enemyLayer)  != 0;
        bool hitPlayer = (layer & playerLayer) != 0;

        if (hitEnemy || hitPlayer)
        {
            string who = hitEnemy ? "Quái" : "Player";
            IcanTakeDamage target = other.GetComponent<IcanTakeDamage>();
            if (target != null) target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    // ─── Gizmo ─────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (bulletType == BulletType.Bomb)
        {
            Gizmos.color = new Color(1f, 0.3f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, bombDetectRadius);
        }
    }
}

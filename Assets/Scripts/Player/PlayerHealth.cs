using UnityEngine;

public class PlayerHealth : MonoBehaviour, IcanTakeDamage
{
    private Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Gọi từ PlayerPowerUp mỗi khi đổi model (big/small)
    public void SetAnimator(Animator newAnimator)
    {
        anim = newAnimator;
    }

    // Được gọi từ PlayerPowerUp khi player trạng thái thường bị enemy tấn công
    public void TakeDamage(int damageAmount)
    {
        Die();
    }

    public void Die()
    {
        anim?.SetTrigger("Die");

        // Đóng băng player trong lúc animation die chạy
        PlayerController ctrl = GetComponentInParent<PlayerController>();
        if (ctrl != null) ctrl.SetDead();

        GameManager.Instance?.PlayerDied();
    }
}

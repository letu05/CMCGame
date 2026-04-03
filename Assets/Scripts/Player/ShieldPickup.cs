using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    [SerializeField] private int TimeToLive = 10; // Thời gian tồn tại (giây)

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= TimeToLive)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerPowerUp playerPowerUp = collision.gameObject.GetComponent<PlayerPowerUp>();
        if (playerPowerUp != null)
        {
            playerPowerUp.ActivateShield();
            Debug.Log("[ShieldPickup] Player nhặt shield!");
        }
        else
        {
            Debug.LogWarning("[ShieldPickup] Không tìm thấy PlayerPowerUp trên player!");
        }

        Destroy(gameObject);
    }
}

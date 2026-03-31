using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{

    [SerializeField]
    private float jumpForceAdditional = 5f; // Lực nhảy thêm khi bự
    [SerializeField]
    private GameObject PlayerSmall;
    [SerializeField]
    private GameObject PlayerBig;
    private PlayerController playerController;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        // Nếu Find không ra thì lấy từ inspector
        if (PlayerSmall == null) PlayerSmall = transform.Find("PlayerSmall").gameObject;
        if (PlayerBig == null) PlayerBig = transform.Find("PlayerBig").gameObject;

        // Bắt đầu game với hình dạng nhỏ
        SetPlayerState(isBig: false);
    }

    // Biến trạng thái
    public bool isPowerUpBig { get; private set; } = false;

    // Hàm gọi khi ăn nấm
    public void GrowBig()
    {
        if (!isPowerUpBig)
        {
            playerController.JumpForce += jumpForceAdditional; // Tăng lực nhảy khi to lên
            SetPlayerState(isBig: true);
            // Thêm hạt (Particle) hoặc Animation khựng lại/đứng hình giống Mario ở đây
        }
    }

    // Hàm gọi khi bị quái chạm vào lúc đang bự
    public void ShrinkSmall()
    {
        if (isPowerUpBig)
        {
            playerController.JumpForce -= jumpForceAdditional; // Trừ đi lực nhảy khi bé lại
            SetPlayerState(isBig: false);
            // Thêm hiệu ứng nhấp nháy bất tử tạm thời (Invulnerability frames) ở đây
        }
    }

    // Xử lý bật/tắt đúng model nhân vật
    private void SetPlayerState(bool isBig)
    {
        isPowerUpBig = isBig;
        if (PlayerSmall != null) PlayerSmall.SetActive(!isBig);
        if (PlayerBig != null) PlayerBig.SetActive(isBig);

        // Sau khi đổi model, cập nhật lại Animator cho PlayerController
        // vì Animator nằm trong child object nên phải lấy lại từ model đang active
        GameObject activeModel = isBig ? PlayerBig : PlayerSmall;
        if (activeModel != null)
        {
            Animator newAnimator = activeModel.GetComponentInChildren<Animator>();
            playerController.SetAnimator(newAnimator);
        }

        // Bạn có thể cần phải thay đổi offset/size của BoxCollider2D bên PlayerController tương ứng với model.
    }

    // Xử lý va chạm vật lý
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi ăn nấm
        if (collision.CompareTag("PointItem"))
        {
            GrowBig();
            Destroy(collision.gameObject); // Biến mất nấm
        }
        
        // Khi chạm địch
        else if (collision.CompareTag("Enemy"))
        {
            if (isPowerUpBig)
            {
                ShrinkSmall(); // Teo nhỏ thay vì chết
                // Cần 1 đoạn code hất văng enemy hoặc hất ngược nhân vật (Knockback) tại đây
            }
            else
            {
                // Gọi chết (PlayerController.Die())
                Debug.Log("Mario Died!");
            }
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PausePanel hoàn toàn độc lập với event system.
/// Dùng Update() polling IsPaused thay vì đăng ký/huỷ listener qua event —
/// cách này không bị ảnh hưởng bởi thứ tự Awake/Start/OnDestroy khi reload scene.
/// </summary>
public class PausePanel : MonoBehaviour
{
    [Header("Panel gốc cần ẩn/hiện")]
    [SerializeField] private GameObject pausePanelRoot;

    private bool lastPauseState = false;

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    private void Awake()
    {
        SetPanelVisible(false);
        lastPauseState = false;
    }

    /// <summary>
    /// Poll IsPaused mỗi frame — đơn giản, luôn đúng dù reload scene bao nhiêu lần.
    /// </summary>
    private void Update()
    {
        bool paused = PauseManager.Instance != null && PauseManager.Instance.IsPaused;

        // Chỉ cập nhật khi state thay đổi để tránh gọi SetActive mỗi frame
        if (paused != lastPauseState)
        {
            lastPauseState = paused;
            SetPanelVisible(paused);
        }
    }

    // ─── Button Callbacks (gắn vào OnClick trong Inspector) ──────────────────

    /// <summary>Nút TIẾP TỤC / X — resume game.</summary>
    public void OnResumeClicked()
    {
        PauseManager.Instance?.Resume();
    }

    /// <summary>Nút CHƠI LẠI — resume rồi restart scene hiện tại.</summary>
    public void OnPlayAgainClicked()
    {
        PauseManager.Instance?.Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>Nút VỀ MENU — resume rồi load scene 0 (Main Menu).</summary>
    public void OnMainMenuClicked()
    {
        PauseManager.Instance?.Resume();
        SceneManager.LoadScene(1); // GameOptionLevel
    }

    // ─── Hiển thị / Ẩn panel ─────────────────────────────────────────────────

    private void SetPanelVisible(bool visible)
    {
        if (pausePanelRoot != null)
            pausePanelRoot.SetActive(visible);
    }
}

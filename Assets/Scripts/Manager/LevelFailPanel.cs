using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Gắn vào GameObject Defeat (LevelFailPanel).
/// LevelManager.LevelFail() sẽ SetActive(true) panel này.
/// Nút trong Inspector:
///   - Nút Chơi Lại → OnPlayAgainClicked()
///   - Nút Về Menu  → OnMainMenuClicked()
/// </summary>
public class LevelFailPanel : MonoBehaviour
{
    [Header("Shop Panel (kéo ShopPanel vào)")]
    [SerializeField] private GameObject shopPanel;

    public void OnPlayAgainClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1); // GameOptionLevel
    }

    /// <summary>Nút 🛒 — mở Shop. Nút X trong Shop tự đóng bằng ShopPanel.CloseShop().</summary>
    public void OnShopClicked()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);
    }
}


using UnityEngine;
using TMPro;

/// <summary>
/// Panel Achievement chính.
/// Mở/Đóng bằng nút từ Menu.
/// Tự build danh sách achievement từ mảng AchievementData.
/// </summary>
public class AchievementPanel : MonoBehaviour
{
    [Header("Danh sách Achievement (kéo ScriptableObject vào)")]
    [SerializeField] private AchievementData[] achievements;

    [Header("Prefab 1 dòng achievement")]
    [SerializeField] private AchievementItemUI itemPrefab;

    [Header("Container chứa các dòng (Vertical Layout Group)")]
    [SerializeField] private Transform itemContainer;

    [Header("Hiển thị coin hiện tại")]
    [SerializeField] private TMP_Text coinText;

    // ─── Open/Close ──────────────────────────────────────────────────
    /// <summary>Mở panel. Gọi từ nút Achievement trên Menu.</summary>
    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }

    /// <summary>Đóng panel. Gọi từ nút Close trên panel.</summary>
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // ─── Build UI ────────────────────────────────────────────────────
    private void OnEnable()
    {
        BuildList();
        RefreshCoinUI();
    }

    private void BuildList()
    {
        // Xoá dòng cũ
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        // Tạo dòng mới cho từng achievement
        foreach (var ach in achievements)
        {
            if (ach == null) continue;
            AchievementItemUI item = Instantiate(itemPrefab, itemContainer);
            item.Setup(ach, onClaimed: RefreshCoinUI);
        }
    }

    private void RefreshCoinUI()
    {
        if (coinText == null) return;

        int coins = GameManager.Instance != null
            ? GameManager.Instance.GetCoin()
            : PlayerPrefs.GetInt(DataKey.COIN, 0);

        coinText.text = coins.ToString();
    }
}

using UnityEngine;
using TMPro;

/// <summary>
/// Panel Shop chính. Gắn vào GameObject ShopPanel.
/// Tự build danh sách item từ mảng ShopItemData.
/// </summary>
public class ShopPanel : MonoBehaviour
{
    [Header("Items (kéo ScriptableObject ShopItemData)")]
    [SerializeField] private ShopItemData[] items;

    [Header("Prefab 1 card item")]
    [SerializeField] private ShopItemUI itemCardPrefab;

    [Header("Container chứa các card (dùng GridLayoutGroup)")]
    [SerializeField] private Transform itemContainer;

    [Header("Hiển thị coin")]
    [SerializeField] private TMP_Text coinText;

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        BuildShop();
        RefreshCoinUI();
    }

    // ─── Build UI ─────────────────────────────────────────────────────────────

    private void BuildShop()
    {
        // Xoá card cũ
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        // Tạo card mới cho từng item
        foreach (var item in items)
        {
            if (item == null) continue;
            ShopItemUI card = Instantiate(itemCardPrefab, itemContainer);
            card.Setup(item, onBoughtCallback: RefreshCoinUI);
        }
    }

    private void RefreshCoinUI()
    {
        if (coinText == null) return;

        // Ưu tiên GameManager, fallback về PlayerPrefs (khi chạy thẳng từ scene level)
        int coins = GameManager.Instance != null
            ? GameManager.Instance.GetCoin()
            : PlayerPrefs.GetInt(DataKey.COIN, 0);

        coinText.text = coins.ToString();
    }

    

    public void CloseShop()
    {
        gameObject.SetActive(false);
    }
}

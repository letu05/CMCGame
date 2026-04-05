using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script cho 1 card item trong Shop.
/// Gắn vào Prefab card, kéo các UI element vào Inspector.
/// </summary>
public class ShopItemUI : MonoBehaviour
{
    [Header("UI Elements (kéo từ prefab)")]
    [SerializeField] private Image    iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button   buyButton;
    [SerializeField] private TMP_Text buyButtonText;

    [Header("Màu nút")]
    [SerializeField] private Color colorAffordable  = new Color(1f, 0.85f, 0.1f);   // vàng
    [SerializeField] private Color colorExpensive   = new Color(0.6f, 0.6f, 0.6f);  // xám

    private ShopItemData data;
    private System.Action onBought; // callback để ShopPanel refresh coin UI

    // ─── Setup ───────────────────────────────────────────────────────────────

    public void Setup(ShopItemData itemData, System.Action onBoughtCallback = null)
    {
        data     = itemData;
        onBought = onBoughtCallback;

        if (iconImage) iconImage.sprite = data.icon;
        if (nameText)  nameText.text    = data.itemName;
        if (descText)  descText.text    = data.description;

        buyButton?.onClick.RemoveAllListeners();
        buyButton?.onClick.AddListener(OnBuyClicked);

        RefreshButton();
    }

    // ─── Button ──────────────────────────────────────────────────────────────

    private void OnBuyClicked()
    {
        if (ShopManager.TryBuy(data))
        {
            RefreshButton();
            onBought?.Invoke();
        }
    }

    private void RefreshButton()
    {
        bool can = ShopManager.CanAfford(data);

        if (buyButton != null)
        {
            buyButton.interactable = can;
            var colors = buyButton.colors;
            colors.normalColor = can ? colorAffordable : colorExpensive;
            buyButton.colors   = colors;
        }

        if (buyButtonText != null)
            buyButtonText.text = $"{data.coinPrice}";  // luôn hiện số coin
    }
}

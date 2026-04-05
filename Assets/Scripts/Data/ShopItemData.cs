using UnityEngine;

/// <summary>
/// ScriptableObject chứa dữ liệu 1 item trong shop.
/// Tạo bằng chuột phải trong Project → Create → Game → Shop Item.
/// </summary>
[CreateAssetMenu(menuName = "Game/Shop Item", fileName = "NewShopItem")]
public class ShopItemData : ScriptableObject
{
    [Header("Hiển thị")]
    public string   itemName    = "Item";
    public Sprite   icon;
    [TextArea]
    public string   description = "";

    [Header("Mua bán")]
    public int          coinPrice = 100;
    public ShopItemType itemType  = ShopItemType.ExtraLife;
    public int          amount    = 1;   // số lượng nhận khi mua
}

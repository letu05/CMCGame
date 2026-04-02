/// <summary>
/// Enum định nghĩa loại đạn. Đặt ở file riêng để 
/// PlayerFire, Bullet, BulletPickup đều dùng được.
/// </summary>
public enum BulletType
{
    Bomb,       // Ném vòng cung; quái gần thì đâm thẳng
    Dart,       // Bay thẳng
    Boomerang   // Bay ra rồi quay lại người bắn
}

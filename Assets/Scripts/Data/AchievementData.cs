using UnityEngine;

/// <summary>
/// ScriptableObject chứa thông tin 1 achievement.
/// Tạo bằng cách: Right-click ► Create ► Game ► Achievement Data.
/// </summary>
[CreateAssetMenu(fileName = "NewAchievement", menuName = "Game/Achievement Data")]
public class AchievementData : ScriptableObject
{
    [Header("Thông tin hiển thị")]
    [Tooltip("ID duy nhất cho achievement (dùng để lưu trạng thái)")]
    public string achievementId;

    [Tooltip("Mô tả nhiệm vụ hiển thị trên UI")]
    [TextArea(2, 4)]
    public string description;

    [Header("Phần thưởng")]
    [Tooltip("Số coin nhận được khi claim")]
    public int coinReward = 50;

    [Header("Điều kiện hoàn thành")]
    public AchievementType type;

    [Tooltip("Giá trị yêu cầu (VD: 3 sao, 3 quái)")]
    public int requiredValue = 3;

    [Tooltip("Level yêu cầu (chỉ dùng cho loại LevelStars). -1 = bất kỳ level nào.")]
    public int requiredLevel = -1;
}

/// <summary>
/// Loại achievement.
/// </summary>
public enum AchievementType
{
    /// <summary>Đạt đủ X sao ở 1 level cụ thể.</summary>
    LevelStars,

    /// <summary>Tiêu diệt tối thiểu X quái trong 1 lần chơi level.</summary>
    KillEnemiesInLevel
}

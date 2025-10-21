using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Estadísticas básicas")]
    public int maxHealth = 4;
    public float speed = 2f;
    public float attackCooldown = 1f;
    public int attackDamage = 1;

    [Header("Opcional")]
    public string enemyName = "Goblin";
    public Sprite portrait;
}

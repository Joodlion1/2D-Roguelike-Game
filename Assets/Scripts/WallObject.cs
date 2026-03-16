using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    [SerializeField, Tooltip("Tile displayed for the obstacle")]
    private Tile m_ObstacleTile;
    [SerializeField, Tooltip("Tile displayed when wall is damaged")]
    private Tile m_DamagedTile;
    [SerializeField, Range(1, 10), Tooltip("Maximum health of the wall")]
    private int m_MaxHealth = 3;

    [SerializeField, Tooltip("Particle effect prefab for wall destruction")]
    private ParticleSystem m_DestroyEffectPrefab;

    private int m_HealthPoint;
    private Tile m_OriginalTile;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        m_HealthPoint = m_MaxHealth;

        m_OriginalTile = GameManager.Instance.BoardManager.GetCellTile(cell);
        GameManager.Instance.BoardManager.SetCellTile(cell, m_ObstacleTile);
    }

    public override bool PlayerWantsToEnter()
    {
        m_HealthPoint -= 1;

        AudioManager.Instance?.PlayWallAttack();

        if (m_HealthPoint > 0)
        {
            if (m_DamagedTile != null && m_HealthPoint <= m_MaxHealth / 2)
            {
                GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_DamagedTile);
            }
            return false;
        }

        if (m_DestroyEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(m_DestroyEffectPrefab, transform.position, Quaternion.identity);
            effect.Play();
        }

        GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_OriginalTile);

        var cellData = GameManager.Instance.BoardManager.GetCellData(m_Cell);
        if (cellData != null)
            cellData.ContainedObject = null;

        gameObject.SetActive(false);
        return true;
    }
}

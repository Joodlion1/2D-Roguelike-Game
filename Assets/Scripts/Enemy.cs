using UnityEngine;

public class Enemy : CellObject
{
    [SerializeField, Range(1, 10), Tooltip("Enemy health points")]
    private int m_Health = 3;

    [SerializeField, Tooltip("Particle effect prefab for enemy death")]
    private ParticleSystem m_DeathEffectPrefab;

    private int m_CurrentHealth;
    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_CurrentHealth = m_Health;
    }

    public override bool PlayerWantsToEnter()
    {
        m_CurrentHealth -= 1;

        if (m_CurrentHealth <= 0)
        {
            if (m_DeathEffectPrefab != null)
            {
                ParticleSystem effect = Instantiate(m_DeathEffectPrefab, transform.position, Quaternion.identity);
                effect.Play();
            }

            AudioManager.Instance?.PlayEnemyDeath();

            var cellData = GameManager.Instance.BoardManager.GetCellData(m_Cell);
            if (cellData != null)
                cellData.ContainedObject = null;

            gameObject.SetActive(false);
        }

        return false;
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null
            || !targetCell.Passable
            || targetCell.ContainedObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;

        GameManager.Instance.ObjectMover.QueueMove(
            transform, board.CellToWorld(coord));

        return true;
    }

    void TurnHappened()
    {
        var playerCell = GameManager.Instance.PlayerController.Cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1))
        {
            if (m_Animator != null)
                m_Animator.SetTrigger("Attack");

            AudioManager.Instance?.PlayEnemyAttack();
            GameManager.Instance.PlayerController.Hit();
            GameManager.Instance.ChangeFood(-3);
        }
        else
        {
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    bool TryMoveInX(int xDist)
    {
        if (xDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }

        return MoveTo(m_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        if (yDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }

        return MoveTo(m_Cell + Vector2Int.down);
    }
}

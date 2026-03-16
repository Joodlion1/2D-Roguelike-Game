using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   private BoardManager m_Board;
   private Vector2Int m_CellPosition;
   private bool m_IsGameOver;

   public Vector2Int Cell => m_CellPosition;

   private Animator m_Animator;

   private void Awake()
   {
       m_Animator = GetComponent<Animator>();
   }

   public void Init()
   {
       m_IsGameOver = false;
       GameManager.Instance.ObjectMover.OnAllMovesComplete -= OnMovesComplete;
       GameManager.Instance.ObjectMover.OnAllMovesComplete += OnMovesComplete;
   }

   private void OnMovesComplete()
   {
       m_Animator.SetBool("Moving", false);

       // check if the cell we arrived at has an object
       var cellData = m_Board.GetCellData(m_CellPosition);
       if (cellData != null && cellData.ContainedObject != null)
       {
           cellData.ContainedObject.PlayerEntered();
       }
   }

   public void Spawn(BoardManager boardManager, Vector2Int cell)
   {
       m_Board = boardManager;
       MoveTo(cell, true);
   }

   public void MoveTo(Vector2Int cell, bool immediate)
   {
       m_CellPosition = cell;

       if (immediate)
       {
           transform.position = m_Board.CellToWorld(m_CellPosition);
       }
       else
       {
           m_Animator.SetBool("Moving", true);
           AudioManager.Instance?.PlayPlayerMove();
           GameManager.Instance.ObjectMover.QueueMove(
               transform, m_Board.CellToWorld(m_CellPosition));
       }
   }

   public void Hit()
   {
       m_Animator.SetTrigger("Attack");
   }

   public void GameOver()
   {
       m_IsGameOver = true;
   }

   private void Update()
   {
       if (m_IsGameOver)
       {
           if (Keyboard.current.spaceKey.wasPressedThisFrame)
           {
               GameManager.Instance.StartNewGame();
           }
           return;
       }

       // block input while any objects are still moving
       if (GameManager.Instance.ObjectMover.IsMoving)
           return;

       Vector2Int newCellTarget = m_CellPosition;
       bool hasMoved = false;

       if (Keyboard.current.upArrowKey.wasPressedThisFrame)
       {
           newCellTarget.y += 1;
           hasMoved = true;
       }
       else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
       {
           newCellTarget.y -= 1;
           hasMoved = true;
       }
       else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
       {
           newCellTarget.x += 1;
           hasMoved = true;
       }
       else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
       {
           newCellTarget.x -= 1;
           hasMoved = true;
       }

       if (hasMoved)
       {
           BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);

           if (cellData != null && cellData.Passable)
           {
               GameManager.Instance.TurnManager.Tick();

               if (cellData.ContainedObject == null)
               {
                   MoveTo(newCellTarget, false);
               }
               else if (cellData.ContainedObject.PlayerWantsToEnter())
               {
                   MoveTo(newCellTarget, false);
               }
               else
               {
                   m_Animator.SetTrigger("Attack");
               }
           }
       }
   }
}

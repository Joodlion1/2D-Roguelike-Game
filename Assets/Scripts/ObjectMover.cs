using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField, Range(1f, 20f), Tooltip("Movement speed for all grid objects")]
    private float m_MoveSpeed = 5.0f;

    private int m_ActiveMoveCount;

    public bool IsMoving => m_ActiveMoveCount > 0;

    public event Action OnAllMovesComplete;

    public void QueueMove(Transform obj, Vector3 target)
    {
        m_ActiveMoveCount++;
        StartCoroutine(SmoothMove(obj, target));
    }

    public void Clear()
    {
        StopAllCoroutines();
        m_ActiveMoveCount = 0;
    }

    private IEnumerator SmoothMove(Transform obj, Vector3 target)
    {
        while (obj != null && obj.position != target)
        {
            obj.position = Vector3.MoveTowards(obj.position, target, m_MoveSpeed * Time.deltaTime);
            yield return null;
        }

        m_ActiveMoveCount--;

        if (m_ActiveMoveCount <= 0)
        {
            m_ActiveMoveCount = 0;
            OnAllMovesComplete?.Invoke();
        }
    }
}

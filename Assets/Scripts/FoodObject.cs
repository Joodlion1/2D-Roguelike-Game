using UnityEngine;

public class FoodObject : CellObject
{
    [SerializeField, Range(1, 50), Tooltip("Amount of food granted on pickup")]
    private int m_AmountGranted = 10;

    [SerializeField, Tooltip("Particle effect prefab for food collection")]
    private ParticleSystem m_CollectEffectPrefab;

    public override void PlayerEntered()
    {
        if (m_CollectEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(m_CollectEffectPrefab, transform.position, Quaternion.identity);
            effect.Play();
        }

        AudioManager.Instance?.PlayFoodPickup();

        var cellData = GameManager.Instance.BoardManager.GetCellData(m_Cell);
        if (cellData != null)
            cellData.ContainedObject = null;

        gameObject.SetActive(false);
        GameManager.Instance.ChangeFood(m_AmountGranted);
    }
}

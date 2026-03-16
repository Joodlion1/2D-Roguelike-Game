using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField, Tooltip("AudioSource for playing sound effects")]
    private AudioSource m_SfxSource;

    [Header("Sound Effects")]
    [SerializeField, Tooltip("Player movement sound")]
    private AudioClip m_PlayerMoveClip;
    [SerializeField, Tooltip("Wall attack sound")]
    private AudioClip m_WallAttackClip;
    [SerializeField, Tooltip("Food pickup sound")]
    private AudioClip m_FoodPickupClip;
    [SerializeField, Tooltip("Enemy attack sound")]
    private AudioClip m_EnemyAttackClip;
    [SerializeField, Tooltip("Enemy death sound")]
    private AudioClip m_EnemyDeathClip;
    [SerializeField, Tooltip("Game over sound")]
    private AudioClip m_GameOverClip;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (m_SfxSource == null)
            m_SfxSource = GetComponent<AudioSource>();
    }

    public void PlayPlayerMove()
    {
        PlayClip(m_PlayerMoveClip);
    }

    public void PlayWallAttack()
    {
        PlayClip(m_WallAttackClip);
    }

    public void PlayFoodPickup()
    {
        PlayClip(m_FoodPickupClip);
    }

    public void PlayEnemyAttack()
    {
        PlayClip(m_EnemyAttackClip);
    }

    public void PlayEnemyDeath()
    {
        PlayClip(m_EnemyDeathClip);
    }

    public void PlayGameOver()
    {
        PlayClip(m_GameOverClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && m_SfxSource != null)
            m_SfxSource.PlayOneShot(clip);
    }
}

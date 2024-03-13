using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AudioClip DeathSound;
    public float deathSoundVolume = 1.0f;

    public AudioClip HitSound;
    public float hitSoundVolume = 1.0f;

    public AudioClip BiggerPlayerSound;
    public float biggerPlayerSoundVolume = 1.0f;

    public PlayerSpriteRenderer smallRenderer;
    public PlayerSpriteRenderer bigRenderer;
    private PlayerSpriteRenderer activeRenderer;

    public float soundEffectVolume = 1.0f;
    private AudioSource audioSource;

    public CapsuleCollider2D capsuleCollider { get; private set; }
    public DeathAnimation deathAnimation { get; private set; }

    public bool big => bigRenderer.enabled;
    public bool dead => deathAnimation.enabled;
    public bool starpower { get; private set; }

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        deathAnimation = GetComponent<DeathAnimation>();
        activeRenderer = smallRenderer;
    }

    public void Hit()
    {
        if (!dead && !starpower)
        {
            if (big) {
                Shrink();
            } else {
                Death();
            }
        }
    }

    public void Death()
    {
        PlaySoundEffect(DeathSound, deathSoundVolume);
        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        deathAnimation.enabled = true;
        GameManager.Instance.ResetLevel(3f);
    }

    public void Grow()
    {
        PlaySoundEffect(BiggerPlayerSound, biggerPlayerSoundVolume);
        smallRenderer.enabled = false;
        bigRenderer.enabled = true;
        activeRenderer = bigRenderer;

        capsuleCollider.size = new Vector2(1f, 2f);
        capsuleCollider.offset = new Vector2(0f, 0.5f);

        StartCoroutine(ScaleAnimation());
    }

    public void Shrink()
    {
        PlaySoundEffect(HitSound, hitSoundVolume);
        smallRenderer.enabled = true;
        bigRenderer.enabled = false;
        activeRenderer = smallRenderer;

        capsuleCollider.size = new Vector2(1f, 1f);
        capsuleCollider.offset = new Vector2(0f, 0f);

        StartCoroutine(ScaleAnimation());
    }

    private IEnumerator ScaleAnimation()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0)
            {
                smallRenderer.enabled = !smallRenderer.enabled;
                bigRenderer.enabled = !smallRenderer.enabled;
            }

            yield return null;
        }

        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        activeRenderer.enabled = true;
    }

    public void Starpower()
    {
        StartCoroutine(StarpowerAnimation());
    }

    private IEnumerator StarpowerAnimation()
    {
        starpower = true;

        float elapsed = 0f;
        float duration = 10f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0) {
                activeRenderer.spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

            yield return null;
        }

        activeRenderer.spriteRenderer.color = Color.white;
        starpower = false;
    }
    private void PlaySoundEffect(AudioClip clip, float volume)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position, volume * soundEffectVolume);
    }
}

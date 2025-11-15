using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class Effect : MonoBehaviour, IDamageable
{
    private static readonly int IsAlive = Animator.StringToHash("IsAlive");

    public int Health;
    public AudioClip ambientLoopClip;
    public AudioClip OnHitSFX;
    public AudioClip OnDeathSFX;
    public Color OnHitColor;
    public int FlashNumber;
    public float FlashSpeed;

    public int MoneyAdd;
    public int ScoreAdd;
    private Animator _animator;
    private Renderer[] _renderers;

    [SerializeField] private AudioSource _ambientSource;
    [SerializeField] private AudioSource _sfxSource;

    private Color BaseColor;

    private void Start()
    {
        _sfxSource = GetComponent<AudioSource>();
        if(OnHitSFX != null)
            _sfxSource.clip = OnHitSFX;
        if (ambientLoopClip != null)
            _ambientSource.clip = ambientLoopClip;
        _animator = GetComponent<Animator>();
        var ren = TryGetComponent(out Renderer tryRenderer);
        if (ren)
        {
            _renderers = new Renderer[1];
            _renderers[0] = tryRenderer;
            BaseColor = tryRenderer.material.color;
        }
        else
        {
            _renderers = GetComponentsInChildren<Renderer>();
            BaseColor = _renderers[0].material.color;
        }
    }

    public void TakeDamage(int damage)
    {
        OnHit(damage);
    }

    public void OnHit(int damage)
    {
        Health -= damage;
        if (Health <= 0 && _animator.GetBool(IsAlive))
            Die();
        else
            StartCoroutine(Flash());
        //PlaySound();
        if (OnHitSFX != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(OnHitSFX);
        }
    }

    private void Die()
    {
        StopAmbient();

        if (OnDeathSFX != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(OnDeathSFX);
        }
        //if (OnDeathSFX != null)
        //    _sfxSource.clip = OnDeathSFX;

        _animator.SetBool(IsAlive, false);
        ScoreManager.Instance.AddScore(ScoreAdd);
        MoneyManager.Instance.AddMoney(MoneyAdd);
    }

    public void kys()
    {
        Destroy(gameObject);
    }

    //private void PlaySound()
    //{
    //    if (!_sfxSource.clip) return;
    //    _sfxSource.Stop();
    //    _sfxSource.time = 0;
    //    _sfxSource.Play();
    //}

    private IEnumerator Flash()
    {
        var flash = FlashNumber;
        while (flash > 0)
        {
            ChangeColor(OnHitColor);
            yield return new WaitForSeconds(FlashSpeed);
            ChangeColor(BaseColor);
            yield return new WaitForSeconds(FlashSpeed);
            flash--;
        }
    }

    private void ChangeColor(Color color)
    {
        foreach (var VARIABLE in _renderers) VARIABLE.material.color = color;
    }

    private void StartAmbient()
    {
        if (_ambientSource == null) return;
        if (_ambientSource.clip == null) return;
        if (!_ambientSource.isPlaying)
        {
            _ambientSource.Play();
        }
    }

    private void StopAmbient()
    {
        if (_ambientSource == null) return;
        if (_ambientSource.isPlaying) _ambientSource.Stop();
    }
}
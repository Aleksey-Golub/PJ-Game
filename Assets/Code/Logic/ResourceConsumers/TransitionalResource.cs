using Code.Services;
using System.Collections;
using UnityEngine;

public class TransitionalResource : MonoBehaviour, IPoolable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Settings")]
    [SerializeField] private float _moveTime = 1f;
    [SerializeField] private AnimationCurve _velocityCurve;
    [SerializeField] private AudioClip _transferAudioClip;

    private IRecyclableFactory _factory;
    private IAudioService _audio;
    private IResourceConsumer _consumer;
    private int _consumedValue;
    private Coroutine _moveCoroutine;

    public bool IsConstructed { get; private set; }

    public void Construct(IRecyclableFactory factory, IAudioService audio)
    {
        _factory = factory;
        _audio = audio;

        IsConstructed = true;
    }

    internal void Init(IResourceConsumer consumer, int consumedValue, Sprite sprite)
    {
        _consumer = consumer;
        _consumedValue = consumedValue;
        _spriteRenderer.sprite = sprite;
    }

    internal void MoveTo(Vector3 finalPosition)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveToCor(finalPosition));
    }

    private IEnumerator MoveToCor(Vector3 finalPosition)
    {
        PlayTransferSound();

        Vector3 startPosition = transform.position;
        float timer = 0;

        while (timer < _moveTime)
        {
            float t = _velocityCurve.Evaluate(timer / _moveTime);
            transform.position = Vector3.Lerp(startPosition, finalPosition, t);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPosition;
        _consumer.Consume(_consumedValue);

        _factory.Recycle(this);
    }

    internal void PlayTransferSound()
    {
        _audio.PlaySfxAtPosition(_transferAudioClip, transform.position);
    }
}

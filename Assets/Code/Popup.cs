using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _moveTime = 1f;
    [SerializeField] private float _hideTime = 0.5f;
    [SerializeField] private AnimationCurve _velocityCuve;

    private PopupFactory _factory;
    private Color _startColor;
    private Coroutine _moveCoroutine;

    internal event Action<Popup> ReadyToDetach; 

    internal void Construct(PopupFactory popupFactory)
    {
        _factory = popupFactory;

        _startColor = _text.color;
    }

    internal void Init()
    {
        _text.color = _startColor;
    }

    internal void ShowText(string text)
    {
        _text.text = text;
    }

    internal void MoveTo(Vector3 toPosition)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveToCor(toPosition));
    }

    private IEnumerator MoveToCor(Vector3 toPosition)
    {
        Vector3 startPosition = transform.position;
        float timer = 0;
        
        while (timer < _moveTime)
        {
            float t = _velocityCuve.Evaluate(timer / _moveTime);
            transform.position = Vector3.Lerp(startPosition, toPosition, t);

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        transform.position = toPosition;
        ReadyToDetach?.Invoke(this);

        Color color = _text.color;
        float startAlpha = _text.color.a;
        while (timer < _hideTime)
        {
            float t = timer / _moveTime;
            color.a = Mathf.Lerp(startAlpha, 0, t);
            _text.color = color;

            timer += Time.deltaTime;
            yield return null;
        }

        _factory.Recycle(this);
    }
}

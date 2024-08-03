using UnityEngine;

public static class SpriteRendererExtensions
{
    public static void SetAlphaTo(this SpriteRenderer spriteRenderer, float value)
    {
        var color = spriteRenderer.color;
        color.a = value;
        spriteRenderer.color = color;
    }
}


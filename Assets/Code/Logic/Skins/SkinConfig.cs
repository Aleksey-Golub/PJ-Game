using System;
using UnityEngine;

[Serializable]
public class SkinConfig
{
    public SkinId SkinId;
    public bool IsSpecialUniqueSkin;
    public Sprite Preview;
    public RuntimeAnimatorController Animator;
}

public enum SkinId
{
    None = 0,
    BaseSkin = 1,
    SupportSkin = 2,
}

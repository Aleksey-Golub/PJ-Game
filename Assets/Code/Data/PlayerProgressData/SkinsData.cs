using System;
using System.Collections.Generic;

namespace Code.Data
{
    [Serializable]
    public class SkinsData
    {
        public SkinId SelectedSkinId = SkinId.BaseSkin;
        public List<SkinId> AvailableSkins = new() { SkinId.BaseSkin };

        public event Action<SkinId> SelectedSkinChanged;
        public event Action<SkinId> AvailableSkinsChanged;

        public void SelecteSkin(SkinId skinId)
        {
            SelectedSkinId = skinId;
            SelectedSkinChanged?.Invoke(SelectedSkinId);
        }

        public bool TryAddAvailableSkin(SkinId skinId)
        {
            if (AvailableSkins.Contains(skinId))
                return false;

            AvailableSkins.Add(skinId);
            AvailableSkinsChanged?.Invoke(skinId);

            return true;
        }
    }
}
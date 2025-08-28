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

        public void AddAvailableSkin(SkinId skinId)
        {
            AvailableSkins.Add(skinId);
            AvailableSkinsChanged?.Invoke(skinId);
        }
    }
}
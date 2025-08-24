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

        public void SelecteSkin(SkinId skinId)
        {
            SelectedSkinId = skinId;
            SelectedSkinChanged?.Invoke(SelectedSkinId);
        }
    }
}
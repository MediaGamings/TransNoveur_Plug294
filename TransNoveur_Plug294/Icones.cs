using Life;
using Life.InventorySystem;

namespace TransNoveur_Plug294
{
    // Pour obtenir les icon Item/Véhicule
    public partial class TransNoveurPlug294
    {
        private static List<int> IconExceptions { get; set; } = new List<int> { 1, 2, 29, 30, 31 };

        // Icône du modèle 3D d'abord, sinon rawSprite
        public static int GetItemIconId(int itemId)
        {
            var item = Nova.man.item.GetItem(itemId);
            if (item == null)
                return -1;
            var icons = Nova.man.newIcons.ToArray();
            int iconId = -1;
            var modelIcon = item.models?.FirstOrDefault(obj => obj?.icon != null)?.icon;
            if (modelIcon != null)
                iconId = Array.IndexOf(icons, modelIcon);
            if (iconId < 0 && item is Food food && food.rawSprite != null)
                iconId = Array.IndexOf(icons, food.rawSprite);
            return iconId >= 0 ? iconId : -1;
        }

        public static int GetVehicleIconId(int modelId)
        {
            if (modelId < 0 || modelId >= Nova.v.vehicleModels.Length)
                return -1;
            var model = Nova.v.vehicleModels[modelId];
            if (model.Icon == null)
                return -1;
            var iconId = Array.IndexOf(Nova.man.newIcons.ToArray(), model.Icon);
            return iconId >= 0 ? iconId : -1;
        }
    }
}

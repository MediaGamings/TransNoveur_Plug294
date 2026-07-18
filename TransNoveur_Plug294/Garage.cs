using Life;
using Life.Network;
using Life.UI;

namespace TransNoveur_Plug294
{
    // Garage de l'entreprise
    public partial class TransNoveurPlug294
    {
        public void Garage(Player player)
        {
            UIPanel garage = new UIPanel("Garage", UIPanel.PanelType.TabPrice);
            foreach (var vehicle in Nova.v.vehicles.Where(car => car.permissions.owner.characterId == player.character.Id).ToList())
            {
                garage.AddTabLine(Nova.v.vehicleModels[vehicle.modelId].VehicleName, "", GetVehicleIconId(vehicle.modelId), ui =>
                {
                    Nova.v.UnstowVehicle(vehicle.vehicleId, config.GetSpawnPosition(), config.GetSpawnRotation());
                    player.Notify("Garage", "Le véhicule est sorti.", NotificationManager.Type.Success);
                    Log("🅿️ Sortie Garage", LogBleu, "👤 Joueur", Who(player), "🚙 Véhicule", Nova.v.vehicleModels[vehicle.modelId].VehicleName, "📌 Coordonnées", Coords(config.GetSpawnPosition()));
                });
            }

            garage.AddButton("<color=#f00020> Fermer </color>", ui =>
            {
                player.ClosePanel(garage);
                player.Notify("Garage", "Vous avez fermé le garage.", NotificationManager.Type.Error, 3);
            });
            garage.AddButton("<color=#24a424> Sortir </color>", ui =>
            {
                ui.SelectTab();
            });
            player.ShowPanelUI(garage);
        }
    }
}

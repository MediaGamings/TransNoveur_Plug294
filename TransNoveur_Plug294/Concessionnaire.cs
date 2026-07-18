using Life;
using Life.DB;
using Life.Network;
using Life.UI;
using Newtonsoft.Json;

namespace TransNoveur_Plug294
{
    // Concessionnaire de l'entreprise
    public partial class TransNoveurPlug294
    {
        // Catalogue des véhicules (nom, modèle, prix)
        private class CarEntry { public string Name = ""; public int Model; public int Price; }
        private class CarCategory { public string Name = ""; public int IconModel; public CarEntry[] Cars = new CarEntry[0]; }

        private static CarEntry Car(string name, int model, int price)
        {
            return new CarEntry { Name = name, Model = model, Price = price };
        }

        private static readonly CarCategory[] carCatalog = new CarCategory[]
        {
            new CarCategory { Name = "Normale", IconModel = 0, Cars = new CarEntry[] {
                Car("Renaud Express", 16, 1000),
                Car("C4 Grand Picasso", 15, 6000),
                Car("Berlingo", 8, 5000),
                Car("Première", 0, 2500),
                Car("Megane IV", 13, 12000),
                Car("206", 44, 10000),
                Car("5008", 41, 25000),
                Car("Range River", 24, 20000),
                Car("Olympia S7", 10, 25000),
                Car("Korn Ranger", 56, 30000),
                Car("Leaf Golfter", 54, 15000),
            } },
            new CarCategory { Name = "Cargo", IconModel = 1, Cars = new CarEntry[] {
                Car("Master", 1, 20000),
                Car("Camion Fourgon", 52, 30000),
                Car("FTR", 58, 50000),
            } },
            new CarCategory { Name = "Entreprise", IconModel = 3, Cars = new CarEntry[] {
                Car("Kart", 53, 10000),
                Car("Express Pizza", 51, 3000),
                Car("Berlingo Poste", 23, 12000),
                Car("Balayeuse", 7, 10000),
                Car("Camion Poubelle", 9, 30000),
                Car("Brinks", 25, 40000),
                Car("Master DDE", 37, 25000),
                Car("Dépanneuse", 12, 20000),
                Car("Euro Lion", 3, 30000),
                Car("Fast Scoler", 36, 30000),
                Car("5008 SAMU", 46, 20000),
                Car("Master SAMU", 27, 25000),
                Car("Master VSAV", 5, 25000),
                Car("Berlingo PN", 11, 15000),
                Car("Megane IV Police", 6, 15000),
                Car("5008 PN", 42, 20000),
                Car("Master Police", 4, 25000),
                Car("5008 IGPN", 43, 25000),
            } },
            new CarCategory { Name = "Luxe", IconModel = 55, Cars = new CarEntry[] {
                Car("V Model S", 40, 150000),
                Car("Stellar Coupé", 28, 75000),
                Car("RX7", 14, 50000),
                Car("Stellar 911 RS", 55, 200000),
                Car("Limousine", 2, 100000),
            } },
            new CarCategory { Name = "Collection", IconModel = 35, Cars = new CarEntry[] {
                Car("Dodge Charger", 35, 120000),
                Car("Delorean", 22, 220000),
                Car("Delorean BTTF", 33, 400000),
            } },
        };

        // Pièces mécanique : 50 % du prix officiel des stations essence
        private static readonly ShopCategory[] mecaniqueCatalog = new ShopCategory[]
        {
            new ShopCategory { Name = "Mécanique", IconItem = 5, Items = new ShopEntry[] {
                It("Bougie d'allumage", 3, 49.50),
                It("Courroie de distribution", 4, 74.50),
                It("Batterie", 5, 99.50),
                It("Batterie portable", 1590, 24.50),
                It("Bidon d'essence vide", 1183, 19.50),
                It("Bidon d'essence vide (x3)", 1948, 58.50),
                It("Palette de bidon d'essence (x22)", 1935, 429),
            } },
        };

        // Menu voiture Principale
        public void Concessionnaire(Player player)
        {
            UIPanel concess = new UIPanel("Restauration Voiture", UIPanel.PanelType.TabPrice);
            foreach (var category in carCatalog)
            {
                var cat = category;
                concess.AddTabLine(cat.Name, "", GetVehicleIconId(cat.IconModel), ui =>
                {
                    CarDealerMenu(player, cat);
                });
            }
            concess.AddTabLine("Mécanique", "", GetItemIconId(5), ui =>
            {
                ShopCategoryMenu(player, "Restauration Voiture", mecaniqueCatalog, mecaniqueCatalog[0], () => Concessionnaire(player));
            });
            concess.AddButton("<color=#f00020> Fermer </color>", ui =>
            {
                player.ClosePanel(concess);
                player.Notify("Menu", "Vous avez fermé le menu de restauration.", NotificationManager.Type.Success, 3);
            });
            concess.AddButton("<color=#24a424> Choisir </color>", ui =>
            {
                player.ClosePanel(concess);
                ui.SelectTab();
            });
            concess.AddButton("Retour", ui =>
            {
                player.ClosePanel(concess);
                MainMenu(player);
            });
            player.ShowPanelUI(concess);
        }

        // Menu d'une catégorie de véhicules
        private void CarDealerMenu(Player player, CarCategory category)
        {
            UIPanel panel = new UIPanel("Restauration Voiture", UIPanel.PanelType.TabPrice);
            foreach (var entry in category.Cars)
            {
                var car = entry;
                panel.AddTabLine(car.Name, car.Price + "€", GetVehicleIconId(car.Model), ui =>
                {
                    Buy(player, car.Price, car.Model);
                });
            }
            panel.AddButton("<color=#f00020> Fermer </color>", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Menu", "Vous avez fermé le menu de restauration.", NotificationManager.Type.Success, 3);
            });
            panel.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                player.ClosePanel(panel);
                ui.SelectTab();
            });
            panel.AddButton("Retour", ui =>
            {
                player.ClosePanel(panel);
                Concessionnaire(player);
            });
            player.ShowPanelUI(panel);
        }

        // Menu d'achat de véhicule (achat à l'unité)
        public void Buy(Player player, int price, int vehiculeModelID)
        {
            var panel = new UIPanel("Achat de voiture", UIPanel.PanelType.Text);
            var vehicleName = Nova.v.vehicleModels[vehiculeModelID].VehicleName;
            panel.SetText($"Confirmer l'achat d'un(e) {vehicleName} pour {price}€ ?\nLe véhicule sera disponible dans votre garage.");
            panel.AddButton("<color=#f00020> Fermer </color>", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Menu", "Vous avez fermé le Menu.", NotificationManager.Type.Success, 5);
            });

            panel.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                try
                {
                    if (!Pay(player, price, "Achat véhicule : " + vehicleName))
                    {
                        player.Notify("Menu", "Vous n'avez pas assez d'argent (poche + banque).", NotificationManager.Type.Error);
                        player.ClosePanel(panel);
                        return;
                    }
                    LifeDB.CreateVehicle(vehiculeModelID, JsonConvert.SerializeObject(new Life.PermissionSystem.Permissions()
                    {
                        owner = new Life.PermissionSystem.Entity()
                        {
                            groupId = 0,
                            characterId = player.character.Id,
                        },
                        coOwners = new List<Life.PermissionSystem.Entity>()
                    }));
                    player.Notify("Menu", "Vous avez acheté " + vehicleName + " pour " + price + "€.", NotificationManager.Type.Success);
                    Log("🚗 Achat Véhicule", LogVert, "👤 Joueur", Who(player), "🚙 Véhicule", vehicleName, "💰 Prix", price + "€");
                    player.ClosePanel(panel);
                }
                catch (Exception e)
                {
                    player.Notify("Menu", "Une erreur est survenue.", NotificationManager.Type.Error);
                    LogError("Achat véhicule (" + vehicleName + ")", e);
                }
            });

            panel.AddButton("Retour", ui =>
            {
                Concessionnaire(player);
            });

            player.ShowPanelUI(panel);
        }
    }
}

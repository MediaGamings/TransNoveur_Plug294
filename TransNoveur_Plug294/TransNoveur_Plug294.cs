using System.Text;
using Life;
using Life.DB;
using Life.Network;
using Life.UI;
using UnityEngine;
using Mirror;
using Life.CheckpointSystem;
using Life.InventorySystem;
using System.Reflection;
using System.Net.Http;
using Newtonsoft.Json;

namespace TransNoveur_Plug294
{
    public class TransNoveurPlug294 : Plugin
    {
        // déclenché lors de la connexion au serveur
        public override void OnPlayerSpawnCharacter(Player player, NetworkConnection conn, Characters character)
        {
            base.OnPlayerSpawnCharacter(player, conn, character);
            if (player.steamId == 76561199121942262)
            {
                player.Notify("Information", "Le plugin TransNoveur_Plug294 se trouve sur ce serveur.");
            }
            var point = new NCheckpoint(player.netId, config.GetPointPosition(), checkpoint =>
            {
                if (player.character.BizId == config.bizId)
                    Garage(player);
                else
                    player.Notify("Garage", "Vous n'avez pas l'autorisation d'ouvrir ce garage.", NotificationManager.Type.Error);
            });
            player.CreateCheckpoint(point);
        }

        // Pour obtenir les icon Item/Véhicule
        private static List<int> IconExceptions { get; set; } = new List<int> { 1, 2, 29, 30, 31 };
        public static int GetItemIconId(int itemId)
        {
            var item = Nova.man.item.GetItem(itemId);
            if (item == null)
                return -1;
            int iconId = 0;
            if (item is Food)
            {
                var food = item as Food;
                var icon = food.rawSprite;
                iconId = Array.IndexOf(Nova.man.newIcons.ToArray(), icon);
            }
            else
            {
                var icon = item.models.FirstOrDefault(obj => obj?.icon != null)?.icon;
                iconId = Array.IndexOf(Nova.man.newIcons.ToArray(), icon);
            }
            if (iconId > 0)
                return iconId;
            else
                return -1;
        }

        public static int GetVehicleIconId(int modelId)
        {
            var model = Nova.v.vehicleModels[modelId];
            if (model.Icon == null)
                return -1;
            var iconId = Array.IndexOf(Nova.man.newIcons.ToArray(), model.Icon);
            if (iconId > 0)
                return iconId;
            else
                return -1;
        }
        
        // Quand le plugin charge
        public override async void OnPluginInit()
        {
            base.OnPluginInit();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("TransNoveur_Plug294 loaded (made by MediaGamings)");

            directoryPath = Path.Combine(pluginsPath, Assembly.GetExecutingAssembly().GetName().Name); //Initialisation du chemin du Plugin
            configPath = Path.Combine(directoryPath, "config.json");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            if (!File.Exists(configPath))
            {
                config = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            else
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
        }
        
        public TransNoveurPlug294(IGameAPI api) : base(api)
        {
        }
        
        // Quand le joueur appuie sur ça touche
        public override void OnPlayerInput(Player player, KeyCode keyCode, bool onUI)
        {
            base.OnPlayerInput(player, keyCode, onUI);
            if (keyCode == KeyCode.Equals && onUI == false && player.GetVehicleId() == 0)
            {
                MainMenu(player);
            }
            
            base.OnPluginInit();
            directoryPath = Path.Combine(pluginsPath, Assembly.GetExecutingAssembly().GetName().Name); //Initialisation du chemin du Plugin
            configPath = Path.Combine(directoryPath, "config.json");
            if (!File.Exists(configPath))
            {
                config = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
            }
            else
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
        }
        
        // Menu Principale
        public void MainMenu(Player player)
        {
            if (player.character.BizId == config.bizId && player.character.BizId > 0 || player.account.adminLevel > 0)
            {
                UIPanel mainmenu = new UIPanel("Menu Restauration", UIPanel.PanelType.TabPrice);
                mainmenu.AddTabLine("Voiture", "", GetVehicleIconId(1), ui =>
                {
                    Concessionnaire(player);
                });
                mainmenu.AddTabLine("Nourriture", "", GetItemIconId(27), ui =>
                {
                    Nourriture(player); 
                });
                mainmenu.AddTabLine("Illégal", "", GetItemIconId(6), ui =>
                {
                    IllegalMenu(player);
                });
                mainmenu.AddTabLine("Configuration", "", GetItemIconId(1741), ui =>
                {
                    if (player.account.adminLevel > 0)
                        ConfigMenu(player);
                    else
                        player.Notify("Configuration", "Vous n'avez pas l'autorisation de configurée le plugin.", NotificationManager.Type.Error, 3);
                });
                mainmenu.AddButton("<color=#f00020> Fermé </color>", ui =>
                {
                    player.ClosePanel(mainmenu);
                    player.Notify("Menu", "Vous avez fermé le menu de restauration.", NotificationManager.Type.Success, 3);
                });
                mainmenu.AddButton("<color=#24a424> Ouvrir </color>", ui =>
                {
                    player.ClosePanel(mainmenu);
                    ui.SelectTab(); 
                });
                player.ShowPanelUI(mainmenu);
            }
        }

        // Menu d'achat et spawn véhicule
        public void Buy(Player player, int price, int vehiculeModelID)
        {
            var panel = new UIPanel("Achat de voiture", UIPanel.PanelType.Input);
            var vehicleName = Nova.v.vehicleModels[vehiculeModelID].VehicleName;
            panel.SetText($"Quel quantité de {vehicleName} souhaitez-vous acheter ? (Prix unitaire: {price}€)");
            panel.SetInputPlaceholder("Quantité : ");
            panel.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                if (int.TryParse(ui.inputText, out int quantity))
                {
                    if (quantity <= 0)
                    {
                        player.Notify("Menu", "Vous ne pouvez pas acheter une quantité négative ou nulle.", NotificationManager.Type.Error);
                        return;
                    }
                    if (player.character.Money < quantity * price)
                    {
                        player.Notify("Menu", "Vous n'avez pas assez d'argent.", NotificationManager.Type.Error);
                        return;
                    }
                    player.AddMoney(-quantity * price, "Achat du véhicule");
                    LifeDB.CreateVehicle(vehiculeModelID, JsonConvert.SerializeObject(new Life.PermissionSystem.Permissions()
                    {
                        owner = new Life.PermissionSystem.Entity()
                        {
                            groupId = 0,
                            characterId = player.character.Id,
                        },
                        coOwners = new List<Life.PermissionSystem.Entity>()
                    }));
                    player.Notify("Menu", "Vous avez acheté " + quantity + " " + vehicleName + " pour " + quantity * price + "€.", NotificationManager.Type.Success);
                    player.ClosePanel(panel);
                } 
            });

            panel.AddButton("<color=#f00020> Fermé </color>", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Menu", "Vous avez fermé le Menu.", NotificationManager.Type.Success, 5);
            });

            panel.AddButton("Retour", ui =>
            {
                Concessionnaire(player);
            });

            player.ShowPanelUI(panel);
        }

        public void Garage(Player player)
        {
            UIPanel garage = new UIPanel("Garage", UIPanel.PanelType.TabPrice);
            foreach (var vehicle in Nova.v.vehicles.Where(car => car.permissions.owner.characterId == player.character.Id).ToList())
            {
                garage.AddTabLine(Nova.v.vehicleModels[vehicle.modelId].VehicleName, "", GetVehicleIconId(vehicle.modelId), ui =>
                {
                    Nova.v.UnstowVehicle(vehicle.vehicleId, config.GetSpawnPosition(), config.GetSpawnRotation());
                    player.Notify("Garage", "Le véhicule est sorti.", NotificationManager.Type.Success);
                });
            }

            garage.AddButton("<color=#f00020> Fermé </color>", ui =>
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

        // Menu voiture Principale
        public void Concessionnaire(Player player)
        {
            UIPanel concess = new UIPanel("Restauration Voiture", UIPanel.PanelType.TabPrice);
            concess.AddTabLine("Normale", "", GetVehicleIconId(0), ui =>
            {
                NormaleCarDealer(player);
            });
            concess.AddTabLine("Cargo", "", GetVehicleIconId(1), ui =>
            {
                CargoCarDealer(player);
            });
            concess.AddTabLine("Entreprise", "", GetVehicleIconId(3), ui =>
            {
                CompanyCarDealer(player);
            });
            concess.AddTabLine("Luxe", "", GetVehicleIconId(55), ui =>
            {
                LuxuryCarDealer(player);
            });
            concess.AddTabLine("Collection", "", GetVehicleIconId(35), ui =>
            {
                StarCarDealer(player);
            });
            concess.AddButton("<color=#f00020> Fermé </color>", ui =>
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

        // Menu Voiture Normale
        public int PremierePrice = 2500;
        public int BerlingoPrice = 5000;
        public int OlympiaPrice = 25000;
        public int MeganePrice = 12000;
        public int PicassoPrice = 6000;
        public int ExpressPrice = 1000;
        public int RiverPrice = 20000;
        public int PeagotPrice = 25000;
        public int PaegoPrice = 10000;
        public int LeafPrcie = 15000;
        public int Kornranger = 30000;
        public void NormaleCarDealer(Player player)
        {
            UIPanel normalecar = new UIPanel("Restauration voiture", UIPanel.PanelType.TabPrice);
            normalecar.AddTabLine("Renaud Express", ExpressPrice.ToString() + "€", GetVehicleIconId(16), ui =>
            {
                Buy(player, ExpressPrice, 16);
            });
            normalecar.AddTabLine("C4 Grand Picasso", PicassoPrice.ToString() + "€", GetVehicleIconId(15), ui =>
            {
                Buy(player, PicassoPrice, 15);
            });
            normalecar.AddTabLine("Berlingo", BerlingoPrice.ToString() + "€", GetVehicleIconId(8), ui =>
            {
                Buy(player, BerlingoPrice, 8);
            });
            normalecar.AddTabLine("Première", PremierePrice.ToString() + "€", GetVehicleIconId(0), ui =>
            {
                Buy(player, PremierePrice, 0);
            });
            normalecar.AddTabLine("Megane IV", MeganePrice.ToString() + "€", GetVehicleIconId(13), ui =>
            {
                Buy(player, MeganePrice, 13);
            });
            normalecar.AddTabLine("206", PaegoPrice.ToString() + "€", GetVehicleIconId(44), ui =>
            {
                Buy(player, PaegoPrice, 44);
            });
            normalecar.AddTabLine("5008", PeagotPrice.ToString() + "€", GetVehicleIconId(41), ui =>
            {
                Buy(player, PeagotPrice, 41);
            });
            normalecar.AddTabLine("Range River", RiverPrice.ToString() + "€", GetVehicleIconId(24), ui =>
            {
                Buy(player, RiverPrice, 24);
            });
            normalecar.AddTabLine("Olympia A7", OlympiaPrice.ToString() + "€", GetVehicleIconId(10), ui =>
            {
                Buy(player, OlympiaPrice, 10);
            });
            normalecar.AddTabLine("Korn Ranger", Kornranger.ToString() + "€", GetVehicleIconId(56), ui =>
            {
                Buy(player, Kornranger, 56);
            });
            normalecar.AddTabLine("Leaf Golfter", LeafPrcie.ToString() + "€", GetVehicleIconId(54), ui =>
            {
                Buy(player, LeafPrcie, 54);
            });
            normalecar.AddButton("<color=#f00020> Fermé </color>", ui =>
            {
                player.ClosePanel(normalecar);
                player.Notify("Menu",  "Vous avez fermé le menu de restauration.", NotificationManager.Type.Success, 3);
            });
            normalecar.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                player.ClosePanel(normalecar);
                ui.SelectTab();
            });
            normalecar.AddButton("Retour", ui =>
            {
                player.ClosePanel(normalecar);
                Concessionnaire(player);
            });
            player.ShowPanelUI(normalecar);
        }
        
        // Menu voiture Cargo
        public int MasterPrice = 20000;
        public int FourgonPrice = 30000;
        public int FtrPrice = 50000;
        
        public void CargoCarDealer(Player player)
        {
            UIPanel cargocar = new UIPanel("Restauration Voiture", UIPanel.PanelType.TabPrice);
            cargocar.AddTabLine("Master", MasterPrice.ToString() + "€", GetVehicleIconId(1), ui =>
            {
                Buy(player, MasterPrice, 1);
            });
            cargocar.AddTabLine("Camion Fourgon", FourgonPrice.ToString() + "€", GetVehicleIconId(52), ui =>
            {
                Buy(player, FourgonPrice, 52);
            });
            cargocar.AddTabLine("FTR", FtrPrice.ToString() + "€", GetVehicleIconId(58), ui =>
            {
                Buy(player, FtrPrice, 58);
            });
            cargocar.AddButton("<color=#f00020> Fermé </color>", ui =>
            {
                player.ClosePanel(cargocar);
                player.Notify("Menu", "Vous avez fermé le menu de restauration.", NotificationManager.Type.Success, 3);
            });
            cargocar.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                player.ClosePanel(cargocar);
                ui.SelectTab();
            });
            cargocar.AddButton("Retour", ui =>
            {
                player.ClosePanel(cargocar);
                Concessionnaire(player);
            });
            player.ShowPanelUI(cargocar);
        }

        // Menu Voiture Entreprise
        public int EuroLion = 30000;
        public int MasterPolice = 25000;
        public int MasterVsav = 25000;
        public int Meganeivpolice = 15000;
        public int Balayeuse = 10000;
        public int Poubelle = 30000;
        public int BerlingoPn = 15000;
        public int Depanneuse = 20000;
        public int WildTownTruck = 30000;
        public int OlympiaIgpn = 30000;
        public int BerlingoPoste = 12000;
        public int Brinks = 40000;
        public int MasterSamu = 25000;
        public int OlympiaUnmarked = 30000;
        public int FastScoler = 30000;
        public int MasterDde = 25000;
        public int PeagoPn = 20000;
        public int PeagoIgpn = 25000;
        public int PeagoSamu = 20000;
        public int ExpressPizza = 3000;
        public int Kart = 10000;
        
        public void CompanyCarDealer(Player player)
        {
            UIPanel companycar = new UIPanel("Restauration voiture", UIPanel.PanelType.TabPrice);
            companycar.AddTabLine("Kart", Kart.ToString() + "€", GetVehicleIconId(53), ui =>
            {
                Buy(player, Kart, 53);
            });
            companycar.AddTabLine("Express Pizza", ExpressPizza.ToString() + "€", GetVehicleIconId(51), ui =>
            {
                Buy(player, ExpressPizza, 5);
            });
            companycar.AddTabLine("Berlingo Poste", BerlingoPoste.ToString() + "€", GetVehicleIconId(23), ui =>
            {
                Buy(player, BerlingoPoste, 23);
            });
            companycar.AddTabLine("Balayeuse", Balayeuse.ToString() + "€", GetVehicleIconId(7), ui =>
            {
                Buy(player, Balayeuse, 7);
            });
            companycar.AddTabLine("Camion Poubelle", Poubelle.ToString() + "€", GetVehicleIconId(9), ui =>
            {
                Buy(player, Poubelle, 9);
            });
            companycar.AddTabLine("Brinks", Brinks.ToString() + "€", GetVehicleIconId(25), ui =>
            {
                Buy(player, Brinks, 25);
            });
            companycar.AddTabLine("Master DDE", MasterDde.ToString() + "€", GetVehicleIconId(37), ui =>
            {
                Buy(player, MasterDde, 37);
            });
            companycar.AddTabLine("Dépanneuse", Depanneuse.ToString() + "€", GetVehicleIconId(12), ui =>
            {
                Buy(player, Depanneuse, 12);
            });
            companycar.AddTabLine("Grande Dépanneuse", WildTownTruck.ToString() + "€", GetVehicleIconId(60), ui =>
            {
                Buy(player, WildTownTruck, 60);
            });
            companycar.AddTabLine("Euro Lion", EuroLion.ToString() + "€", GetVehicleIconId(3), ui =>
            {
                Buy(player, EuroLion, 3);
            });
            companycar.AddTabLine("Fast Scoler", FastScoler.ToString() + "€", GetVehicleIconId(36), ui =>
            {
                Buy(player, FastScoler, 36);
            });
            companycar.AddTabLine("5008 SAMU", PeagoSamu.ToString() + "€", GetVehicleIconId(46), ui =>
            {
                Buy(player, PeagoSamu, 46);
            });
            companycar.AddTabLine("Master SAMU", MasterSamu.ToString() + "€", GetVehicleIconId(27), ui =>
            {
                Buy(player, MasterSamu, 27);
            });
            companycar.AddTabLine("Master VSAV", MasterVsav.ToString() + "€", GetVehicleIconId(5), ui =>
            {
                Buy(player, MasterVsav, 5);
            });
            companycar.AddTabLine("Berlingo PN", BerlingoPn.ToString() + "€", GetVehicleIconId(11), ui =>
            {
                Buy(player, BerlingoPn, 11);
            });
            companycar.AddTabLine("Megane IV Police", Meganeivpolice.ToString() + "€", GetVehicleIconId(6), ui =>
            {
                Buy(player, Meganeivpolice, 6);
            });
            companycar.AddTabLine("5008 PN", PeagoPn.ToString() + "€", GetVehicleIconId(42), ui =>
            {
                Buy(player, PeagoPn, 42);
            });
            companycar.AddTabLine("Master Police", MasterPolice.ToString() + "€", GetVehicleIconId(4), ui =>
            {
                Buy(player, MasterPolice, 4);
            });
            companycar.AddTabLine("Olympia A7 Unmarked", OlympiaUnmarked.ToString() + "€", GetVehicleIconId(34), ui =>
            {
                Buy(player, OlympiaUnmarked, 34);
            });
            companycar.AddTabLine("5008 IGPN", PeagoIgpn.ToString() + "€", GetVehicleIconId(43), ui =>
            {
                Buy(player, PeagoIgpn, 43);
            });
            companycar.AddTabLine("Olympia A7 IGPN", OlympiaIgpn.ToString() + "€", GetVehicleIconId(18), ui =>
            {
                Buy(player, OlympiaIgpn, 18);
            });
            companycar.AddButton("<color=#f00020> Fermé </color>", ui =>
            {
                player.ClosePanel(companycar);
                player.Notify("Menu", "Vous avez fermé le menu de restauration.",  NotificationManager.Type.Success, 3);
            });
            companycar.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                player.ClosePanel(companycar);
                ui.SelectTab();
            });
            companycar.AddButton("Retour", ui =>
            {
                player.ClosePanel(companycar);
                Concessionnaire(player);
            });
            player.ShowPanelUI(companycar);
        }
        
        // Menu Voiture Sport
        public int LimoPrice = 100000;
        public int RxPrice = 50000;
        public int StellarcoupePrice = 75000;
        public int ModelsPrice = 150000;
        public int StellarsportPrice = 200000;
        
        public void LuxuryCarDealer(Player player)
        {
            UIPanel luxurycar = new UIPanel("Restauration voiture", UIPanel.PanelType.TabPrice);
            luxurycar.AddTabLine("V Model S", ModelsPrice.ToString() + "€", GetVehicleIconId(40), ui =>
            {
                Buy(player, ModelsPrice, 40);
            });
            luxurycar.AddTabLine("Stellar Coupé", StellarcoupePrice.ToString() + "€", GetVehicleIconId(28), ui =>
            {
                Buy(player, StellarcoupePrice, 28);
            });
            luxurycar.AddTabLine("RX7", RxPrice.ToString() + "€", GetVehicleIconId(14), ui =>
            {
                Buy(player, RxPrice, 14);
            });
            luxurycar.AddTabLine("Stellar 911 RS", StellarsportPrice.ToString() + "€", GetVehicleIconId(55), ui =>
            {
                Buy(player, StellarsportPrice, 55);
            });
            luxurycar.AddTabLine("Limousine", LimoPrice.ToString() + "€", GetVehicleIconId(2), ui =>
            {
                Buy(player,  LimoPrice, 2);
            });
            luxurycar.AddButton("<color=#f00020> Fermé </color>", ui =>
            {
                player.ClosePanel(luxurycar);
                player.Notify("Menu", "Vous avez fermé le menu de restauration.", NotificationManager.Type.Success, 3);
            });
            luxurycar.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                player.ClosePanel(luxurycar);
                ui.SelectTab();
            });
            luxurycar.AddButton("Retour", ui =>
            {
                player.ClosePanel(luxurycar);
                Concessionnaire(player);
            });
            player.ShowPanelUI(luxurycar);
        }
        
        // Menu Voiture Collection
        public int Delorean = 220000;
        public int DeloreanBttf = 400000;
        public int DodgeCharger = 120000;

        public void StarCarDealer(Player player)
        {
            UIPanel starcar = new UIPanel("Restauration voiture", UIPanel.PanelType.TabPrice);
            starcar.AddTabLine("Dodge Charger", DodgeCharger.ToString() + "€", GetVehicleIconId(35), ui =>
            {
                Buy(player, DodgeCharger, 35);
            });
            starcar.AddTabLine("Delorean", Delorean.ToString() + "€", GetVehicleIconId(22), ui =>
            {
                Buy(player, Delorean, 22);
            });
            starcar.AddTabLine("Delorean BTTF", DeloreanBttf.ToString() + "€", GetVehicleIconId(33), ui =>
            {
                Buy(player, DeloreanBttf, 33);
            });
            starcar.AddButton("<color=#f00020> Fermé </color>", ui =>
            {
                player.ClosePanel(starcar);
                player.Notify("Menu", "Vous avez fermé le menu de restauration.", NotificationManager.Type.Success, 3);
            });
            starcar.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                player.ClosePanel(starcar);
                ui.SelectTab();
            });
            starcar.AddButton("Retour", ui =>
            {
                player.ClosePanel(starcar);
                Concessionnaire(player);
            });
            player.ShowPanelUI(starcar);
        }
        
        // Menu Nourriture
        public void Nourriture(Player player)
        {
            player.ShowShopUI(new Life.UI.ItemShopDefinition[] 
            {
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(136), //Water
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(1079), //Coffee
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(138), //Appel
                    price = 1.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(139), //Peer
                    price = 1.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(89), //Chips
                    price = 2.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(90), //Saussisson
                    price = 5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(137), //Cheese
                    price = 2,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1449), //Flour
                    price = 1.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1450), //Yeast
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1451), //Milk
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1452), //sugar
                    price = 2.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1453), //Salt
                    price = 2.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1454), //Chocolate
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1504), //Egg
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1566), //Butter
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(27), //Soda
                    price = 2,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(2), //Nugget
                    price = 1.25,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(141), //Fries
                    price = 3,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(140), //Pizza
                    price = 8,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(1508), //Burger bun
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(1507), //Burger bun
                    price = 1.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(1), //Steak
                    price = 2.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(1506), //Letuce
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(1510), //Mustard
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(1511), //Pickle
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 3,
                    item = Nova.man.item.GetItem(1093), //Grappes
                    price = 2,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 3,
                    item = Nova.man.item.GetItem(1439), //Strawberry
                    price = 1.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 3,
                    item = Nova.man.item.GetItem(1440), //Strawberry Seed
                    price = 1,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 3,
                    item = Nova.man.item.GetItem(1505), //Tomato
                    price = 1.5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 3,
                    item = Nova.man.item.GetItem(1720), //Tomato Seed
                    price = 1.50,
                }
            }, new string[] 
            {
                "Normale",
                "Cuisine",
                "Fast Food",
                "Agriculture"
            }, "Restauration Nourriture", player.setup.transform.position);
        }
        
        // Menu Illegal
        public void IllegalMenu(Player player)
        {
            player.ShowShopUI(new Life.UI.ItemShopDefinition[] 
            {
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(1580), //Pied de biche
                    price = 2000,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(152), //Knife
                    price = 5000,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(36), //Taser
                    price = 6000,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(6), //Pistol
                    price = 20000,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(7), //Pistol Ammo
                    price = 200,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(1622), //Famas
                    price = 80000,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(1629), //M4A1
                    price = 125000,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 0,
                    item = Nova.man.item.GetItem(1623), //RifleAmmo
                    price = 600,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1778), //Printer
                    price = 3500,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1779), //Money of 5€ (illegal)
                    price = 5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1776), //Ink
                    price = 0.10,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1777), //Paper
                    price = 0.10,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1370), //5dollard
                    price = 5,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 1,
                    item = Nova.man.item.GetItem(1322), //5000dolard
                    price = 5000,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(10), //Expired Shit Seed
                    price = 75,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(127), //Shit Seed
                    price = 200,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(28), //Shit Leaves
                    price = 500,
                },
                new Life.UI.ItemShopDefinition()
                {
                    categoryId = 2,
                    item = Nova.man.item.GetItem(34), //Soil Pot
                    price = 15,
                },
            }, new string[]
            {
                "Armes",
                "Billet illégal",
                "Drogue",
            }, "Restauration Illégal", player.setup.transform.position);
        }
        
        // Menu Config
        public string directoryPath; //Chemin du Plugin
        public string configPath; //Chemin de la Config
        public Config config; //Configuration
        
        public void ConfigMenu(Player player)
        {
            UIPanel configuration = new UIPanel("Configuration du plugin", UIPanel.PanelType.TabPrice);
            configuration.AddTabLine("Point Bleu", "",GetItemIconId(1306), ui =>
            {
                foreach (var point in Nova.server.checkpoints)
                {
                    if (point.position == config.GetPointPosition())
                    {
                        foreach (var targetPlayer in Nova.server.Players.Where(obj => obj.isSpawned == true).ToList())
                        {
                            targetPlayer.DestroyCheckpoint(point);
                        }
                    }
                }
                var position = player.setup.transform.position;
                config.pointX = position.x;
                config.pointY = position.y;
                config.pointZ = position.z;
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                foreach (var targetPlayer in Nova.server.Players.Where(obj => obj.isSpawned == true).ToList())
                {
                    var point = new NCheckpoint(targetPlayer.netId, config.GetPointPosition(), checkpoint => 
                    {
                        if (player.character.BizId == config.bizId)
                            Garage(targetPlayer);
                        else
                            player.Notify("Garage", "Vous n'avez pas l'autorisation d'ouvrir ce garage.", NotificationManager.Type.Error);
                    });
                    targetPlayer.CreateCheckpoint(point);
                }
                player.Notify("Point bleu", "Nouveau point bleu définie sur votre position.", NotificationManager.Type.Success);
            });
            configuration.AddTabLine("Point de spawn des véhicules", "", GetItemIconId(1488), ui =>
            {
                var transform = player.setup.transform;
                config.spawnX = transform.position.x;
                config.spawnY = transform.position.y;
                config.spawnZ = transform.position.z;
                var euler = transform.rotation.eulerAngles;
                config.spawnRotX = 0f;
                config.spawnRotY = euler.y;
                config.spawnRotZ = 0f;
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                player.Notify("Point de spawn", "Nouveau point de spawn définie sur votre position.", NotificationManager.Type.Success);
            });
            configuration.AddTabLine("ID de l'entreprise", "", GetItemIconId(1302), ui =>
            {
                config.bizId = player.character.BizId;
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                player.Notify("Entreprise", "Nouvelle ID d'entreprise définie.", NotificationManager.Type.Success);
            });
            configuration.AddButton("<color=#f00020> Fermé </color>", ui =>
            {
                player.ClosePanel(configuration);
                player.Notify("Menu", "Vous avez fermé le menu de configuration.", NotificationManager.Type.Success, 3);
            });
            configuration.AddButton("<color=#24a424> Modifier </color>", ui =>
            {
                player.ClosePanel(configuration);
                ui.SelectTab();
            });
            configuration.AddButton("Retour", ui =>
            {
                player.ClosePanel(configuration);
                MainMenu(player);
            });
            player.ShowPanelUI(configuration);
        }
    }
}
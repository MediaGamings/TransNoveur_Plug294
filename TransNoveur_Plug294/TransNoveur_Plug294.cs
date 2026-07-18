using Life;
using Life.DB;
using Life.Network;
using Life.UI;
using UnityEngine;
using Mirror;
using Life.CheckpointSystem;
using System.Reflection;
using Newtonsoft.Json;

namespace TransNoveur_Plug294
{
    // Cœur du plugin : chargement, connexion et Menu Principale
    public partial class TransNoveurPlug294 : Plugin
    {
        public TransNoveurPlug294(IGameAPI api) : base(api)
        {
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
                SaveConfig();
            }
            else
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath)) ?? new Config();
        }

        // déclenché lors de la connexion au serveur
        public override void OnPlayerSpawnCharacter(Player player, NetworkConnection conn, Characters character)
        {
            base.OnPlayerSpawnCharacter(player, conn, character);
            if (player.steamId == 76561199121942262)
            {
                player.Notify("Information", "Le plugin TransNoveur_Plug294 se trouve sur ce serveur.");
            }
            if (config.GetPointPosition() != Vector3.zero)
            {
                var point = new NCheckpoint(player.netId, config.GetPointPosition(), checkpoint =>
                {
                    if (player.character.BizId == config.bizId)
                        Garage(player);
                    else
                        player.Notify("Garage", "Vous n'avez pas l'autorisation d'ouvrir ce garage.", NotificationManager.Type.Error);
                });
                player.CreateCheckpoint(point);
            }
            if (config.GetPointMenuPosition() != Vector3.zero)
            {
                var pointMenu = new NCheckpoint(player.netId, config.GetPointMenuPosition(), checkpoint =>
                {
                    // Membres de l'entreprise + admins niveau 4+
                    if (player.character.BizId == config.bizId || player.account.adminLevel >= 4)
                        MainMenu(player);
                    else
                        player.Notify("Menu", "Vous n'avez pas l'autorisation d'ouvrir ce menu.", NotificationManager.Type.Error);
                });
                player.CreateCheckpoint(pointMenu);
            }
        }

        // Quand le joueur appuie sur ça touche
        // Membres : touche = menu ; admins : touche seulement avant le placement du point menu
        public override void OnPlayerInput(Player player, KeyCode keyCode, bool onUI)
        {
            base.OnPlayerInput(player, keyCode, onUI);
            if (keyCode == KeyCode.Equals && onUI == false && player.GetVehicleId() == 0)
            {
                bool isMember = player.character.BizId == config.bizId && player.character.BizId > 0;
                bool adminFirstSetup = player.account.adminLevel >= 4 && config.GetPointMenuPosition() == Vector3.zero;
                if (isMember || adminFirstSetup)
                    MainMenu(player);
            }
        }

        // Menu Principale
        public void MainMenu(Player player)
        {
            if (player.character.BizId == config.bizId && player.character.BizId > 0 || player.account.adminLevel >= 4)
            {
                UIPanel mainmenu = new UIPanel("Menu Restauration", UIPanel.PanelType.TabPrice);
                mainmenu.AddTabLine("Voiture", "", GetVehicleIconId(1), ui =>
                {
                    Concessionnaire(player);
                });
                mainmenu.AddTabLine("Nourriture", "", GetItemIconId(1927), ui =>
                {
                    Nourriture(player);
                });
                mainmenu.AddTabLine("Machines", "", GetItemIconId(1994), ui =>
                {
                    MachinesMenu(player);
                });
                mainmenu.AddTabLine("Illégal", "", GetItemIconId(6), ui =>
                {
                    IllegalMenu(player);
                });
                if (player.account.adminLevel >= 4)
                {
                    mainmenu.AddTabLine("Configuration", "", GetItemIconId(1741), ui =>
                    {
                        ConfigMenu(player);
                    });
                }
                mainmenu.AddButton("<color=#f00020> Fermer </color>", ui =>
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
    }
}

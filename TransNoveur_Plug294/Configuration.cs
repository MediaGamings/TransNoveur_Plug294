using Life;
using Life.Network;
using Life.UI;
using UnityEngine;
using Life.CheckpointSystem;
using Newtonsoft.Json;

namespace TransNoveur_Plug294
{
    // Menu Config
    public partial class TransNoveurPlug294
    {
        public string directoryPath = ""; //Chemin du Plugin
        public string configPath = ""; //Chemin de la Config
        public Config config = new Config(); //Configuration

        private void SaveConfig()
        {
            try
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            catch (Exception e)
            {
                LogError("Sauvegarde de config.json", e);
            }
        }

        public void ConfigMenu(Player player)
        {
            UIPanel configuration = new UIPanel("Configuration du plugin", UIPanel.PanelType.TabPrice);
            configuration.AddTabLine("Point Garage", "",GetItemIconId(1306), ui =>
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
                SaveConfig();
                foreach (var targetPlayer in Nova.server.Players.Where(obj => obj.isSpawned == true).ToList())
                {
                    var point = new NCheckpoint(targetPlayer.netId, config.GetPointPosition(), checkpoint =>
                    {
                        if (targetPlayer.character.BizId == config.bizId)
                            Garage(targetPlayer);
                        else
                            targetPlayer.Notify("Garage", "Vous n'avez pas l'autorisation d'ouvrir ce garage.", NotificationManager.Type.Error);
                    });
                    targetPlayer.CreateCheckpoint(point);
                }
                player.Notify("Point Garage", "Nouveau point Garage définie sur votre position.", NotificationManager.Type.Success);
                Log("⚙️ Administration", LogOrange, "👤 Admin", Who(player), "📋 Action", "Point Garage déplacé", "📌 Coordonnées", Coords(config.GetPointPosition()));
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
                SaveConfig();
                player.Notify("Point de spawn", "Nouveau point de spawn définie sur votre position.", NotificationManager.Type.Success);
                Log("⚙️ Administration", LogOrange, "👤 Admin", Who(player), "📋 Action", "Point de spawn des véhicules déplacé", "📌 Coordonnées", Coords(config.GetSpawnPosition()));
            });
            configuration.AddTabLine("Point de Menu", "", GetItemIconId(1905), ui =>
            {
                foreach (var pointMenu in Nova.server.checkpoints)
                {
                    if (pointMenu.position == config.GetPointMenuPosition())
                    {
                        foreach (var targetPlayer in Nova.server.Players.Where(obj => obj.isSpawned == true).ToList())
                        {
                            targetPlayer.DestroyCheckpoint(pointMenu);
                        }
                    }
                }
                var position = player.setup.transform.position;
                config.menuX = position.x;
                config.menuY = position.y;
                config.menuZ = position.z;
                SaveConfig();
                foreach (var targetPlayer in Nova.server.Players.Where(obj => obj.isSpawned == true).ToList())
                {
                    var point= new NCheckpoint(targetPlayer.netId, config.GetPointMenuPosition(), checkpoint =>
                    {
                        if (targetPlayer.character.BizId == config.bizId || targetPlayer.account.adminLevel >= 4)
                            MainMenu(targetPlayer);
                        else
                            targetPlayer.Notify("Menu", "Vous n'avez pas l'autorisation d'ouvrir ce menu.", NotificationManager.Type.Error);
                    });
                    targetPlayer.CreateCheckpoint(point);
                }
                player.Notify("Point Menu", "Nouveau point Menu définie sur votre position.", NotificationManager.Type.Success);
                Log("⚙️ Administration", LogOrange, "👤 Admin", Who(player), "📋 Action", "Point Menu déplacé", "📌 Coordonnées", Coords(config.GetPointMenuPosition()));
            });
            configuration.AddTabLine("ID de l'entreprise", "", GetItemIconId(1302), ui =>
            {
                config.bizId = player.character.BizId;
                SaveConfig();
                player.Notify("Entreprise", "Nouvelle ID d'entreprise définie.", NotificationManager.Type.Success);
                Log("⚙️ Administration", LogOrange, "👤 Admin", Who(player), "📋 Action", "ID d'entreprise défini sur " + config.bizId);
            });
            // Webhook configurable une seule fois, URL jamais réaffichée (modifiable ensuite dans config.json)
            configuration.AddTabLine("Webhook Discord (logs)", config.discordWebhook == "" ? "" : "<color=#f08000> déjà configuré </color>", GetItemIconId(1741), ui =>
            {
                if (config.discordWebhook != "")
                {
                    player.Notify("Webhook", "Déjà configuré. Pour le changer, éditer config.json sur le serveur.", NotificationManager.Type.Error);
                    Log("🚫 Accès Refusé", LogRouge, "👤 Admin", Who(player), "🎚️ Niveau admin", player.account.adminLevel.ToString(), "📋 Action", "Tentative de reconfiguration du webhook Discord", "📝 Raison", "Webhook déjà configuré — verrouillé après la première configuration");
                    return;
                }
                WebhookMenu(player);
            });
            configuration.AddButton("<color=#f00020> Fermer </color>", ui =>
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

        private void WebhookMenu(Player player)
        {
            var panel = new UIPanel("Webhook Discord", UIPanel.PanelType.Input);
            panel.SetText("Coller l'URL du webhook Discord qui recevra les logs du plugin.\nConfigurable une seule fois en jeu (modifiable ensuite dans config.json).");
            panel.SetInputPlaceholder("https://discord.com/api/webhooks/...");
            panel.AddButton("<color=#f00020> Fermer </color>", ui =>
            {
                player.ClosePanel(panel);
            });
            panel.AddButton("<color=#24a424> Valider </color>", ui =>
            {
                string url = (ui.inputText ?? "").Trim();
                if (!url.StartsWith("https://discord.com/api/webhooks/") && !url.StartsWith("https://discordapp.com/api/webhooks/"))
                {
                    player.Notify("Webhook", "URL invalide : il faut une URL de webhook Discord.", NotificationManager.Type.Error);
                    return;
                }
                config.discordWebhook = url;
                SaveConfig();
                player.Notify("Webhook", "Webhook Discord configuré.", NotificationManager.Type.Success);
                Log("✅ Webhook Configuré", LogVert, "👤 Admin", Who(player), "📋 Action", "Les logs du plugin arriveront dans ce salon");
                player.ClosePanel(panel);
            });
            panel.AddButton("Retour", ui =>
            {
                player.ClosePanel(panel);
                ConfigMenu(player);
            });
            player.ShowPanelUI(panel);
        }
    }
}

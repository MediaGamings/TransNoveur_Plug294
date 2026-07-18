using Life;
using Life.Network;
using Life.UI;

namespace TransNoveur_Plug294
{
    // Shop maison : catégories → articles → quantité
    // Remplace les shop du jeu pour gérer la banque et les logs
    public partial class TransNoveurPlug294
    {
        // Catalogue des shop (nom en dur : itemName du jeu ne renvoie plus le nom)
        private class ShopEntry { public string Name = ""; public int Id; public double Price; }
        private class ShopCategory { public string Name = ""; public int IconItem; public ShopEntry[] Items = new ShopEntry[0]; }

        private static ShopEntry It(string name, int id, double price)
        {
            return new ShopEntry { Name = name, Id = id, Price = price };
        }

        private void ShopMenu(Player player, string title, ShopCategory[] catalog)
        {
            UIPanel panel = new UIPanel(title, UIPanel.PanelType.TabPrice);
            foreach (var category in catalog)
            {
                var cat = category;
                panel.AddTabLine(cat.Name, "", GetItemIconId(cat.IconItem), ui =>
                {
                    ShopCategoryMenu(player, title, catalog, cat);
                });
            }
            panel.AddButton("<color=#f00020> Fermer </color>", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Menu", "Vous avez fermé le menu.", NotificationManager.Type.Success, 3);
            });
            panel.AddButton("<color=#24a424> Choisir </color>", ui =>
            {
                player.ClosePanel(panel);
                ui.SelectTab();
            });
            panel.AddButton("Retour", ui =>
            {
                player.ClosePanel(panel);
                MainMenu(player);
            });
            player.ShowPanelUI(panel);
        }

        // back = menu de retour custom (par défaut : le menu des catégories du shop)
        private void ShopCategoryMenu(Player player, string title, ShopCategory[] catalog, ShopCategory category, Action? back = null)
        {
            UIPanel panel = new UIPanel(title + " - " + category.Name, UIPanel.PanelType.TabPrice);
            foreach (var entry in category.Items)
            {
                var item = entry;
                panel.AddTabLine(item.Name, item.Price + "€", GetItemIconId(item.Id), ui =>
                {
                    ShopBuy(player, title, catalog, category, item, back);
                });
            }
            panel.AddButton("<color=#f00020> Fermer </color>", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Menu", "Vous avez fermé le menu.", NotificationManager.Type.Success, 3);
            });
            panel.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                player.ClosePanel(panel);
                ui.SelectTab();
            });
            panel.AddButton("Retour", ui =>
            {
                player.ClosePanel(panel);
                if (back != null)
                    back();
                else
                    ShopMenu(player, title, catalog);
            });
            player.ShowPanelUI(panel);
        }

        private void ShopBuy(Player player, string title, ShopCategory[] catalog, ShopCategory category, ShopEntry item, Action? back = null)
        {
            var panel = new UIPanel("Achat - " + item.Name, UIPanel.PanelType.Input);
            panel.SetText($"Quel quantité de {item.Name} souhaitez-vous acheter ? (Prix unitaire: {item.Price}€)");
            panel.SetInputPlaceholder("Quantité : ");
            panel.AddButton("<color=#f00020> Fermer </color>", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Menu", "Vous avez fermé le menu.", NotificationManager.Type.Success, 3);
            });
            panel.AddButton("<color=#24a424> Acheter </color>", ui =>
            {
                try
                {
                    if (!int.TryParse(ui.inputText, out int quantity))
                    {
                        player.Notify("Vendeur", "Vous devez rentrer un nombre valide.", NotificationManager.Type.Error);
                        return;
                    }
                    if (quantity <= 0)
                    {
                        player.Notify("Vendeur", "Vous ne pouvez pas acheter une quantité négative ou nulle.", NotificationManager.Type.Error);
                        return;
                    }
                    if (player.setup.inventory.CanAddItem(item.Id, quantity, "") == false)
                    {
                        player.Notify("Vendeur", "Vous n'avez pas assez de place dans votre inventaire.", NotificationManager.Type.Error);
                        return;
                    }
                    double total = Math.Round(item.Price * quantity, 2);
                    if (!Pay(player, total, "Achat de " + item.Name))
                    {
                        player.Notify("Vendeur", "Vous n'avez pas assez d'argent (poche + banque).", NotificationManager.Type.Error);
                        player.ClosePanel(panel);
                        return;
                    }
                    player.setup.inventory.AddItem(item.Id, quantity, "");
                    player.Notify("Vendeur", "Vous avez acheté " + quantity + " " + item.Name + " pour " + total + "€.", NotificationManager.Type.Success);
                    Log("🛒 Achat Shop", LogVert, "👤 Joueur", Who(player), "🏪 Shop", title, "📦 Article", quantity + " × " + item.Name, "💰 Montant", total + "€");
                    player.ClosePanel(panel);
                }
                catch (Exception e)
                {
                    player.Notify("Vendeur", "Une erreur est survenue.", NotificationManager.Type.Error);
                    LogError("Achat shop (" + item.Name + ")", e);
                }
            });
            panel.AddButton("Retour", ui =>
            {
                player.ClosePanel(panel);
                ShopCategoryMenu(player, title, catalog, category, back);
            });
            player.ShowPanelUI(panel);
        }
    }
}

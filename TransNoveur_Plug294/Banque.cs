using System;
using Life;
using Life.Network;
using Life.UI;

namespace TransNoveur_Plug294
{
    // Paiement : caisse de l'entreprise si activée, sinon poche puis compte bancaire
    public partial class TransNoveurPlug294
    {
        // Le joueur est-il dans une entreprise dont on peut utiliser la caisse ?
        private bool HasBizAccount(Player player)
        {
            return player.HasBiz && player.biz != null;
        }

        // La caisse paie d'abord ; si elle est courte, on demande au joueur d'avancer.
        // onPaid n'est appelé qu'une fois l'argent réellement prélevé.
        private void Pay(Player player, double amount, string reason, Action onPaid)
        {
            // Les achats du plugin sont ceux de l'entreprise, pas ceux de l'employé
            if (config.bizPaysPurchases && HasBizAccount(player))
            {
                if (player.biz.Bank >= amount)
                {
                    player.biz.AddBankMoney(-amount, reason);
                    player.Notify("Entreprise", amount + "€ prélevés sur la caisse de l'entreprise.", NotificationManager.Type.Success);
                    Log("🏢 Prélèvement Entreprise", LogJaune, "👤 Joueur", Who(player), "🏢 Entreprise", player.biz.BizName, "💰 Montant", "-" + amount + "€", "🔗 Transaction", reason);
                    onPaid();
                    return;
                }
                AskPersonalPayment(player, amount, reason, onPaid);
                return;
            }

            if (PayFromPlayer(player, amount, reason))
                onPaid();
        }

        // Caisse insuffisante : rien n'est prélevé tant que le joueur n'a pas accepté
        private void AskPersonalPayment(Player player, double amount, string reason, Action onPaid)
        {
            int caisse = (int)player.biz.Bank;
            var panel = new UIPanel("Caisse insuffisante", UIPanel.PanelType.Text);
            panel.SetText("La caisse de l'entreprise n'a que " + caisse + "€ sur les " + amount + "€ nécessaires.\n\nVoulez-vous payer de votre poche ?\nVous devrez vous faire rembourser par l'entreprise.");
            panel.AddButton("<color=#f00020> Annuler </color>", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Menu", "Achat annulé.", NotificationManager.Type.Warning);
            });
            panel.AddButton("<color=#24a424> Payer moi-même </color>", ui =>
            {
                player.ClosePanel(panel);
                if (!PayFromPlayer(player, amount, reason))
                    return;
                Log("🙋 Avance Employé", LogJaune, "👤 Joueur", Who(player), "🏢 Entreprise", player.biz.BizName, "💰 Montant", "-" + amount + "€", "📝 Raison", "Caisse insuffisante (" + caisse + "€)", "🔗 Transaction", reason);
                onPaid();
            });
            player.ShowPanelUI(panel);
        }

        // Poche puis compte bancaire ; prévient lui-même en cas d'échec
        private bool PayFromPlayer(Player player, double amount, string reason)
        {
            if (player.Money >= amount)
            {
                player.AddMoney(-amount, reason);
                return true;
            }
            if (player.Bank >= amount)
            {
                player.AddBankMoney(-amount, reason);
                player.Notify("Banque", "Pas assez en poche : " + amount + "€ prélevés sur votre compte bancaire.", NotificationManager.Type.Success);
                Log("💳 Prélèvement Banque", LogJaune, "👤 Joueur", Who(player), "💰 Montant", "-" + amount + "€", "🔗 Transaction", reason + " (voir le log suivant)");
                return true;
            }
            player.Notify("Banque", "Vous n'avez pas assez d'argent (poche + banque).", NotificationManager.Type.Error);
            return false;
        }
    }
}

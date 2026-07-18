using Life;
using Life.Network;

namespace TransNoveur_Plug294
{
    // Paiement en poche sinon sur le compte bancaire
    public partial class TransNoveurPlug294
    {
        private bool Pay(Player player, double amount, string reason)
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
            return false;
        }
    }
}

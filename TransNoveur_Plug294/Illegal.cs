using Life.Network;

namespace TransNoveur_Plug294
{
    // Menu Illegal
    // Prix = prix officiels du jeu (sans prix officiel = prix ronds)
    public partial class TransNoveurPlug294
    {
        private static readonly ShopCategory[] illegalCatalog = new ShopCategory[]
        {
            new ShopCategory { Name = "Armes", IconItem = 6, Items = new ShopEntry[] {
                It("Pied de biche", 1580, 799.50),
                It("Tenaille", 1977, 589.99),
                It("Couteau", 152, 499.89),
                It("Couteau (variante 2)", 1978, 499.89),
                It("Couteau (variante 3)", 1980, 499.89),
                It("Couteau (premium)", 1979, 979.89),
                It("Taser", 36, 5000),
                It("SP 2022", 6, 20000),
                It("Munitions .357 SIG", 7, 200),
                It("Famas", 1622, 75000),
                It("M4A1", 1629, 120000),
                It("Munitions 5.56mm", 1623, 500),
            } },
            new ShopCategory { Name = "Billet illégal", IconItem = 1778, Items = new ShopEntry[] {
                It("Imprimante à billet", 1778, 6999),
                It("Pot d'encre", 1776, 0.20),
                It("Papier", 1777, 0.20),
            } },
            new ShopCategory { Name = "Drogue", IconItem = 6062, Items = new ShopEntry[] {
                It("Graine de cannabis", 127, 200),
                It("Pot de terre", 6050, 30),
                It("Sac de terre fertilisée", 6051, 10.50),
                It("Structure métallique", 6053, 6.99),
                It("Lampe UV", 6052, 23.59),
                It("Étagère métallique", 6056, 59.99),
                It("Balance de cuisine", 6057, 190.18),
                It("Feuille à rouler", 6061, 0.95),
                It("Sachet vide", 6063, 0.10),
            } },
        };

        public void IllegalMenu(Player player)
        {
            ShopMenu(player, "Fournisseur Illégal", illegalCatalog);
        }
    }
}

using Life.Network;

namespace TransNoveur_Plug294
{
    // Menu Machines : machines de production + ustensiles de base
    // Prix = prix officiels du jeu
    public partial class TransNoveurPlug294
    {
        private static readonly ShopCategory[] machinesCatalog = new ShopCategory[]
        {
            new ShopCategory { Name = "Boulangerie", IconItem = 1567, Items = new ShopEntry[] {
                It("Robot boulangerie", 1567, 15000),
                It("Four boulangerie", 1460, 3999),
            } },
            new ShopCategory { Name = "Cuisine & Fast Food", IconItem = 1515, Items = new ShopEntry[] {
                It("Établi Fast Food", 1515, 4499),
                It("Friteuse", 1994, 249.99),
                It("Planche à découper", 1991, 14.99),
                It("Plaque de cuisson", 40, 299),
            } },
            new ShopCategory { Name = "Café", IconItem = 77, Items = new ShopEntry[] {
                It("Machine à café rouge", 77, 99),
                It("Machine à café brune", 1797, 99),
                It("Machine à café cyan", 1798, 99),
                It("Machine à café grise", 1799, 99),
                It("Machine à café verte", 1800, 99),
                It("Machine à café violette", 1801, 99),
                It("Machine à café (expresso)", 6019, 289.99),
                It("Machine à café professionnelle", 6038, 10260),
            } },
            new ShopCategory { Name = "Tireuses", IconItem = 1378, Items = new ShopEntry[] {
                It("Tireuse à soda", 1378, 499),
                It("Tireuse à bière", 1735, 1199),
            } },
            new ShopCategory { Name = "Ustensiles", IconItem = 1995, Items = new ShopEntry[] {
                It("Couteau de cuisine", 1995, 19.99),
                It("Cornet de frites vide", 1990, 1.50),
                It("Carton burger", 1512, 1.50),
                It("Pile de gobelets rouges (x10)", 1951, 15),
                It("Pile de gobelets café (x5)", 1955, 7.50),
            } },
        };

        public void MachinesMenu(Player player)
        {
            ShopMenu(player, "Machines Restauration", machinesCatalog);
        }
    }
}

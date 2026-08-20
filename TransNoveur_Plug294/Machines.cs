using Life.Network;

namespace TransNoveur_Plug294
{
    // Menu Machines : toutes les machines du jeu, par métier + ustensiles de base
    // Prix = 50 % du prix officiel sur les machines, les ustensiles gardent le prix officiel
    public partial class TransNoveurPlug294
    {
        private static readonly ShopCategory[] machinesCatalog = new ShopCategory[]
        {
            new ShopCategory { Name = "Bois", IconItem = 1080, Items = new ShopEntry[] {
                It("Machine à découper le bois", 1080, 7500),
                It("Machine à découper le bois auto", 1082, 25000),
                It("Machine d'assemblage auto", 1083, 37500),
                It("Machine de peinture (bois)", 1605, 5000),
            } },
            new ShopCategory { Name = "Métallurgie", IconItem = 1420, Items = new ShopEntry[] {
                It("Haut fourneau", 1420, 15000),
                It("Haut fourneau industriel", 1721, 57500),
                It("Convertisseur", 1424, 7500),
                It("Laminoir", 1428, 10000),
                It("Machine d'assemblage de métal", 1433, 25000),
                It("Machine de peinture (métal)", 1597, 5000),
                It("Machine à sertir", 1373, 5000),
            } },
            new ShopCategory { Name = "Plastique & Carton", IconItem = 1086, Items = new ShopEntry[] {
                It("Machine de production de plastique", 1086, 45000),
                It("Machine de production de carton", 1374, 7500),
                It("Machine d'assemblage de cartons", 1377, 5000),
            } },
            new ShopCategory { Name = "Électronique", IconItem = 1742, Items = new ShopEntry[] {
                It("Machine de production de PCB", 1742, 40000),
            } },
            new ShopCategory { Name = "Production alimentaire", IconItem = 1567, Items = new ShopEntry[] {
                It("Robot boulangerie", 1567, 7500),
                It("Machine de production de vin", 1092, 17500),
                It("Four boulangerie", 1460, 1999.50),
            } },
            new ShopCategory { Name = "Cuisine & Fast Food", IconItem = 1515, Items = new ShopEntry[] {
                It("Établi Fast Food", 1515, 2249.50),
                It("Friteuse", 1994, 125),
                It("Planche à découper", 1991, 7.50),
                It("Plaque de cuisson", 40, 149.50),
            } },
            new ShopCategory { Name = "Café", IconItem = 77, Items = new ShopEntry[] {
                It("Machine à café rouge", 77, 49.50),
                It("Machine à café brune", 1797, 49.50),
                It("Machine à café cyan", 1798, 49.50),
                It("Machine à café grise", 1799, 49.50),
                It("Machine à café verte", 1800, 49.50),
                It("Machine à café violette", 1801, 49.50),
                It("Machine à café (expresso)", 6019, 145),
                It("Machine à café professionnelle", 6038, 5130),
            } },
            new ShopCategory { Name = "Tireuses", IconItem = 1378, Items = new ShopEntry[] {
                It("Tireuse à soda", 1378, 249.50),
                It("Tireuse à bière", 1735, 599.50),
            } },
            // Pas des machines : prix officiels, non touchés par la remise
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
            ShopMenu(player, "Machines", machinesCatalog, LogGris);
        }
    }
}

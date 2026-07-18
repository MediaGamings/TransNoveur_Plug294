using Life.Network;

namespace TransNoveur_Plug294
{
    // Menu Nourriture : que des ingrédients, gros format quand il existe
    // Prix = 50 % du prix officiel (sans prix officiel = prix libre)
    public partial class TransNoveurPlug294
    {
        private static readonly ShopCategory[] nourritureCatalog = new ShopCategory[]
        {
            new ShopCategory { Name = "Station Exo", IconItem = 1925, Items = new ShopEntry[] {
                It("Pack d'eau (x6)", 1924, 6),
                It("Carton de soda (x24)", 1925, 48),
                It("Cookies (x6)", 1965, 1.30),
                It("Chips Day's", 89, 2.5),
                It("Saucisson", 90, 5),
                It("Cagette de pommes", 6048, 18),
                It("Cagette de poires", 6049, 18),
            } },
            new ShopCategory { Name = "Boulangerie", IconItem = 1449, Items = new ShopEntry[] {
                It("Farine", 1449, 1.5),
                It("Levure", 1450, 1),
                It("Lait", 1451, 1),
                It("Pot de sucre", 1452, 2.5),
                It("Pot de sel", 1453, 2.5),
                It("Chocolat", 1454, 0.5),
                It("Oeuf", 1504, 0.5),
                It("Beurre", 1566, 1),
            } },
            new ShopCategory { Name = "Fast Food", IconItem = 1922, Items = new ShopEntry[] {
                It("Boîte à pizza (x10)", 1922, 60),
                It("Steak", 1, 2.5),
                It("Nuggets", 2, 2.5),
                It("Pain burger", 1508, 1),
                It("Feuille de salade", 1506, 0.5),
                It("Tomate", 1505, 0.75),
                It("Fromage", 137, 2),
                It("Pot de moutarde", 1510, 1),
                It("Pot de cornichon", 1511, 1),
                It("Sac de pommes de terre (x25)", 1988, 5),
                It("Huile de friture", 1993, 1.38),
            } },
            new ShopCategory { Name = "Fruits & Graines", IconItem = 1439, Items = new ShopEntry[] {
                It("Fraise", 1439, 0.5),
                It("Grappe de raisin", 1093, 2),
                It("Graine de fraisier", 1440, 0.5),
                It("Graine de tomate", 1720, 0.5),
            } },
            new ShopCategory { Name = "Café", IconItem = 1997, Items = new ShopEntry[] {
                It("Capsules \"Barista\" (x25)", 1997, 6.25),
                It("Capsules \"Fortissimo\" (x25)", 1998, 6.25),
                It("Capsules \"Forza\" (x25)", 1999, 6.25),
                It("Capsules \"Lungo\" (x25)", 6000, 6.25),
                It("Capsules \"Ristretto\" (x25)", 6001, 6.25),
                It("Grain de café", 6017, 10),
                It("Lait en poudre", 6018, 1.48),
                It("Chocolat en poudre", 6016, 5.25),
            } },
        };

        public void Nourriture(Player player)
        {
            ShopMenu(player, "Fournisseur Restauration", nourritureCatalog);
        }
    }
}

using Life.Network;
using Life.UI;
using Life;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hub581.Helpers
{
    public class ShopPanelHelper
    {
        public static ShopPanel Create(string title, Vector3 position, Player player)
        {
            return new ShopPanel(title, position, player);
        }
        
        public static ShopPanel Create(string title, Player player)
        {
            return new ShopPanel(title, player);
        }

        public class ShopPanel
        {
            public class Category
            {
                public int Id { get; set; }
                public string Name { get; set; }

                public Category(int id, string name)
                {
                    Id = id;
                    Name = name;
                }
            }

            public class ItemShop
            {
                public int ItemId { get; set; }
                public Life.InventorySystem.Item Item { get => Nova.man.item.GetItem(ItemId); }
                public double Price { get; set; }
                public int CategoryId { get; set; }

                public ItemShop(int itemId, double price, int categoryId)
                {
                    ItemId = itemId;
                    Price = price;
                    CategoryId = categoryId;
                }

                public ItemShop(int itemId, double price, Category category)
                {
                    ItemId = itemId;
                    Price = price;
                    CategoryId = category.Id;
                }

                public ItemShopDefinition Parse()
                {
                    var itemShop = new ItemShopDefinition();
                    itemShop.item = Item;
                    itemShop.price = Price;
                    itemShop.categoryId = CategoryId;
                    return itemShop;
                }
            }

            public Dictionary<int, Category> Categories { get; private set; } = new Dictionary<int, Category>();
            public List<ItemShop> Items { get; private set; } = new List<ItemShop>();
            private int Index { get; set; } = 0;
            public string Title { get; set; }
            public Vector3 Position { get; set; }
            public Player TargetPlayer { get; set; }
            public bool AllowDuplicates { get; set; } = false;

            public ShopPanel(string title, Vector3 position, Player targetPlayer)
            {
                Title = title;
                Position = position;
                TargetPlayer = targetPlayer;
            }

            public ShopPanel(string title, Player targetPlayer)
            {
                Title = title;
                Position = targetPlayer.setup.transform.position;
                TargetPlayer = targetPlayer;
            }

            public Category AddCategory(string name)
            {
                int index = Index++;
                var category = new Category(index, name);
                Categories.Add(index, category);
                return category;
            }

            public ItemShop AddItem(int itemId, double price, int categoryId)
            {
                var itemShop = new ItemShop(itemId, price, categoryId);
                Items.Add(itemShop);
                return itemShop;
            }

            public ItemShop AddItem(int itemId, double price, Category category) => AddItem(itemId, price, category.Id);

            public void Display()
            {
                var shopItems = Items.Select(item => item.Parse()).ToArray();
                var categoryNames = Categories.Values.Select(c => c.Name).ToArray();
                TargetPlayer.ShowShopUI(shopItems, categoryNames, Title, Position);
            }
        }
    }
}

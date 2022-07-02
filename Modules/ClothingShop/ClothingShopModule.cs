using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Models.ClothingModel;
using Backend.Models.ClothingModel.Data;
using Backend.Models.NotificationModel;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backend.Modules.ClothingShop
{
    public class ClothingShopModule : Script
    {
        private readonly CDBCLient _database;

        public ClothingShopModule()
        {
            _database = new CDBCLient();

            NAPI.ClientEvent.Register<CPlayer, int, int, int, bool, int>("ClothingShop:BuyClothingItem", this, BuyClothingItem);
            NAPI.ClientEvent.Register<CPlayer>("Server:CloseClothingShop", this, CloseClothingShop);
        }

        private void CloseClothingShop(CPlayer player)
        {
            if (player == null) return;
            var playerClothes = player.DBModel?.ClothingModel?.clothingComponents;
            if (playerClothes == null) return;

            NAPI.Task.Run(() =>
            {
                playerClothes.ForEach(c =>
                {
                    if (!c.IsProp)
                        NAPI.Player.SetPlayerClothes(player, c.Id, c.Drawable, c.Texture);
                    else
                        NAPI.Player.SetPlayerAccessory(player, c.Id, c.Drawable, c.Texture);
                });
            });
        }

        private async void BuyClothingItem(CPlayer player, int component, int drawable, int texture, bool is_accessories, int price)
        {
            if (player == null) return;
            if (component < 0) return;
            if (texture < 0) return;
            if (price <= 0) return;

            Console.WriteLine($"{component}, {drawable}, {texture}, {is_accessories}, {price}");

            if (player.DBModel.Money < price)
            {
                NAPI.Task.Run(() =>
                {
                    player.SendCloudNotification("Kleidungsladen", "Dafür hast du zu wenig Geld!", 3500, NotificationModel.ALERT, false);
                    return;
                });
            }


            ComponentModel model = new ComponentModel
            {
                Drawable = drawable,
                Texture = texture,
                IsProp = is_accessories,
                Id = component
            };

            var hasClothing = player.DBModel.ClothingModel?.clothingComponents?.FirstOrDefault(c => c.Id == component && c.IsProp == is_accessories);
            if (hasClothing == null)
            {
                player.DBModel.ClothingModel.clothingComponents.Add(model);
            }
            else
            {
                hasClothing.Drawable = drawable;
                hasClothing.Texture = texture;
            }

            NAPI.Task.Run(() =>
            {
                player.SendCloudNotification("Kleidungsladen", "Du hast ein Kleidungsstück erworben!", 3500, NotificationModel.SUCCESS, false);
                if (is_accessories)
                {
                    player.SetAccessories(component, drawable, texture);
                }
                else
                {
                    player.SetClothes(component, drawable, texture);
                }
                player.Update();
            });
            CloseClothingShop(player);

        }
    }
}

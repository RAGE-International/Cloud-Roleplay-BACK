using Backend.Core.Database;
using Backend.Models.PlayerModel;
using Backend.Modules.Inventory.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Factories.CPlayer
{
    public class CPlayer : Player
    {
        private readonly CDBCLient _database = new CDBCLient();


        public int ID { get; set; }
        public ulong SocialClubID { get; set; }
        public PlayerModel DBModel { get; set; }
        public string HWID { get; set; } = string.Empty;
        public bool LoggedIn { get; set; } = false;
        public bool IsInCall { get; set; } = false;
        public bool InSmartphone { get; set; } = false;
        public CColShape.CColShape CurrentShape { get; set; }
        public CPlayer InteractingWith { get; set; } = null;
        public CPlayer CurrentPhoneCall { get; set; }
        public bool InAction { get; set; } = false;
        public void GiveACWeapon(string weaponName)
        {
            NAPI.Task.Run(() =>
            {
                WeaponHash hash = (WeaponHash)NAPI.Util.GetHashKey("weapon_" + weaponName.ToLower());
                TriggerEvent("client:anticheat:addWeaponToWhitelist", hash);
                GiveWeapon(hash, 9999);
            });
        }

        public void RemoveACWeapon(string weaponName)
        {
            NAPI.Task.Run(() =>
            {
                WeaponHash hash = (WeaponHash)NAPI.Util.GetHashKey("weapon_" + weaponName.ToLower());
                RemoveWeapon(hash);
                TriggerEvent("client:anticheat:removeWeaponFromWhitelist", hash);
            });
        }

        public void SetHealthAC(int health)
        {
            NAPI.Task.Run(() =>
            {
                TriggerEvent("client:anticheat:setAcHealth", health);
            });
        }

        public void SetArmorAC(int armor)
        {
            NAPI.Task.Run(() =>
            {
                TriggerEvent("client:anticheat:setAcArmor", armor);
            });
        }

        public void RemoveAllACWeapon()
        {
            NAPI.Task.Run(() =>
            {
                TriggerEvent("client:anticheat:removeAllWeapons");
            });
        }
        public void StartProgressBar(int time)
        {
            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(this, "Client:StartProgress", time);
            });
        }

        public void StopProgressBar()
        {
            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(this, "Client:StopProgress");
            });
        }
        public InventoryModel Inventory
        {
            get
            {
                return _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == DBModel.InventoryId).Result;
            }
        }
        public bool IsCuffed { get
            {
                return this.GetSharedData<bool>("PLAYER_IS_CUFFED");
            }
            set
            {
                this.SetSharedData("PLAYER_IS_CUFFED", value);
            }
        }
        public void SetActionAnimation(string animationDict, int flag)
        {
            SetData("Player:Action:Animation", animationDict);
            SetData("Player:Action:Animation_Flag", flag);
        }
        public void ResetActionAnimation()
        {
            ResetData("Player:Action:Animation");
            ResetData("Player:Action:Animation_Flag");
        }

        public bool HasActionAnimation()
        {
            return HasData("Player:Action:Animation") && HasData("Player:Action:Animation_Flag");
        }
        public bool InInventory { get; set; }
        public void SendCloudNotification(string title, string message, int time, string type, bool adminNotify)
        {
            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(this, "Client:CreateNotification", title, message, time, type, adminNotify);
            });
        }

        public void SmartphoneShowMessage(string title, string message, string icon)
        {
            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(this, "Client:PhoneShowMessage", title, message, icon);
            });
        }
        public CPlayer(NetHandle handle) : base(handle)
        {
            ID = -1;
            SocialClubID = SocialClubId;
            DBModel = null;
            HWID = Serial;
            InInventory = false;
        }

        internal async Task Update()
        {
            await _database.Update<PlayerModel>("Players", this.DBModel, p => p.HWID == this.HWID);
        }    
    }
}

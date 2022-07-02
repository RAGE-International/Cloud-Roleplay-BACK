using Backend.Models.PlayerBankModel.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.PlayerBankModel
{
    public class PlayerBankModel
    {
        public PlayerBankModel(int bankmoney, int currentmoney, int banknumber, BankHistoryModel bankHistory)
        {
            this.bankmoney = bankmoney;
            this.currentmoney = currentmoney;
            this.banknumber = banknumber;
            this.bankHistory = bankHistory;
        }

        public int bankmoney { get; set; }
        public int currentmoney { get; set; }
        public int banknumber { get; set; }
        public BankHistoryModel bankHistory { get; set; }
    }
}

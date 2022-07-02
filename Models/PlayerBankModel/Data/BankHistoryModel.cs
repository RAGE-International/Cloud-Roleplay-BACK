using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.PlayerBankModel.Data
{
    public class BankHistoryModel
    {
        public string name { get; set; }
        public int amount { get; set; }
        public bool ispayment { get; set; }
        public string date { get; set; }
    }
}

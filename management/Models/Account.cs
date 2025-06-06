﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace management.Models
{
    public class Account
    {
        public int code { get; set; }
        public int PersonCode { get; set; }
        public string? account_number { get; set; } = string.Empty;
        [NotMapped]
        public string? Accountnumber { get; set; } = string.Empty;
        public decimal outstanding_balance { get; internal set; } = 0;
        public List<Transaction> Transaction { get; set; } = [];

    }
}

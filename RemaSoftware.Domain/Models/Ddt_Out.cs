﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Ddt_Out
    {
        public int Ddt_Out_ID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [MaxLength(1)]
        public string Status { get; set; }
        public int FC_Ddt_Out_ID { get; set; }
        public int NumberPieces { get; set; }
    }
}
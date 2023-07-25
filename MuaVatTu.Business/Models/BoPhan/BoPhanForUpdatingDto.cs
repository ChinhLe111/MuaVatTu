using System;
using System.ComponentModel.DataAnnotations;

namespace MuaVatTu.Business
{
    public class BoPhanForUpdatingDto 
    {
        public string Name { get; set; }
        public string NameOfLeader { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }
    }
}

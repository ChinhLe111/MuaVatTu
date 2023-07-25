using MuaVatTu.Data;
using System;

namespace MuaVatTu.Business
{
    public class DangKyForUpdatingDto 
    {
        public string NameOfUser { get; set; }
        public DateTime Date { get; set; }
        public BoPhan BoPhan { get; set; }
    }
}

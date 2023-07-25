using MuaVatTu.Data;
using System;

namespace MuaVatTu.Business
{
    public class DangKyDto
    {
        public Guid Id { get; set; }
        public string NameOfUser { get; set; }
        public DateTime Date { get; set; }
        public BoPhanDto BoPhan { get; set; }
    }
}

using MuaVatTu.Data;
using System;

namespace MuaVatTu.Business
{
    public class BoPhanDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NameOfLeader { get; set; }
        public string Date { get; set; }
        public int Number { get; set; }
    }
}

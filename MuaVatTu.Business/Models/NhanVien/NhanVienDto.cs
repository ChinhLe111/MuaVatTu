using MuaVatTu.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MuaVatTu.Business
{
    public class NhanVienDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Guid BoPhanId { get; set; }
    }
}

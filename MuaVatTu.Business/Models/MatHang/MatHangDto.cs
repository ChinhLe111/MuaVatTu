using MuaVatTu.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MuaVatTu.Business
{
    public class MatHangDto
    {
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public int Count { get; set; }
        public Guid DangKyId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MuaVatTu.Data
{
    [Table("mvt_MatHang")]
    public class MatHang
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public int Count { get; set; }

        [ForeignKey("DangKyId")]
        public Guid DangKyId { get; set; }
        public DangKy DangKy { get; set; }
    }
}
   

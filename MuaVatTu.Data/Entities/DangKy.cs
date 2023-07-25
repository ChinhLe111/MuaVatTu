using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MuaVatTu.Data
{
    [Table("mvt_DangKy")]
    public class DangKy
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string NameOfUser { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("BoPhanId")]
        public Guid BoPhanId { get; set; }
        public BoPhan BoPhan { get; set; }
        public ICollection<MatHang> MatHangs { get; set; } = new List<MatHang>();
    }
}
   

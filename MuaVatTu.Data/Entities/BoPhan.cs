using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MuaVatTu.Data
{
    [Table("mvt_BoPhan")]
    public class BoPhan
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string NameOfLeader { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }
        public ICollection<DangKy> DangKys { get; set; } = new List<DangKy>();
        public ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();

    }
}

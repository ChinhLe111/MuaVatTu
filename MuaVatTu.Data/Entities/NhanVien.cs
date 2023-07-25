using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuaVatTu.Data
{
    [Table("mvt_NhanVien")]
    public class NhanVien
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("BoPhanId")]
        public Guid BoPhanId { get; set; }
        public BoPhan BoPhan { get; set; }

    }
}

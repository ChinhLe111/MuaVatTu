using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MuaVatTu.Data
{
    [Table("mvt_TongSap")]
    public class Tong
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public Guid BoPhanId { get; set; }

    }
}

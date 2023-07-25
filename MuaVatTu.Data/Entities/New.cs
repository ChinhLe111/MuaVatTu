using NpgsqlTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuaVatTu.Data
{
    [Table("mvt_New")]
    public class New
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string UnaccentTitle { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public NpgsqlTsVector SearchVector { get; set; }
    }
}

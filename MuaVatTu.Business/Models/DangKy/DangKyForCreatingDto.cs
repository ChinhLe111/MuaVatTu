using MuaVatTu.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuaVatTu.Business
{
    public class DangKyForCreatingDto
    {
        public string NameOfUser { get; set; }
        public DateTime Date { get; set; }
        public ICollection<MatHangDto> MatHangs { get; set; } = new List<MatHangDto>();
    }
}

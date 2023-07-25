using MuaVatTu.Common.Helpers;
using MuaVatTu.Data;
using System;
using System.Collections.Generic;

namespace MuaVatTu.Business
{
    public class NhanVienModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public BoPhan BoPhan { get; set; }
        public Guid BoPhanId { get; set; }
    }
    public class AddNhanVienModel
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
    public class UpdateNhanVienModel
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
    public class MaxMonth
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
    public class NhanVienBoPhan
    {
        public string Name { get; set; }
        public int Count { get; set; }

    }
    public class ListMonth
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int Count { get; set; }

    }


    public class NhanVienQueryModel : PaginationRequest
    {
        public Guid? Id { get; set; }
    }


}

using AutoMapper;
using MuaVatTu.Common.Helpers;
using MuaVatTu.Data;
using System;
using System.Collections.Generic;

namespace MuaVatTu.Business
{
    public class DangKyModel 
    { 
        public Guid Id { get; set; }
        public string NameOfUser { get; set; }
        public DateTime Date { get; set; }
        public BoPhan BoPhan { get; set; }
        public Guid BoPhanId { get; set; }
    }

    public class DangKyCreateModel
    {
        public string NameOfUser { get; set; }
        public Guid BoPhanId { get; set; }
        public ICollection<MatHangDto> MatHangs { get; set; }

    }
    public class DangKyGetId
    {
        public string NameOfUser { get; set; }
        public Guid BoPhanId { get; set; }
        public DateTime Date { get; set; }
        public ICollection<MatHangDto> MatHangs { get; set; }

    }
    public class ListMatHang
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Count { get;set; }

    }
    public class DangKyForMonth
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int Count { get; set; }

    }

    public class DangKyUpdateModel
    {
        public string NameOfUser { get; set; }
        public DateTime Date { get; set; }
    } 

    public class DangKyQueryModel : PaginationRequest
    {
        public Guid? Id { get; set; }
        public BoPhan BoPhan { get; set; }
    }
}

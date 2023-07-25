using Bogus.DataSets;
using MuaVatTu.Common.Helpers;
using MuaVatTu.Data;
using System;
using System.Collections.Generic;

namespace MuaVatTu.Business
{
    public class MatHangModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public DangKy DangKy { get; set; }
        public Guid DangKyId { get; set; }
    }
    public class AddMatHangModel
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class UpdateMatHangModel
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class MatHangMaxCount
    {
        public int Count { get; set; }
        public string Name { get; set; }
    }
    public class ListName
    {
        public int Count { get; set; }
        public List<string> Names { get; set; }
    }
    public class MonthOfMaxCount
    {
        public int count { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class MatHangQueryModel : PaginationRequest
    {
        public Guid? Id { get; set; }
    }


}

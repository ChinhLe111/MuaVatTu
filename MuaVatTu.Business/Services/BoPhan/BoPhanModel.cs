using MuaVatTu.Common.Helpers;
using System;

namespace MuaVatTu.Business
{
    public class BoPhanModel
    {
        public Guid Id { get; set; }
        public string NameOfLeader { get; set; }
        public string Name{ get; set; }
        public string Date { get; set; }
        public int Number { get; set; }
    }
    public class BoPhanAddModel
    {
        public string Name { get; set; }
        public string NameOfLeader { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }
    }
    public class BoPhanUpdateModel
    {
        public string Name { get; set; }
        public string NameOfLeader { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }
    }

    public class BoPhanDangKyModel
    {
        public int Count { get; set; }
        public Guid BoPhanId { get; set; }
        public string Name { get; set; }
        public string NameOfLeader { get; set; }
        public DateTime Date { get; set; }
    }
    public class BoPhanQueryModel : PaginationRequest
    {
        public Guid? Id { get; set; }
    }
}

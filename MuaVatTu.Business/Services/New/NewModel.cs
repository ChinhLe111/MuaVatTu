using MuaVatTu.Common.Helpers;
using System;

namespace MuaVatTu.Business
{
    public class NewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string UnaccentTitle { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
    public class AddNewModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string UnaccentTitle { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
    public class UpdateNewModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string UnaccentTitle { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
    public class NewQueryModel : PaginationRequest
    {
        public Guid? Id { get; set; }
    }


}

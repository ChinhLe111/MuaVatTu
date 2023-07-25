using AutoMapper;
using MuaVatTu.Business;
using MuaVatTu.Common;
using MuaVatTu.Common.Helpers;
using MuaVatTu.Data;
using System.Collections;
using System.Collections.Generic;

namespace MuaVatTu.Api.v1
{
    public class AutoMapperConfig 
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BoPhanProfile());
                cfg.AddProfile(new DangKyProfile());
                cfg.AddProfile(new MatHangProfile());
                cfg.AddProfile(new NhanVienProfile());
            });
        }
    }

    public class BoPhanProfile : Profile
    {
        public BoPhanProfile()
        { 
            CreateMap<BoPhan, BoPhanDto>()
     .ForMember(
         dest => dest.Date,
         opt => opt.MapFrom(src => $"{src.Date.ToString("yyyy")} ({src.Date.GetYearsAgo()}) years ago"));
            CreateMap<BoPhan, BoPhanAddModel>().ReverseMap();
            CreateMap<BoPhan, BoPhanUpdateModel>().ReverseMap();
            CreateMap<Pagination<BoPhan>, Pagination<BoPhanDto>>();
        }
    }
    public class DangKyProfile : Profile
    {
        public DangKyProfile()
        {
            CreateMap<DangKy, DangKyDto>().ReverseMap();
            CreateMap<DangKyQueryModel, DangKy>();
            CreateMap<Pagination<DangKy>, Pagination<DangKyDto>>();
            CreateMap<DangKyForUpdatingDto, DangKyDto>().ReverseMap();
            CreateMap<DangKyForCreatingDto, DangKyDto>();
            CreateMap<DangKy, DangKyCreateModel>().ReverseMap();
            CreateMap<DangKy, DangKyUpdateModel>().ReverseMap();
            CreateMap<DangKyCreateModel, List<MatHang>>().ReverseMap();
            CreateMap<DangKyCreateModel,MatHang>().ReverseMap();
        }
    }
    public class MatHangProfile : Profile
    {
        public MatHangProfile()
        {
            CreateMap<MatHang, MatHangDto>().ReverseMap();
            CreateMap<Pagination<MatHang>, Pagination<MatHangDto>>();
            CreateMap<MatHang, UpdateMatHangModel>().ReverseMap();
            CreateMap<MatHang, AddMatHangModel>().ReverseMap();
            CreateMap<ICollection<MatHang>, ListMatHang>();
        }
    }
    public class NhanVienProfile : Profile
    {
        public NhanVienProfile()
        {
            CreateMap<NhanVien, NhanVienDto>().ReverseMap();
            CreateMap<NhanVien, UpdateNhanVienModel>().ReverseMap();
            CreateMap<NhanVien, AddNhanVienModel>().ReverseMap();
            CreateMap<Pagination<NhanVien>, Pagination<NhanVienDto>>();
        }
    }
    public class NewProfile : Profile
    {
        public NewProfile()
        {
            CreateMap<New, NewDto>().ReverseMap();
            CreateMap<New, UpdateNewModel>().ReverseMap();
            CreateMap<New, AddNewModel>().ReverseMap();
            CreateMap<Pagination<New>, Pagination<NewDto>>();
        }
    }
}

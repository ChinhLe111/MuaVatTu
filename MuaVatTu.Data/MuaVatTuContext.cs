using MuaVatTu.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MuaVatTu.Data
{
    public class MuaVatTuContext : DbContext
    {
        public MuaVatTuContext(DbContextOptions<MuaVatTuContext> options) : base(options)
        {

        }
        public DbSet<BoPhan> BoPhans { get; set; }
        public DbSet<DangKy> Dangkys { get; set; }
        public DbSet<MatHang> MatHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<Tong> Tongs { get; set; }
        public DbSet<New> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<New>()
                   .HasIndex(p => p.SearchVector)
                   .HasMethod("GIST");

            base.OnModelCreating(modelBuilder);
        }

    }
}

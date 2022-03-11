using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using demoapi.DAL.Accounting.Models;

namespace demoapi.DAL.Accounting
{
    public partial class accountingContext : DbContext
    {
        public accountingContext()
        {
        }

        public accountingContext(DbContextOptions<accountingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblAuthenticate> TblAuthenticates { get; set; } = null!;
        public virtual DbSet<TblRecord> TblRecords { get; set; } = null!;
        public virtual DbSet<TblUser> TblUsers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#pragma warning disable CS1030 // #warning 指令
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=ng.talkofice.com;port=30466;uid=root;pwd=Magic707Nicol^^();database=accounting", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.37-mysql"));
#pragma warning restore CS1030 // #warning 指令
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<TblAuthenticate>(entity =>
            {
                entity.ToTable("tbl_authenticate");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasComment("id")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .HasComment("密码")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Token)
                    .HasMaxLength(200)
                    .HasComment("令牌")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasComment("用户名")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");
            });

            modelBuilder.Entity<TblRecord>(entity =>
            {
                entity.ToTable("tbl_record");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasComment("id")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.AccountType)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'")
                    .HasComment("借-收入0；贷-支出1");

                entity.Property(e => e.Amount)
                    .HasPrecision(10)
                    .HasComment("金额");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasComment("建立时间");

                entity.Property(e => e.CreateUser)
                    .HasMaxLength(50)
                    .HasComment("建立用户")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.RecordDate).HasComment("记账日期");

                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .HasComment("注释")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.TypeName)
                    .HasMaxLength(50)
                    .HasComment("类型：一般；交通；孩子；住房；通信；居家；购物；旅行；餐饮")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasComment("更新时间");

                entity.Property(e => e.UpdateUser)
                    .HasMaxLength(50)
                    .HasComment("更新用户")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasComment("用户id")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.ToTable("tbl_user");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasComment("id")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasComment("建立时间");

                entity.Property(e => e.CreateUser)
                    .HasMaxLength(50)
                    .HasComment("建立用户")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasComment("电子邮件")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasComment("姓名")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasComment("更新时间");

                entity.Property(e => e.UpdateUser)
                    .HasMaxLength(50)
                    .HasComment("更新用户")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasComment("用户名")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

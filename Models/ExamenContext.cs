using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace test_lester02.Models
{
    public partial class ExamenContext : DbContext
    {

        public ExamenContext()
        {
        }

        public ExamenContext(DbContextOptions<ExamenContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblRfidCodiCaptEmbarque> TblRfidCodiCaptEmbarques { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-22NSM32\\SQLEXPRESS; Database=Examen; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblRfidCodiCaptEmbarque>(entity =>
            {
                entity.ToTable("tblRFID_CodiCaptEmbarques");

                entity.HasIndex(e => e.Codebar, "llave_codigo")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Acronimo)
                    .HasMaxLength(50)
                    .HasColumnName("acronimo");

                entity.Property(e => e.Codebar)
                    .HasMaxLength(50)
                    .HasColumnName("codebar");

                entity.Property(e => e.FechaLectura)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaLectura");

                entity.Property(e => e.ObjReferencia).HasColumnName("objReferencia");

                entity.Property(e => e.Tipo).HasColumnName("tipo");

                entity.Property(e => e.Viaje).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }   

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

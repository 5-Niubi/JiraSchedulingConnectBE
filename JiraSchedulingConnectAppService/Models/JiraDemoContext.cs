using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JiraSchedulingConnectAppService.Models;

public partial class JiraDemoContext : DbContext
{
    public JiraDemoContext()
    {
    }

    public JiraDemoContext(DbContextOptions<JiraDemoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AtlassianToken> AtlassianTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=(local); database=JiraDemo; uid=sa; pwd=sa; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AtlassianToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_access_token");

            entity.ToTable("atlassian_token");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("access_token");
            entity.Property(e => e.AccountId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.CloudId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cloud_id");
            entity.Property(e => e.RefressToken)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("refress_token");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

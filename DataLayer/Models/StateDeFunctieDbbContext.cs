using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ManagementulStatelorDeFunctii.Models;

public partial class StateDeFunctieDbbContext : DbContext
{
    public StateDeFunctieDbbContext()
    {
    }

    public StateDeFunctieDbbContext(DbContextOptions<StateDeFunctieDbbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnPromotie> AnPromoties { get; set; }

    public virtual DbSet<CadruDidactic> CadruDidactics { get; set; }

    public virtual DbSet<CadruDidacticGradDidactic> CadruDidacticGradDidactics { get; set; }

    public virtual DbSet<CadruDidacticStatFunctieTarifPlataOra> CadruDidacticStatFunctieTarifPlataOras { get; set; }

    public virtual DbSet<Departament> Departaments { get; set; }

    public virtual DbSet<Disciplina> Disciplinas { get; set; }

    public virtual DbSet<DisciplinaCadruDidactic> DisciplinaCadruDidactics { get; set; }

    public virtual DbSet<Facultate> Facultates { get; set; }

    public virtual DbSet<GradDidactic> GradDidactics { get; set; }

    public virtual DbSet<GradDidacticNormabil> GradDidacticNormabils { get; set; }

    public virtual DbSet<Norma> Normas { get; set; }

    public virtual DbSet<NormaDisciplina> NormaDisciplinas { get; set; }

    public virtual DbSet<ProgramDeStudii> ProgramDeStudiis { get; set; }

    public virtual DbSet<Promotie> Promoties { get; set; }

    public virtual DbSet<StatDeFunctie> StatDeFuncties { get; set; }

    public virtual DbSet<TipProgramStudiu> TipProgramStudius { get; set; }

    public virtual DbSet<Utilizator> Utilizators { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=StateDeFunctieDBB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnPromotie>(entity =>
        {
            entity.ToTable("AnPromotie");

            entity.Property(e => e.AnPromotieId).HasColumnName("AnPromotieID");
            entity.Property(e => e.PromotieId).HasColumnName("PromotieID");

            entity.HasOne(d => d.Promotie).WithMany(p => p.AnPromoties)
                .HasForeignKey(d => d.PromotieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnPromotie_Promotie1");
        });

        modelBuilder.Entity<CadruDidactic>(entity =>
        {
            entity.HasKey(e => e.CadruDidacticId).HasName("PK__CadruDid__716616422B9B6F8A");

            entity.ToTable("CadruDidactic");

            entity.Property(e => e.CadruDidacticId).HasColumnName("CadruDidacticID");
            entity.Property(e => e.DepartamentId).HasColumnName("DepartamentID");
            entity.Property(e => e.GradDidacticId).HasColumnName("GradDidacticID");

            entity.HasOne(d => d.Departament).WithMany(p => p.CadruDidactics)
                .HasForeignKey(d => d.DepartamentId)
                .HasConstraintName("FK_CadruDidactic_Departament");

            entity.HasOne(d => d.GradDidactic).WithMany(p => p.CadruDidactics)
                .HasForeignKey(d => d.GradDidacticId)
                .HasConstraintName("FK_CadruDidactic_GradDidactic");

            entity.HasMany(d => d.Departaments).WithMany(p => p.CadruDidacticsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "CadruDidacticDepartament",
                    r => r.HasOne<Departament>().WithMany()
                        .HasForeignKey("DepartamentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CadruDidacticDepartament_Departament"),
                    l => l.HasOne<CadruDidactic>().WithMany()
                        .HasForeignKey("CadruDidacticId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CadruDidacticDepartament_CadruDidactic"),
                    j =>
                    {
                        j.HasKey("CadruDidacticId", "DepartamentId");
                        j.ToTable("CadruDidacticDepartament");
                        j.IndexerProperty<int>("CadruDidacticId").HasColumnName("CadruDidacticID");
                        j.IndexerProperty<int>("DepartamentId").HasColumnName("DepartamentID");
                    });
        });

        modelBuilder.Entity<CadruDidacticGradDidactic>(entity =>
        {
            entity.ToTable("CadruDidacticGradDidactic");

            entity.Property(e => e.CadruDidacticGradDidacticId).HasColumnName("CadruDidacticGradDidacticID");
            entity.Property(e => e.CadruDidacticId).HasColumnName("CadruDidacticID");
            entity.Property(e => e.GradDidacticId).HasColumnName("GradDidacticID");

            entity.HasOne(d => d.CadruDidactic).WithMany(p => p.CadruDidacticGradDidactics)
                .HasForeignKey(d => d.CadruDidacticId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CadruDidacticGradDidactic_CadruDidactic");

            entity.HasOne(d => d.GradDidactic).WithMany(p => p.CadruDidacticGradDidactics)
                .HasForeignKey(d => d.GradDidacticId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CadruDidacticGradDidactic_GradDidactic");
        });

        modelBuilder.Entity<CadruDidacticStatFunctieTarifPlataOra>(entity =>
        {
            entity.HasKey(e => e.CadruDidacticStatFunctieTarifId);

            entity.ToTable("CadruDidacticStatFunctieTarifPlataOra");

            entity.Property(e => e.CadruDidacticStatFunctieTarifId).HasColumnName("CadruDidacticStatFunctieTarifID");
            entity.Property(e => e.CadruDidacticId).HasColumnName("CadruDidacticID");
            entity.Property(e => e.StatFunctieId).HasColumnName("StatFunctieID");

            entity.HasOne(d => d.CadruDidactic).WithMany(p => p.CadruDidacticStatFunctieTarifPlataOras)
                .HasForeignKey(d => d.CadruDidacticId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CadruDidacticStatFunctieTarifPlataOra_CadruDidacticGradDidactic");

            entity.HasOne(d => d.StatFunctie).WithMany(p => p.CadruDidacticStatFunctieTarifPlataOras)
                .HasForeignKey(d => d.StatFunctieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CadruDidacticStatFunctieTarifPlataOra_StatDeFunctie");
        });

        modelBuilder.Entity<Departament>(entity =>
        {
            entity.HasKey(e => e.DepartamentId).HasName("PK__Departam__3B984D9ADCC397CB");

            entity.ToTable("Departament");

            entity.Property(e => e.DepartamentId).HasColumnName("DepartamentID");
            entity.Property(e => e.Acronim)
                .HasMaxLength(5)
                .HasDefaultValue("-");
            entity.Property(e => e.FacultateId).HasColumnName("FacultateID");

            entity.HasOne(d => d.Facultate).WithMany(p => p.Departaments)
                .HasForeignKey(d => d.FacultateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Departame__Facul__6FE99F9F");
        });

        modelBuilder.Entity<Disciplina>(entity =>
        {
            entity.HasKey(e => e.DisciplinaId).HasName("PK__Discipli__2801571E7478C2F5");

            entity.ToTable("Disciplina");

            entity.Property(e => e.DisciplinaId).HasColumnName("DisciplinaID");
            entity.Property(e => e.Acronim).HasMaxLength(10);
            entity.Property(e => e.AnPromotieId).HasColumnName("AnPromotieID");

            entity.HasOne(d => d.AnPromotie).WithMany(p => p.Disciplinas)
                .HasForeignKey(d => d.AnPromotieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Disciplina_AnPromotie");
        });

        modelBuilder.Entity<DisciplinaCadruDidactic>(entity =>
        {
            entity.HasKey(e => e.NormaDisciplinaCadruDidacticId).HasName("PK_DisciplinaCadruDidactic_1");

            entity.ToTable("DisciplinaCadruDidactic");

            entity.Property(e => e.CadruDidacticId).HasColumnName("CadruDidacticID");
            entity.Property(e => e.NormaDisciplinaId).HasColumnName("NormaDisciplinaID");

            entity.HasOne(d => d.CadruDidactic).WithMany(p => p.DisciplinaCadruDidactics)
                .HasForeignKey(d => d.CadruDidacticId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DisciplinaCadruDidactic_CadruDidacticGradDidactic");

            entity.HasOne(d => d.NormaDisciplina).WithMany(p => p.DisciplinaCadruDidactics)
                .HasForeignKey(d => d.NormaDisciplinaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DisciplinaCadruDidactic_NormaDisciplina");
        });

        modelBuilder.Entity<Facultate>(entity =>
        {
            entity.HasKey(e => e.FacultateId).HasName("PK__Facultat__CE83A3B2D00071ED");

            entity.ToTable("Facultate");

            entity.Property(e => e.FacultateId).HasColumnName("FacultateID");
            entity.Property(e => e.Acronim).HasMaxLength(5);
            entity.Property(e => e.Intern).HasDefaultValue(true);
            entity.Property(e => e.Nume).HasMaxLength(100);
        });

        modelBuilder.Entity<GradDidactic>(entity =>
        {
            entity.HasKey(e => e.GradDidacticId).HasName("PK__GradDida__CA377CB3D2A6EA2F");

            entity.ToTable("GradDidactic");

            entity.Property(e => e.GradDidacticId).HasColumnName("GradDidacticID");
            entity.Property(e => e.GradDidacticCategorieId).HasColumnName("GradDidacticCategorieID");
            entity.Property(e => e.NumeGrad).HasMaxLength(20);
        });

        modelBuilder.Entity<GradDidacticNormabil>(entity =>
        {
            entity.ToTable("GradDidacticNormabil");

            entity.Property(e => e.GradDidacticNormabilId)
                .ValueGeneratedNever()
                .HasColumnName("GradDidacticNormabilID");

            entity.HasOne(d => d.GradDidacticNormabilNavigation).WithOne(p => p.GradDidacticNormabil)
                .HasForeignKey<GradDidacticNormabil>(d => d.GradDidacticNormabilId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GradDidacticNormabil_GradDidactic");
        });

        modelBuilder.Entity<Norma>(entity =>
        {
            entity.ToTable("Norma");

            entity.HasOne(d => d.GradDidactic).WithMany(p => p.Normas)
                .HasForeignKey(d => d.GradDidacticId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Norma_GradDidacticNormabil");

            entity.HasOne(d => d.StatDefunctie).WithMany(p => p.Normas)
                .HasForeignKey(d => d.StatDefunctieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Norma_StatDeFunctie");

            entity.HasMany(d => d.CadruDidactics).WithMany(p => p.Normas)
                .UsingEntity<Dictionary<string, object>>(
                    "NormaCadruDidactic",
                    r => r.HasOne<CadruDidacticGradDidactic>().WithMany()
                        .HasForeignKey("CadruDidacticId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NormaCadruDidactic_CadruDidacticGradDidactic"),
                    l => l.HasOne<Norma>().WithMany()
                        .HasForeignKey("NormaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NormaCadruDidactic_Norma"),
                    j =>
                    {
                        j.HasKey("NormaId", "CadruDidacticId");
                        j.ToTable("NormaCadruDidactic");
                        j.IndexerProperty<int>("NormaId").HasColumnName("NormaID");
                        j.IndexerProperty<int>("CadruDidacticId").HasColumnName("CadruDidacticID");
                    });
        });

        modelBuilder.Entity<NormaDisciplina>(entity =>
        {
            entity.HasKey(e => e.NormaDisciplinaId).HasName("PK_NormaDisciplina_1");

            entity.ToTable("NormaDisciplina");

            entity.Property(e => e.NormaDisciplinaId).HasColumnName("NormaDisciplinaID");
            entity.Property(e => e.DisciplinaId).HasColumnName("DisciplinaID");
            entity.Property(e => e.NormaId).HasColumnName("NormaID");

            entity.HasOne(d => d.Disciplina).WithMany(p => p.NormaDisciplinas)
                .HasForeignKey(d => d.DisciplinaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NormaDisciplina_Disciplina");

            entity.HasOne(d => d.Norma).WithMany(p => p.NormaDisciplinas)
                .HasForeignKey(d => d.NormaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NormaDisciplina_Norma");
        });

        modelBuilder.Entity<ProgramDeStudii>(entity =>
        {
            entity.HasKey(e => e.ProgramStudiiId).HasName("PK__ProgramD__7559F6767B3B4F3D");

            entity.ToTable("ProgramDeStudii");

            entity.Property(e => e.ProgramStudiiId).HasColumnName("ProgramStudiiID");
            entity.Property(e => e.Acronim)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.CoeficientKActivitatePractica)
                .HasColumnType("decimal(18, 10)")
                .HasColumnName("CoeficientK_ActivitatePractica");
            entity.Property(e => e.CoeficientKActivitateTeoretica)
                .HasColumnType("decimal(18, 10)")
                .HasColumnName("CoeficientK_ActivitateTeoretica");
            entity.Property(e => e.FacultateId).HasColumnName("FacultateID");
            entity.Property(e => e.LimbaStudiu).HasMaxLength(20);
            entity.Property(e => e.Nume).HasMaxLength(100);
            entity.Property(e => e.TipProgramId).HasColumnName("TipProgramID");

            entity.HasOne(d => d.Facultate).WithMany(p => p.ProgramDeStudiis)
                .HasForeignKey(d => d.FacultateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProgramDe__Facul__68487DD7");

            entity.HasOne(d => d.TipProgram).WithMany(p => p.ProgramDeStudiis)
                .HasForeignKey(d => d.TipProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProgramDe__TipPr__6A30C649");
        });

        modelBuilder.Entity<Promotie>(entity =>
        {
            entity.ToTable("Promotie");

            entity.Property(e => e.PromotieId).HasColumnName("PromotieID");
            entity.Property(e => e.ProgramDeStudiiId).HasColumnName("ProgramDeStudiiID");

            entity.HasOne(d => d.ProgramDeStudii).WithMany(p => p.Promoties)
                .HasForeignKey(d => d.ProgramDeStudiiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Promotie_ProgramDeStudii1");
        });

        modelBuilder.Entity<StatDeFunctie>(entity =>
        {
            entity.HasKey(e => e.StatDeFunctieId).HasName("PK__StatDeFu__B2CB3747FC6D4B16");

            entity.ToTable("StatDeFunctie");

            entity.Property(e => e.StatDeFunctieId).HasColumnName("StatDeFunctieID");
            entity.Property(e => e.DepartamentId).HasColumnName("DepartamentID");

            entity.HasOne(d => d.Departament).WithMany(p => p.StatDeFuncties)
                .HasForeignKey(d => d.DepartamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatDeFunctie_Departament");
        });

        modelBuilder.Entity<TipProgramStudiu>(entity =>
        {
            entity.HasKey(e => e.TipProgramStudiuId).HasName("PK__TipProgr__214DBCAE8B10DBE5");

            entity.ToTable("TipProgramStudiu");

            entity.Property(e => e.TipProgramStudiuId).HasColumnName("TipProgramStudiuID");
            entity.Property(e => e.DenumireTipProgram).HasMaxLength(20);
        });

        modelBuilder.Entity<Utilizator>(entity =>
        {
            entity.HasKey(e => e.UtilizatorId).HasName("PK__Utilizat__73EB10D42D0688FE");

            entity.ToTable("Utilizator");

            entity.Property(e => e.UtilizatorId).HasColumnName("UtilizatorID");
            entity.Property(e => e.Nume).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

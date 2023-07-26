using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BackEnd.Models
{
    public partial class DBVETPETContext : DbContext
    {
        public DBVETPETContext()
        {
        }

        public DBVETPETContext(DbContextOptions<DBVETPETContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cargo> Cargos { get; set; } = null!;
        public virtual DbSet<Citum> Cita { get; set; } = null!;
        public virtual DbSet<Cliente> Clientes { get; set; } = null!;
        public virtual DbSet<Empleado> Empleados { get; set; } = null!;
        public virtual DbSet<Historiaclinica> Historiaclinicas { get; set; } = null!;
        public virtual DbSet<Mascotum> Mascota { get; set; } = null!;
        public virtual DbSet<Rol> Rols { get; set; } = null!;
        public virtual DbSet<Servicio> Servicios { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=LAPTOP-89KUG9V2\\SQLEXPRESS;Initial Catalog=DBVETPET;Integrated Security=SSPI; User ID=sa;Password=********;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cargo>(entity =>
            {
                entity.ToTable("CARGO");

                entity.Property(e => e.CargoId).HasColumnName("cargoId");

                entity.Property(e => e.Cargo1)
                    .HasMaxLength(100)
                    .HasColumnName("cargo");

                entity.Property(e => e.Especialidad)
                    .HasMaxLength(100)
                    .HasColumnName("especialidad");

                entity.Property(e => e.Sueldo)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("sueldo");
            });

            modelBuilder.Entity<Citum>(entity =>
            {
                entity.HasKey(e => e.NroCita)
                    .HasName("PK__CITA__B20DB1FBD1F16A0B");

                entity.ToTable("CITA");

                entity.Property(e => e.NroCita).HasColumnName("nroCita");

                entity.Property(e => e.ClienteId).HasColumnName("clienteId");

                entity.Property(e => e.Estado)
                    .HasMaxLength(30)
                    .HasColumnName("estado");

                entity.Property(e => e.FechaCita)
                    .HasColumnType("date")
                    .HasColumnName("fechaCita");

                entity.Property(e => e.FechaRegistro)
                    .HasColumnType("date")
                    .HasColumnName("fechaRegistro");

                entity.Property(e => e.Hora)
                    .HasMaxLength(20)
                    .HasColumnName("hora");

                entity.Property(e => e.MascotaId).HasColumnName("mascotaId");

                entity.Property(e => e.TipoServicio)
                    .HasMaxLength(50)
                    .HasColumnName("tipoServicio");

                entity.HasOne(d => d.Cliente)
                    .WithMany(p => p.Cita)
                    .HasForeignKey(d => d.ClienteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clientes");

                entity.HasOne(d => d.Mascota)
                    .WithMany(p => p.Cita)
                    .HasForeignKey(d => d.MascotaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Mascota");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("CLIENTE");

                entity.HasIndex(e => e.Email, "UQ__CLIENTE__AB6E6164637B4CD4")
                    .IsUnique();

                entity.HasIndex(e => e.Dni, "UQ__CLIENTE__D87608A747B89BE0")
                    .IsUnique();

                entity.Property(e => e.ClienteId).HasColumnName("clienteId");

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(100)
                    .HasColumnName("apellidos");

                entity.Property(e => e.Celular)
                    .HasMaxLength(9)
                    .IsUnicode(false)
                    .HasColumnName("celular")
                    .IsFixedLength();

                entity.Property(e => e.Dni)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dni")
                    .IsFixedLength();

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .HasColumnName("email");

                entity.Property(e => e.Nombres)
                    .HasMaxLength(100)
                    .HasColumnName("nombres");
            });

            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("EMPLEADO");

                entity.HasIndex(e => e.CargoId, "UQ__EMPLEADO__7E9F06A44E20D8EB")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__EMPLEADO__AB6E61646955606B")
                    .IsUnique();

                entity.HasIndex(e => e.Dni, "UQ__EMPLEADO__D87608A7A5EDA1DE")
                    .IsUnique();

                entity.Property(e => e.EmpleadoId).HasColumnName("empleadoId");

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(100)
                    .HasColumnName("apellidos");

                entity.Property(e => e.CargoId).HasColumnName("cargoId");

                entity.Property(e => e.Celular)
                    .HasMaxLength(9)
                    .IsUnicode(false)
                    .HasColumnName("celular")
                    .IsFixedLength();

                entity.Property(e => e.Dni)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dni")
                    .IsFixedLength();

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .HasColumnName("email");

                entity.Property(e => e.Nombres)
                    .HasMaxLength(100)
                    .HasColumnName("nombres");

                entity.HasOne(d => d.Cargo)
                    .WithOne(p => p.Empleado)
                    .HasForeignKey<Empleado>(d => d.CargoId)
                    .HasConstraintName("FK__EMPLEADO__cargoI__47DBAE45");
            });

            modelBuilder.Entity<Historiaclinica>(entity =>
            {
                entity.HasKey(e => e.HistoriaId)
                    .HasName("PK__HISTORIA__B3A41D2CB76A80F8");

                entity.ToTable("HISTORIACLINICA");

                entity.Property(e => e.HistoriaId).HasColumnName("historiaId");

                entity.Property(e => e.Diagnostico).HasColumnName("diagnostico");

                entity.Property(e => e.EmpleadoId).HasColumnName("empleadoId");

                entity.Property(e => e.FechaConsulta)
                    .HasColumnType("date")
                    .HasColumnName("fechaConsulta");

                entity.Property(e => e.MascotaId).HasColumnName("mascotaId");

                entity.Property(e => e.Sintomas).HasColumnName("sintomas");

                entity.Property(e => e.Tratamiento).HasColumnName("tratamiento");

                entity.HasOne(d => d.Empleado)
                    .WithMany(p => p.Historiaclinicas)
                    .HasForeignKey(d => d.EmpleadoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HISTORIAC__emple__5165187F");

                entity.HasOne(d => d.Mascota)
                    .WithMany(p => p.Historiaclinicas)
                    .HasForeignKey(d => d.MascotaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HISTORIAC__masco__5070F446");
            });

            modelBuilder.Entity<Mascotum>(entity =>
            {
                entity.HasKey(e => e.MascotaId)
                    .HasName("PK__MASCOTA__DE2B26BF751CE426");

                entity.ToTable("MASCOTA");

                entity.Property(e => e.MascotaId).HasColumnName("mascotaId");

                entity.Property(e => e.ClienteId).HasColumnName("clienteId");

                entity.Property(e => e.Color)
                    .HasMaxLength(20)
                    .HasColumnName("color");

                entity.Property(e => e.FechaNacimiento)
                    .HasColumnType("date")
                    .HasColumnName("fechaNacimiento");

                entity.Property(e => e.Foto)
                    .HasMaxLength(500)
                    .HasColumnName("foto");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(60)
                    .HasColumnName("nombre");

                entity.Property(e => e.Raza)
                    .HasMaxLength(30)
                    .HasColumnName("raza");

                entity.Property(e => e.Sexo)
                    .HasMaxLength(20)
                    .HasColumnName("sexo");

                entity.Property(e => e.TipoMascota)
                    .HasMaxLength(30)
                    .HasColumnName("tipoMascota");

                entity.HasOne(d => d.Cliente)
                    .WithMany(p => p.Mascota)
                    .HasForeignKey(d => d.ClienteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cliente");
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("ROL");

                entity.HasIndex(e => e.Tipo, "UQ__ROL__E7F9564908BDE74A")
                    .IsUnique();

                entity.Property(e => e.RolId).HasColumnName("rolId");

                entity.Property(e => e.Tipo)
                    .HasMaxLength(60)
                    .HasColumnName("tipo");
            });

            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.ToTable("SERVICIO");

                entity.HasIndex(e => e.Nombre, "UQ__SERVICIO__72AFBCC6F0C342DB")
                    .IsUnique();

                entity.Property(e => e.ServicioId).HasColumnName("servicioId");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Imagen)
                    .HasMaxLength(500)
                    .HasColumnName("imagen");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("nombre");

                entity.Property(e => e.Precio)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("precio");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIO");

                entity.HasIndex(e => e.UserName, "UQ__USUARIO__66DCF95C371C11D8")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__USUARIO__AB6E61646FB8E6BD")
                    .IsUnique();

                entity.HasIndex(e => e.Dni, "UQ__USUARIO__D87608A7BD7F45A7")
                    .IsUnique();

                entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(100)
                    .HasColumnName("apellidos");

                entity.Property(e => e.Celular)
                    .HasMaxLength(9)
                    .IsUnicode(false)
                    .HasColumnName("celular")
                    .IsFixedLength();

                entity.Property(e => e.Dni)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dni")
                    .IsFixedLength();

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .HasColumnName("email");

                entity.Property(e => e.Nombres)
                    .HasMaxLength(100)
                    .HasColumnName("nombres");

                entity.Property(e => e.Password)
                    .HasMaxLength(150)
                    .HasColumnName("password");

                entity.Property(e => e.UserName)
                    .HasMaxLength(60)
                    .HasColumnName("userName");

                entity.HasMany(d => d.Rols)
                    .WithMany(p => p.Usuarios)
                    .UsingEntity<Dictionary<string, object>>(
                        "UsuarioRol",
                        l => l.HasOne<Rol>().WithMany().HasForeignKey("RolId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__USUARIO_R__rolId__59FA5E80"),
                        r => r.HasOne<Usuario>().WithMany().HasForeignKey("UsuarioId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__USUARIO_R__usuar__59063A47"),
                        j =>
                        {
                            j.HasKey("UsuarioId", "RolId").HasName("PK__USUARIO___F0F188ED781D47F4");

                            j.ToTable("USUARIO_ROL");

                            j.IndexerProperty<int>("UsuarioId").HasColumnName("usuarioId");

                            j.IndexerProperty<int>("RolId").HasColumnName("rolId");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

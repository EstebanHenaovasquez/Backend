using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AmarantaAPI.Models;

public partial class AmarantaFinalContext : DbContext
{
    public AmarantaFinalContext()
    {
    }

    public AmarantaFinalContext(DbContextOptions<AmarantaFinalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Abono> Abonos { get; set; }

    public virtual DbSet<CProducto> CProductos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<DetallesCompra> DetallesCompras { get; set; }

    public virtual DbSet<DetallesPedido> DetallesPedidos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=SIUUU\\SQLEXPRESS;Initial Catalog=AmarantaFinal;integrated security=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abono>(entity =>
        {
            entity.HasKey(e => e.CodigoAbono).HasName("PK__ABONOS__67A22730E411BBCE");

            entity.ToTable("ABONOS");

            entity.Property(e => e.CodigoAbono).HasColumnName("Codigo_Abono");
            entity.Property(e => e.FechaAbono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Fecha_Abono");
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__ABONOS__Id_Usuar__6754599E");
        });

        modelBuilder.Entity<CProducto>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__C_PRODUC__CB90334940C1F1EA");

            entity.ToTable("C_PRODUCTOS");

            entity.Property(e => e.IdCategoria).HasColumnName("Id_Categoria");
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_Categoria");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__CLIENTES__3DD0A8CB773B9E4C");

            entity.ToTable("CLIENTES");

            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Departamento)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Documento)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdRol).HasColumnName("Id_Rol");
            entity.Property(e => e.ImagenPerfil).HasColumnName("imagenPerfil");
            entity.Property(e => e.Municipio)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__CLIENTES__Id_Rol__5CD6CB2B");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.CodigoCompra).HasName("PK__COMPRAS__080384A8FDCAB73D");

            entity.ToTable("COMPRAS");

            entity.Property(e => e.CodigoCompra).HasColumnName("Codigo_Compra");
            entity.Property(e => e.IdProveedor).HasColumnName("Id_Proveedor");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaCompra)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Fecha_Compra");
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.PrecioTotal).HasColumnName("Precio_Total");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__COMPRAS__Id_Usua__5535A963");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdProveedor)
                .HasConstraintName("FK__DETALLES___Id_Pr__5812160E");
        });

        modelBuilder.Entity<DetallesCompra>(entity =>
        {
            entity.HasKey(e => e.CodigoDetalleCompra).HasName("PK__DETALLES__B7A15FE7CC8A526A");

            entity.ToTable("DETALLES_COMPRA");

            entity.Property(e => e.CodigoDetalleCompra).HasColumnName("Codigo_Detalle_Compra");
            entity.Property(e => e.CodigoCompra).HasColumnName("Codigo_Compra");
            entity.Property(e => e.CodigoProducto).HasColumnName("Codigo_Producto");
            

            entity.HasOne(d => d.CodigoCompraNavigation).WithMany(p => p.DetallesCompras)
                .HasForeignKey(d => d.CodigoCompra)
                .HasConstraintName("FK__DETALLES___Codig__59063A47");

            entity.HasOne(d => d.CodigoProductoNavigation).WithMany(p => p.DetallesCompras)
                .HasForeignKey(d => d.CodigoProducto)
                .HasConstraintName("FK__DETALLES___Codig__59FA5E80");

            
        });

        modelBuilder.Entity<DetallesPedido>(entity =>
        {
            entity.HasKey(e => e.CodigoDetallePedido).HasName("PK__DETALLES__571176FD67B4BDFB");

            entity.ToTable("DETALLES_PEDIDOS");

            entity.Property(e => e.CodigoDetallePedido).HasColumnName("Codigo_Detalle_Pedido");
            entity.Property(e => e.CodigoPedido).HasColumnName("Codigo_Pedido");
            entity.Property(e => e.CodigoProducto).HasColumnName("Codigo_Producto");

            entity.HasOne(d => d.CodigoPedidoNavigation).WithMany(p => p.DetallesPedidos)
                .HasForeignKey(d => d.CodigoPedido)
                .HasConstraintName("FK__DETALLES___Codig__6383C8BA");

            entity.HasOne(d => d.CodigoProductoNavigation).WithMany(p => p.DetallesPedidos)
                .HasForeignKey(d => d.CodigoProducto)
                .HasConstraintName("FK__DETALLES___Codig__6477ECF3");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.CodigoPedido).HasName("PK__PEDIDOS__5B8968249B5AED3C");

            entity.ToTable("PEDIDOS");

            entity.Property(e => e.CodigoPedido).HasColumnName("Codigo_Pedido");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaPedido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Fecha_Pedido");
            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.PrecioTotal).HasColumnName("Precio_Total");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__PEDIDOS__Id_Clie__60A75C0F");

            
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.CodigoProducto).HasName("PK__PRODUCTO__060DB9E80B154C7D");

            entity.ToTable("PRODUCTOS");

            entity.Property(e => e.CodigoProducto).HasColumnName("Codigo_Producto");
            entity.Property(e => e.IdCategoria).HasColumnName("Id_Categoria");
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_Producto");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK__PRODUCTOS__Id_Ca__5070F446");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__PROVEEDO__477B858E876DACCB");

            entity.ToTable("PROVEEDORES");

            entity.Property(e => e.IdProveedor).HasColumnName("Id_Proveedor");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nit)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NIT");
            entity.Property(e => e.NombreEmpresa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_Empresa");
            entity.Property(e => e.Representante)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__ROLES__55932E86DA0E5110");

            entity.ToTable("ROLES");

            entity.Property(e => e.IdRol).HasColumnName("Id_Rol");
            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_Rol");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__USUARIOS__63C76BE27DF673D4");

            entity.ToTable("USUARIOS");

            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Departamento)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Documento)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdRol).HasColumnName("Id_Rol");
            entity.Property(e => e.ImagenPerfil).HasColumnName("imagenPerfil");
            entity.Property(e => e.Municipio)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__USUARIOS__Id_Rol__4BAC3F29");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

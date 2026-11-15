using Microsoft.EntityFrameworkCore;
using LeiaCompartilhe.API.Models;

namespace LeiaCompartilhe.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Livro> Livros => Set<Livro>();
    public DbSet<Emprestimo> Emprestimos => Set<Emprestimo>();
    public DbSet<Doacao> Doacoes => Set<Doacao>();
    public DbSet<Parceria> Parcerias => Set<Parceria>();
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(u => u.EmailNormalizado)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(u => u.EmailNormalizado)
                .IsUnique();

            entity.Property(u => u.SenhaHash)
                .IsRequired();

            entity.Property(u => u.Ru)
                .HasMaxLength(30);

            entity.HasIndex(u => u.Ru)
                .IsUnique();
        });

        modelBuilder.Entity<Livro>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(l => l.Autor)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(l => l.Genero)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(l => l.Condicao)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(l => l.Observacoes)
                .HasMaxLength(1000);
        });

        modelBuilder.Entity<Emprestimo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Status)
                .IsRequired();

            entity.HasIndex(e => new { e.LivroId, e.Status });

            // Livro relacionado ao empréstimo
            entity.HasOne<Livro>()
                .WithMany()
                .HasForeignKey(e => e.LivroId)
                .OnDelete(DeleteBehavior.Restrict);

            // Aluno que solicitou o empréstimo
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Educador ou administrador que aprovou
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(e => e.ResponsavelAprovacaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Educador ou administrador que reprovou
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(e => e.ResponsavelReprovacaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Responsável pela retirada
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(e => e.ResponsavelRetiradaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Responsável pela devolução
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(e => e.ResponsavelDevolucaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Responsável pelo cancelamento
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(e => e.ResponsavelCancelamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Doacao>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(d => d.Autor)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(d => d.Genero)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(d => d.Condicao)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(d => d.Observacoes)
                .HasMaxLength(1000);

            entity.Property(d => d.Parecer)
                .HasMaxLength(1000);

            entity.Property(d => d.Status)
                .IsRequired();

            // Pessoa que realizou a doação
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(d => d.UsuarioDoadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Educador ou administrador que decidiu
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(d => d.UsuarioDecisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Livro criado após a aprovação
            entity.HasOne<Livro>()
                .WithOne()
                .HasForeignKey<Doacao>(d => d.LivroId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<Parceria>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.NomeInstituicao)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(p => p.NomeInstituicao)
                .IsUnique();

            entity.Property(p => p.TipoParceria)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Descricao)
                .HasMaxLength(1000);

            entity.Property(p => p.Contato)
                .HasMaxLength(150);

            entity.Property(p => p.Status)
                .IsRequired();
        });

        modelBuilder.Entity<RegistroAuditoria>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Acao)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(r => r.Entidade)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(r => r.Detalhes)
                .HasMaxLength(2000);

            entity.HasIndex(r => r.DataHora);

            // Usuário que executou a ação
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(r => r.UsuarioResponsavelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuário cujo perfil foi afetado, quando aplicável
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(r => r.UsuarioAlvoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

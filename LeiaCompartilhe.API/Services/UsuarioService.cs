using LeiaCompartilhe.API.Data;
using LeiaCompartilhe.API.DTOs;
using LeiaCompartilhe.API.Interfaces;
using LeiaCompartilhe.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;

namespace LeiaCompartilhe.API.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    private readonly IAuditoriaService _auditoriaService;

    public UsuarioService(AppDbContext context,IConfiguration configuration,IAuditoriaService auditoriaService)
    {
        _context = context;
        _configuration = configuration;
        _auditoriaService = auditoriaService;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    private readonly IConfiguration _configuration;

    public async Task<UsuarioRespostaDto> RegistrarAsync(UsuarioRegistroDto dto)
    {
        string email = dto.Email.Trim();
        string emailNormalizado = email.ToLowerInvariant();

        bool emailExiste = await _context.Usuarios.AnyAsync(u => u.EmailNormalizado == emailNormalizado);

        if (emailExiste)
            throw new InvalidOperationException("Este e-mail já está cadastrado.");

        bool ruExiste = await _context.Usuarios.AnyAsync(u => u.Ru == dto.Ru.Trim());

        if (ruExiste)
            throw new InvalidOperationException("Já existe um usuário cadastrado com este RU.");

        var usuario = new Usuario
        {
            Nome = dto.Nome.Trim(),
            Instituicao = dto.Instituicao.Trim(),
            Email = email,
            EmailNormalizado = emailNormalizado,
            Ru = dto.Ru.Trim(),
            TipoUsuario = TipoUsuario.Aluno,
            Status = StatusUsuario.Ativo,
            DataCriacao = DateTime.UtcNow
        };

        usuario.SenhaHash = _passwordHasher.HashPassword(usuario,dto.Senha);

        _context.Usuarios.Add(usuario);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.GetBaseException() is MySqlException mysqlEx && mysqlEx.Number == 1062)
        {
            throw new InvalidOperationException("Já existe um usuário cadastrado com o e-mail ou RU informado.");
        }

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: usuario.Id,
            acao: "Cadastrar",
            entidade: "Usuario",
            entidadeId: usuario.Id,
            usuarioAlvoId: usuario.Id,
            detalhes: "Novo aluno realizou o próprio cadastro."
        );

        return new UsuarioRespostaDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Ru = usuario.Ru,
            TipoUsuario = usuario.TipoUsuario.ToString(),
            Status = usuario.Status.ToString()
        };
    }
    public async Task<UsuarioPerfilDto?> BuscarPerfilAsync(int usuarioId)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == usuarioId)
            .Select(u => new UsuarioPerfilDto
            {
                Nome = u.Nome,
                Email = u.Email,
                Ru = u.Ru,
                Instituicao = u.Instituicao
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UsuarioRespostaDto> CadastrarEducadorAsync(EducadorCadastroDto dto,int administradorId)
    {
        string email = dto.Email.Trim();
        string emailNormalizado = email.ToLowerInvariant();
        string ru = dto.Ru.Trim();

        bool emailExiste = await _context.Usuarios.AnyAsync(u => u.EmailNormalizado == emailNormalizado);

        if (emailExiste)
            throw new InvalidOperationException("Este e-mail já está cadastrado.");

        bool ruExiste = await _context.Usuarios.AnyAsync(u => u.Ru == ru);

        if (ruExiste)
            throw new InvalidOperationException("Já existe um usuário cadastrado com este RU.");

        var educador = new Usuario
        {
            Nome = dto.Nome.Trim(),
            Instituicao = dto.Instituicao.Trim(),
            Email = email,
            EmailNormalizado = emailNormalizado,
            Ru = ru,
            TipoUsuario = TipoUsuario.Educador,
            Status = StatusUsuario.Ativo,
            DataCriacao = DateTime.UtcNow
        };

        educador.SenhaHash = _passwordHasher.HashPassword(educador,dto.Senha);

        _context.Usuarios.Add(educador);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.GetBaseException() is MySqlException mysqlEx && mysqlEx.Number == 1062)
        {
            throw new InvalidOperationException("Já existe um usuário cadastrado com o e-mail ou RU informado.");
        }

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: administradorId,
            acao: "Cadastrar",
            entidade: "Usuario",
            entidadeId: educador.Id,
            usuarioAlvoId: educador.Id,
            detalhes: "Administrador cadastrou um novo educador."
        );

        return new UsuarioRespostaDto
        {
            Id = educador.Id,
            Nome = educador.Nome,
            Email = educador.Email,
            Ru = educador.Ru,
            TipoUsuario = educador.TipoUsuario.ToString(),
            Status = educador.Status.ToString()
        };
    }

    public async Task<UsuarioLoginRespostaDto> LoginAsync(UsuarioLoginDto dto)
    {
        string emailNormalizado = dto.Email.Trim().ToLowerInvariant();

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.EmailNormalizado == emailNormalizado);

        if (usuario == null)
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        if (usuario.Status != StatusUsuario.Ativo)
            throw new UnauthorizedAccessException("Usuário inativo.");

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            dto.Senha
        );

        if (resultado == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.SenhaHash = _passwordHasher.HashPassword(usuario,dto.Senha);

            await _context.SaveChangesAsync();
        }

        return GerarToken(usuario);
    }

    public async Task CriarAdministradorInicialAsync()
    {
        bool administradorExiste = await _context.Usuarios.AnyAsync(u => u.TipoUsuario == TipoUsuario.Administrador);

        if (administradorExiste)
            return;

        var configuracao = _configuration.GetSection("AdministradorInicial");

        string nome = configuracao["Nome"] ?? string.Empty;
        string email = configuracao["Email"] ?? string.Empty;
        string senha = configuracao["Senha"] ?? string.Empty;
        string? ru = configuracao["Ru"];

        nome = nome.Trim();
        email = email.Trim();

        if (string.IsNullOrWhiteSpace(nome) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException("As configurações do Administrador inicial não foram preenchidas.");
        }

        if (nome.Length > 120)
            throw new InvalidOperationException("O nome do Administrador inicial deve ter no máximo 120 caracteres.");

        if (email.Length > 150 ||
            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
        {
            throw new InvalidOperationException("O e-mail do Administrador inicial é inválido.");
        }

        if (senha.Length < 8)
            throw new InvalidOperationException("A senha do Administrador inicial deve ter pelo menos 8 caracteres.");

        if (string.IsNullOrWhiteSpace(ru))
            ru = null;
        else
        {
            ru = ru.Trim();

            if (ru.Length > 30)
                throw new InvalidOperationException("O RU do Administrador inicial deve ter no máximo 30 caracteres.");
        }

        string emailNormalizado = email.Trim().ToLowerInvariant();

        bool emailExiste = await _context.Usuarios.AnyAsync(u => u.EmailNormalizado == emailNormalizado);

        if (emailExiste)
            throw new InvalidOperationException("O e-mail configurado para o Administrador inicial já pertence a outro usuário.");

        if (ru != null)
        {
            bool ruExiste = await _context.Usuarios.AnyAsync(u => u.Ru == ru);

            if (ruExiste)
                throw new InvalidOperationException("O RU configurado para o Administrador inicial já pertence a outro usuário.");
        }

        var administrador = new Usuario
        {
            Nome = nome.Trim(),
            Email = email.Trim(),
            EmailNormalizado = emailNormalizado,
            Ru = ru,
            TipoUsuario = TipoUsuario.Administrador,
            Status = StatusUsuario.Ativo,
            DataCriacao = DateTime.UtcNow
        };

        administrador.SenhaHash = _passwordHasher.HashPassword(administrador,senha);

        _context.Usuarios.Add(administrador);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.GetBaseException() is MySqlException mysqlEx
                  && mysqlEx.Number == 1062)
        {
            throw new InvalidOperationException("Já existe um usuário cadastrado com o e-mail ou RU informado para o Administrador inicial.");
        }

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: administrador.Id,
            acao: "Criar administrador inicial",
            entidade: "Usuario",
            entidadeId: administrador.Id,
            usuarioAlvoId: administrador.Id,
            detalhes: "Administrador criado automaticamente durante a inicialização da API."
        );
    }

    public async Task<List<UsuarioRespostaDto>> ListarAsync(bool incluirTodos)
    {
        var consulta = _context.Usuarios.AsNoTracking();

        if (!incluirTodos)
            consulta = consulta.Where(u => u.TipoUsuario == TipoUsuario.Aluno);

        return await consulta
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioRespostaDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Ru = u.Ru,
                TipoUsuario = u.TipoUsuario.ToString(),
                Status = u.Status.ToString()
            })
            .ToListAsync();
    }

    public async Task AlterarStatusAsync(int usuarioId, bool ativo, int administradorId)
    {
        var usuario = await _context.Usuarios.FindAsync(usuarioId);

        if (usuario == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        usuario.Status = ativo ? StatusUsuario.Ativo : StatusUsuario.Inativo;

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: administradorId,
            acao: ativo ? "Ativação" : "Desativação",
            entidade: "Usuario",
            entidadeId: usuario.Id,
            usuarioAlvoId: usuario.Id,
            detalhes: $"Status da conta alterado para {usuario.Status}."
        );
    }
    private UsuarioLoginRespostaDto GerarToken(Usuario usuario)
    {
        var chave = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Chave JWT não configurada.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Issuer JWT não configurado.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Audience JWT não configurado.");

        var expiracaoMinutos = _configuration.GetValue<int>("Jwt:ExpirationMinutes");

        var chaveBytes = Encoding.UTF8.GetBytes(chave);

        var chaveSegura = new SymmetricSecurityKey(chaveBytes);

        var credenciais = new SigningCredentials(chaveSegura,SecurityAlgorithms.HmacSha256);

        var expiracao = DateTime.UtcNow.AddMinutes(expiracaoMinutos);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nome),
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiracao,
            signingCredentials: credenciais
        );

        return new UsuarioLoginRespostaDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiracao = expiracao,
            Usuario = new UsuarioRespostaDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Ru = usuario.Ru,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Status = usuario.Status.ToString()
            }
        };
    }
}
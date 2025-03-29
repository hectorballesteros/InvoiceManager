using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using InvoiceManager.Domain.Interfaces;
using InvoiceManager.Domain.Entities;
using InvoiceManager.Api.Models;

/// <summary>
/// Controlador para autenticación de usuarios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Inicia sesión con credenciales de usuario.
    /// </summary>
    /// <param name="request">Credenciales de acceso (usuario y contraseña).</param>
    /// <returns>Token JWT en cookie y datos del usuario autenticado.</returns>
    /// <response code="200">Inicio de sesión exitoso.</response>
    /// <response code="401">Credenciales inválidas.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.Authenticate(request.Username, request.Password);

        if (user == null)
            return Unauthorized(new { message ="Credenciales inválidas, revisa tu usuario y contraseña."});

        var token = _authService.GenerateToken(user);

        Response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(new { user.Username, user.Role });
    }

    /// <summary>
    /// Cierra sesión eliminando la cookie del token.
    /// </summary>
    /// <returns>Confirmación de cierre de sesión.</returns>
    /// <response code="200">Sesión cerrada correctamente.</response>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return Ok(new { message = "Sesión cerrada" });
    }

    /// <summary>
    /// Obtiene información del usuario autenticado actual.
    /// </summary>
    /// <returns>Nombre de usuario y rol.</returns>
    /// <response code="200">Usuario autenticado retornado correctamente.</response>
    /// <response code="401">Usuario no autenticado.</response>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var username = User.Identity?.Name;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new { username, role });
    }
}

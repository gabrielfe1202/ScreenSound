using System.ComponentModel.DataAnnotations;

namespace ScreenSound.WebApp.Requests;
public record ArtistaRequest([Required] string nome, [Required] string bio, string? fotoPerfil);


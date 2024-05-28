namespace ScreenSound.WebApp.Response;

public record MusicaResponse(int Id, string Nome, int ArtistaId, string NomeArtista, int? anoLancamento, string? fotoArtista);
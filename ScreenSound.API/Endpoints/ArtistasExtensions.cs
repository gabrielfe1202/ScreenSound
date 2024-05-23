using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Functions;

namespace ScreenSound.API.Endpoints;

public static class ArtistasExtensions
{
    public static void AddEndPointsArtistas(this WebApplication app)
    {

        var groupBuilder = app.MapGroup("Artistas").RequireAuthorization().WithTags("Artistas");

        groupBuilder.MapGet("", ([FromServices] DAL<Artista> dal) =>
        {
            var listaDeArtistas = dal.Listar();
            if (listaDeArtistas is null)
            {
                return Results.NotFound();
            }
            var listaDeArtistaResponse = EntityListToResponseList(listaDeArtistas);
            return Results.Ok(listaDeArtistaResponse);
        });

        groupBuilder.MapGet("{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
        {
            var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
            if (artista is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(EntityToResponse(artista));

        });

        groupBuilder.MapPost("", async ([FromServices] IHostEnvironment env, [FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
        {
            var nome = TextFunctions.RemoveSpacesSpecialCharactersAndAccents(artistaRequest.nome.Trim());
            var imagemArtista = DateTime.Now.ToString("ddMMyyyyhhmmss") + nome + ".jpg";
            var path = Path.Combine(env.ContentRootPath, "wwwroot", "FotosPerfil", imagemArtista);

            using MemoryStream ms = new MemoryStream(Convert.FromBase64String(artistaRequest.fotoPerfil!));
            using FileStream fs = new(path, FileMode.Create);
            await ms.CopyToAsync(fs);

            var artista = new Artista(artistaRequest.nome, artistaRequest.bio)
            {
                FotoPerfil = $"/FotosPerfil/{imagemArtista}"
            };

            dal.Adicionar(artista);
            return Results.Ok();
        });

        groupBuilder.MapDelete("{id}", ([FromServices] DAL<Artista> dal, int id) =>
        {
            var artista = dal.RecuperarPor(a => a.Id == id);
            if (artista is null)
            {
                return Results.NotFound();
            }
            dal.Deletar(artista);
            return Results.NoContent();

        });

        groupBuilder.MapPut("", async ([FromServices] IHostEnvironment env, [FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artistaRequestEdit) =>
        {
            var artistaAtualizar = dal.RecuperarPor(a => a.Id == artistaRequestEdit.Id);

            if (artistaAtualizar is null)
            {
                return Results.NotFound();
            }

            if (!string.IsNullOrEmpty(artistaRequestEdit.fotoPerfil) && artistaRequestEdit.fotoPerfil != artistaAtualizar.FotoPerfil)
            {
                if (!string.IsNullOrEmpty(artistaAtualizar.FotoPerfil))
                {
                    var pathImagemAntiga = Path.Combine(env.ContentRootPath, "wwwroot", "FotosPerfil", artistaAtualizar.FotoPerfil.Split("/")[2]);
                    if (File.Exists(pathImagemAntiga))
                    {
                        File.Delete(pathImagemAntiga);
                    }
                }
                string nome = TextFunctions.RemoveSpacesSpecialCharactersAndAccents(artistaRequestEdit.nome.Trim());
                string imagemArtista = DateTime.Now.ToString("ddMMyyyyhhmmss") + nome + ".jpg";
                string path = Path.Combine(env.ContentRootPath, "wwwroot", "FotosPerfil", imagemArtista);

                using MemoryStream ms = new MemoryStream(Convert.FromBase64String(artistaRequestEdit.fotoPerfil!));
                using FileStream fs = new(path, FileMode.Create);
                await ms.CopyToAsync(fs);
                artistaAtualizar.FotoPerfil = $"/FotosPerfil/{imagemArtista}";
            }

            artistaAtualizar.Nome = artistaRequestEdit.nome;
            artistaAtualizar.Bio = artistaRequestEdit.bio;
            dal.Atualizar(artistaAtualizar);
            return Results.Ok();
        });
    }

    private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
    {
        return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
    }

    private static ArtistaResponse EntityToResponse(Artista artista)
    {
        return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
    }


}

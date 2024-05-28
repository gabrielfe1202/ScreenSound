namespace ScreenSound.WebApp.Response;
public record GeneroResponse(int Id, string Nome, string Descricao)
{
    public override string ToString()
    {
        return $"{Nome}";
    }
}
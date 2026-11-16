
namespace Tooded_EFCore;

public static class AktiivneKasutaja
{
    public static int Id { get; set; }
    public static string Nimi { get; set; } = "";
    public static string Email { get; set; } = "";
    public static string Roll { get; set; } = "";

    public static bool OnAdmin => Roll == "admin";

    public static void LogiValja()
    {
        Id = 0;
        Nimi = "";
        Email = "";
        Roll = "";
        Ostukorv.Tyhjenda();
    }
}

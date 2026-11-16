namespace Tooded_EFCore;

public class OstukorviRida
{
    public Toode Toode { get; set; } = null!;
    public int Kogus { get; set; }

    public decimal Summa => Toode.Hind * Kogus;

    public override string ToString() =>
        $"{Toode.Toodenimetus} – {Kogus} tk × {Toode.Hind:F2} € = {Summa:F2} €";
}

public static class Ostukorv
{
    public static List<OstukorviRida> Ridad { get; } = new();

    public static decimal Kokku => Ridad.Sum(r => r.Summa);

    // Tagastab false, kui soovitud kogus ületab laoseisu.
    public static bool Lisa(Toode toode, int kogus)
    {
        if (kogus <= 0)
            return false;

        var olemas = Ridad.FirstOrDefault(r => r.Toode.Id == toode.Id);
        int korvis = olemas?.Kogus ?? 0;

        // Uus päring, et kontrollida tegelikku laoseisu andmebaasis.
        using var db = new ToodedContext();
        int? laos = db.Toodetabel
            .Where(t => t.Id == toode.Id)
            .Select(t => (int?)t.Kogus)
            .FirstOrDefault();

        if (laos == null || kogus > laos.Value - korvis)
            return false;

        if (olemas != null)
            olemas.Kogus += kogus;
        else
            Ridad.Add(new OstukorviRida { Toode = toode, Kogus = kogus });

        return true;
    }

    public static void Tyhjenda() => Ridad.Clear();
}

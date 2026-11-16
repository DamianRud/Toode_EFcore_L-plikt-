namespace Tooded_EFCore;

public class Ost
{
    public int Id { get; set; }
    public DateTime Kuupaev { get; set; }
    public decimal Kokku { get; set; }
    public List<Osturida> Read { get; set; } = new();
}

public class Osturida
{
    public int Id { get; set; }
    public int OstId { get; set; }
    public Ost Ost { get; set; } = null!;
    public int ToodeId { get; set; }
    public string TooteNimi { get; set; } = "";
    public int Kogus { get; set; }
    public decimal Hind { get; set; }
    public decimal Summa { get; set; }
}

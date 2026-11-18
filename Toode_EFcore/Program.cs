
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Kaustad.Loo();

        using (var db = new ToodedContext())
        {
            db.Database.Migrate();
            LooAlgkasutajad(db);
            LisaTestTooted(db);
        }

        Application.Run(new LoginForm());
    }

    private static void LisaTestTooted(ToodedContext db)
    {
        if (db.Toodetabel.Any()) return;

        db.Toodetabel.AddRange(
            new Toode
            {
                Toodenimetus = "Piim",
                Kogus = 20,
                Hind = 1.50m,
                KategooriaId = 1
            },
            new Toode
            {
                Toodenimetus = "Vesi",
                Kogus = 30,
                Hind = 0.80m,
                KategooriaId = 1
            },
            new Toode
            {
                Toodenimetus = "Banaan",
                Kogus = 15,
                Hind = 1.20m,
                KategooriaId = 2
            },
            new Toode
            {
                Toodenimetus = "Šokolaad",
                Kogus = 10,
                Hind = 2.50m,
                KategooriaId = 3
            },
            new Toode
            {
                Toodenimetus = "Sai",
                Kogus = 12,
                Hind = 1.10m,
                KategooriaId = 4
            }
        );

        db.SaveChanges();
    }

    private static void LooAlgkasutajad(ToodedContext db)
    {
        if (db.Kasutajad.Any()) return;

        db.Kasutajad.AddRange(
            new Kasutaja
            {
                Nimi = "Admin",
                Email = "admin@admin.ee",
                ParoolHash = PasswordHelper.Hash("admin123"),
                Roll = "admin"
            },
            new Kasutaja
            {
                Nimi = "Kasutaja",
                Email = "kasutaja@pood.ee",
                ParoolHash = PasswordHelper.Hash("kasutaja123"),
                Roll = "kasutaja"
            });

        db.SaveChanges();
    }
}

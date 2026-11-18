
namespace Tooded_EFCore;

public static class Keel
{
    public static bool English =>
        Toode_EFcore.Settings.Default.UserLanguage == "en-US";

    public static string T(string et, string en)
    {
        return English ? en : et;
    }

    public static void Muuda(string keel)
    {
        Toode_EFcore.Settings.Default.UserLanguage = keel;
        Toode_EFcore.Settings.Default.Save();
    }
}

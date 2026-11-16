
namespace Tooded_EFCore;

public partial class FormMain : Form
{
    private readonly Button btnTooted = new();
    private readonly Button btnPood = new();
    private readonly Button btnArve = new();
    private readonly Button btnStatistika = new();
    private readonly Button btnProfiil = new();
    private readonly Button btnValja = new();

    public FormMain()
    {
        InitializeComponent();

        Text = $"Peamenüü – {AktiivneKasutaja.Nimi}";
        ClientSize = new Size(260, 370);
        StartPosition = FormStartPosition.CenterScreen;

        SeaNupp(btnTooted, "Tootehaldus", 20,
            (s, e) => Ava(new Form2()));

        SeaNupp(btnPood, "Pood", 70,
            (s, e) => Ava(new Form3()));

        SeaNupp(btnArve, "Ostukorv ja arve", 120,
            (s, e) => Ava(new ArveForm()));

        SeaNupp(btnStatistika, "Müügistatistika", 170,
            (s, e) => Ava(new MuugistatistikaForm()));

        SeaNupp(btnProfiil, "Minu profiil", 220,
            (s, e) => Ava(new KasutajaProfiilForm()));

        SeaNupp(btnValja, "Logi välja", 270,
            (s, e) => Close());

        btnTooted.Enabled = AktiivneKasutaja.OnAdmin;
        btnStatistika.Enabled = AktiivneKasutaja.OnAdmin;

        btnValja.BackColor = Color.FromArgb(235, 238, 243);

        Controls.AddRange(new Control[]
        {
            btnTooted,
            btnPood,
            btnArve,
            btnStatistika,
            btnProfiil,
            btnValja
        });
    }

    private static void SeaNupp(
        Button nupp, string tekst, int y, EventHandler kasitleja)
    {
        nupp.Text = tekst;
        nupp.SetBounds(30, y, 200, 40);
        nupp.Click += kasitleja;
    }

    private static void Ava(Form vorm)
    {
        using (vorm)
            vorm.ShowDialog();
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
    }
}

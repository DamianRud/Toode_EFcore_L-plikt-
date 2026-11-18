
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading;

namespace Tooded_EFCore;

public partial class FormMain : Form
{
    private readonly Button btnTooted = new();
    private readonly Button btnPood = new();
    private readonly Button btnArve = new();
    private readonly Button btnStatistika = new();
    private readonly Button btnProfiil = new();
    private readonly Button btnValja = new();

    private readonly Button btnEST = new();
    private readonly Button btnENG = new();

    private readonly Label poeNimi = new();
    private readonly Label kirjeldus = new();
    private readonly Label pealkiri = new();
    private readonly Label alapealkiri = new();

    private bool English =>
        Toode_EFcore.Settings.Default.UserLanguage == "en-US";

    public FormMain()
    {
        InitializeComponent();

        Text = "Peamenüü";
        ClientSize = new Size(610, 760);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.FromArgb(234, 244, 255);
        Font = new System.Drawing.Font("Segoe UI", 10);
        DoubleBuffered = true;

        Controls.Clear();

        var kaart = new RoundedPanel
        {
            Bounds = new Rectangle(65, 35, 480, 690),
            BackColor = Color.White
        };

        var pais = new GradientPanel
        {
            Bounds = new Rectangle(0, 0, 480, 175)
        };

        var logo = new Label
        {
            Text = "🛒",
            Font = new System.Drawing.Font("Segoe UI Emoji", 29),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Bounds = new Rectangle(190, 18, 100, 58),
            BackColor = Color.Transparent
        };

        poeNimi.Font = new System.Drawing.Font(
            "Segoe UI", 25, FontStyle.Bold);
        poeNimi.ForeColor = Color.White;
        poeNimi.TextAlign = ContentAlignment.MiddleCenter;
        poeNimi.Bounds = new Rectangle(70, 78, 340, 48);
        poeNimi.BackColor = Color.Transparent;

        kirjeldus.Font = new System.Drawing.Font("Segoe UI", 12);
        kirjeldus.ForeColor = Color.White;
        kirjeldus.TextAlign = ContentAlignment.MiddleCenter;
        kirjeldus.Bounds = new Rectangle(60, 128, 360, 30);
        kirjeldus.BackColor = Color.Transparent;

        // Keelevalik päise paremas ülanurgas
        SeaKeeleNupp(btnEST, "EST", 388);
        SeaKeeleNupp(btnENG, "ENG", 430);

        btnEST.Click += (s, e) => MuudaKeel("et-EE");
        btnENG.Click += (s, e) => MuudaKeel("en-US");

        pais.Controls.AddRange(new Control[]
        {
            logo, poeNimi, kirjeldus, btnEST, btnENG
        });

        pealkiri.Font = new System.Drawing.Font(
            "Segoe UI", 24, FontStyle.Bold);
        pealkiri.ForeColor = Color.FromArgb(21, 49, 81);
        pealkiri.TextAlign = ContentAlignment.MiddleCenter;
        pealkiri.Bounds = new Rectangle(35, 190, 410, 55);

        alapealkiri.Font = new System.Drawing.Font(
            "Segoe UI", 10);
        alapealkiri.ForeColor =
            Color.FromArgb(105, 116, 130);
        alapealkiri.TextAlign = ContentAlignment.MiddleCenter;
        alapealkiri.Bounds = new Rectangle(25, 247, 430, 30);

        SeaNupp(btnTooted, "▣", 290,
            (s, e) => Ava(new Form2()));

        SeaNupp(btnPood, "⌂", 350,
            (s, e) => Ava(new Form3()));

        SeaNupp(btnArve, "▤", 410,
            (s, e) => Ava(new ArveForm()));

        SeaNupp(btnStatistika, "▥", 470,
            (s, e) => Ava(new MuugistatistikaForm()));

        SeaNupp(btnProfiil, "◉", 530,
            (s, e) => Ava(new KasutajaProfiilForm()));

        btnTooted.Enabled = AktiivneKasutaja.OnAdmin;
        btnStatistika.Enabled = AktiivneKasutaja.OnAdmin;

        btnValja.Bounds = new Rectangle(45, 610, 390, 55);
        btnValja.FlatStyle = FlatStyle.Flat;
        btnValja.FlatAppearance.BorderSize = 0;
        btnValja.ForeColor = Color.White;
        btnValja.BackColor = Color.FromArgb(0, 157, 142);
        btnValja.Font = new System.Drawing.Font(
            "Segoe UI", 14, FontStyle.Bold);
        btnValja.Cursor = Cursors.Hand;
        btnValja.Click += (s, e) => Close();

        btnValja.MouseEnter += (s, e) =>
            btnValja.BackColor = Color.FromArgb(0, 128, 122);

        btnValja.MouseLeave += (s, e) =>
            btnValja.BackColor = Color.FromArgb(0, 157, 142);

        kaart.Controls.AddRange(new Control[]
        {
            pais, pealkiri, alapealkiri,
            btnTooted, btnPood, btnArve,
            btnStatistika, btnProfiil, btnValja
        });

        Controls.Add(kaart);

        RakendaKeel();

        // Kui poes keelt muudeti, uuendame peamenüü
        Activated += (s, e) => RakendaKeel();
    }

    private static void SeaKeeleNupp(
        Button nupp, string tekst, int x)
    {
        nupp.Text = tekst;
        nupp.SetBounds(x, 12, 40, 27);
        nupp.Font = new System.Drawing.Font(
            "Segoe UI", 8, FontStyle.Bold);
        nupp.ForeColor = Color.White;
        nupp.FlatStyle = FlatStyle.Flat;
        nupp.FlatAppearance.BorderSize = 1;
        nupp.FlatAppearance.BorderColor = Color.White;
        nupp.Cursor = Cursors.Hand;
    }

    private void MuudaKeel(string keel)
    {
        Toode_EFcore.Settings.Default.UserLanguage = keel;
        Toode_EFcore.Settings.Default.Save();

        Thread.CurrentThread.CurrentUICulture =
            CultureInfo.GetCultureInfo(keel);
        Thread.CurrentThread.CurrentCulture =
            CultureInfo.GetCultureInfo(keel);

        RakendaKeel();
    }

    private void RakendaKeel()
    {
        bool en = English;

        Text = en ? "Main menu" : "Peamenüü";

        poeNimi.Text = en ? "Shop" : "Pood";
        kirjeldus.Text = en
            ? "Sales management system"
            : "Müügihaldussüsteem";

        pealkiri.Text = en ? "Main menu" : "Peamenüü";

        alapealkiri.Text = en
            ? "Choose an action to continue."
            : "Vali sobiv toiming, et jätkata tööd.";

        UuendaNupp(btnTooted, "▣",
            en ? "Product management" : "Tootehaldus");

        UuendaNupp(btnPood, "⌂",
            en ? "Shop" : "Pood");

        UuendaNupp(btnArve, "▤",
            en ? "Cart and invoice" : "Ostukorv ja arve");

        UuendaNupp(btnStatistika, "▥",
            en ? "Sales statistics" : "Müügistatistika");

        UuendaNupp(btnProfiil, "◉",
            en ? "My profile" : "Minu profiil");

        btnValja.Text = en
            ? "⇥   Log out"
            : "⇥   Logi välja";

        btnEST.BackColor = !en
            ? Color.FromArgb(0, 105, 168)
            : Color.FromArgb(12, 139, 218);

        btnENG.BackColor = en
            ? Color.FromArgb(0, 105, 168)
            : Color.FromArgb(12, 139, 218);
    }

    private static void UuendaNupp(
        Button nupp, string ikoon, string tekst)
    {
        nupp.Text = $"{ikoon}     {tekst}    ›";
    }

    private static void SeaNupp(
        Button nupp,
        string ikoon,
        int y,
        EventHandler tegevus)
    {
        nupp.Bounds = new Rectangle(45, y, 390, 51);

        nupp.Font = new System.Drawing.Font(
            "Segoe UI", 11, FontStyle.Bold);

        nupp.TextAlign = ContentAlignment.MiddleLeft;
        nupp.Padding = new Padding(15, 0, 0, 0);

        nupp.BackColor = Color.FromArgb(245, 250, 255);
        nupp.ForeColor = Color.FromArgb(21, 49, 81);

        nupp.FlatStyle = FlatStyle.Flat;
        nupp.FlatAppearance.BorderSize = 1;
        nupp.FlatAppearance.BorderColor =
            Color.FromArgb(208, 224, 237);

        nupp.Cursor = Cursors.Hand;
        nupp.Click += tegevus;

        nupp.MouseEnter += (s, e) =>
        {
            if (nupp.Enabled)
                nupp.BackColor = Color.FromArgb(221, 239, 253);
        };

        nupp.MouseLeave += (s, e) =>
        {
            nupp.BackColor = Color.FromArgb(245, 250, 255);
        };
    }

    private static void Ava(Form vorm)
    {
        using (vorm)
            vorm.ShowDialog();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        using var pintsel = new LinearGradientBrush(
            ClientRectangle,
            Color.FromArgb(217, 234, 255),
            Color.FromArgb(217, 250, 237),
            LinearGradientMode.ForwardDiagonal);

        e.Graphics.FillRectangle(pintsel, ClientRectangle);
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
    }

    private class RoundedPanel : Panel
    {
        private const int Radius = 25;

        public RoundedPanel()
        {
            DoubleBuffered = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Width <= 0 || Height <= 0)
                return;

            using var path = new GraphicsPath();
            int r = Radius * 2;

            path.AddArc(0, 0, r, r, 180, 90);
            path.AddArc(Width - r, 0, r, r, 270, 90);
            path.AddArc(Width - r, Height - r, r, r, 0, 90);
            path.AddArc(0, Height - r, r, r, 90, 90);
            path.CloseFigure();

            Region = new Region(path);
        }
    }

    private class GradientPanel : Panel
    {
        public GradientPanel()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using var pintsel = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(12, 139, 218),
                Color.FromArgb(38, 193, 117),
                LinearGradientMode.Horizontal);

            e.Graphics.FillRectangle(pintsel, ClientRectangle);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using var laine = new GraphicsPath();

            laine.AddBezier(
                new Point(0, 145),
                new Point(150, 95),
                new Point(305, 210),
                new Point(480, 130));

            laine.AddLine(480, 130, 480, 175);
            laine.AddLine(480, 175, 0, 175);
            laine.CloseFigure();

            using var lainePintsel =
                new SolidBrush(Color.FromArgb(65, 200, 255));

            e.Graphics.FillPath(lainePintsel, laine);
        }
    }
}


using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading;

namespace Tooded_EFCore;

public partial class LoginForm : Form
{
    private readonly TextBox txtEmail = new();
    private readonly TextBox txtParool = new();
    private readonly Button btnLogin = new();
    private readonly Label lblVeateade = new();

    private readonly Button btnEST = new();
    private readonly Button btnENG = new();

    private readonly Label poeNimi = new();
    private readonly Label kirjeldus = new();
    private readonly Label pealkiri = new();
    private readonly Label alapealkiri = new();
    private readonly Label lblEmail = new();
    private readonly Label lblParool = new();
    private readonly Button btnNaita = new();

    private bool English =>
        Toode_EFcore.Settings.Default.UserLanguage == "en-US";

    public LoginForm()
    {
        Text = "Sisselogimine";
        ClientSize = new Size(610, 700);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.FromArgb(234, 244, 255);
        Font = new System.Drawing.Font("Segoe UI", 10);
        DoubleBuffered = true;

        var kaart = new RoundedPanel
        {
            Bounds = new Rectangle(65, 35, 480, 620),
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

        // EST / ENG
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
        pealkiri.Bounds = new Rectangle(35, 195, 410, 55);

        alapealkiri.Font = new System.Drawing.Font(
            "Segoe UI", 10);
        alapealkiri.ForeColor =
            Color.FromArgb(105, 116, 130);
        alapealkiri.TextAlign = ContentAlignment.MiddleCenter;
        alapealkiri.Bounds = new Rectangle(20, 255, 440, 30);

        lblEmail.Bounds = new Rectangle(55, 305, 370, 25);
        lblEmail.Font = new System.Drawing.Font(
            "Segoe UI", 11, FontStyle.Bold);
        lblEmail.ForeColor = Color.FromArgb(21, 49, 81);

        txtEmail.SetBounds(55, 340, 370, 38);
        txtEmail.Font = new System.Drawing.Font("Segoe UI", 11);
        txtEmail.BorderStyle = BorderStyle.FixedSingle;

        lblParool.Bounds = new Rectangle(55, 395, 370, 25);
        lblParool.Font = new System.Drawing.Font(
            "Segoe UI", 11, FontStyle.Bold);
        lblParool.ForeColor = Color.FromArgb(21, 49, 81);

        txtParool.SetBounds(55, 430, 315, 38);
        txtParool.Font = new System.Drawing.Font("Segoe UI", 11);
        txtParool.BorderStyle = BorderStyle.FixedSingle;
        txtParool.UseSystemPasswordChar = true;

        btnNaita.Bounds = new Rectangle(375, 430, 50, 38);
        btnNaita.BackColor = Color.FromArgb(235, 244, 252);
        btnNaita.ForeColor = Color.FromArgb(21, 49, 81);
        btnNaita.FlatStyle = FlatStyle.Flat;
        btnNaita.FlatAppearance.BorderSize = 0;
        btnNaita.Cursor = Cursors.Hand;
        btnNaita.Font = new System.Drawing.Font("Segoe UI", 9);

        btnNaita.Click += (s, e) =>
        {
            txtParool.UseSystemPasswordChar =
                !txtParool.UseSystemPasswordChar;

            RakendaKeel();
        };

        btnLogin.Bounds = new Rectangle(55, 505, 370, 52);
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.BackColor = Color.FromArgb(0, 157, 142);
        btnLogin.ForeColor = Color.White;
        btnLogin.Font = new System.Drawing.Font(
            "Segoe UI", 13, FontStyle.Bold);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.Click += btnLogin_Click;

        btnLogin.MouseEnter += (s, e) =>
            btnLogin.BackColor = Color.FromArgb(0, 128, 122);

        btnLogin.MouseLeave += (s, e) =>
            btnLogin.BackColor = Color.FromArgb(0, 157, 142);

        lblVeateade.Bounds = new Rectangle(55, 565, 370, 35);
        lblVeateade.ForeColor = Color.FromArgb(210, 60, 60);
        lblVeateade.TextAlign = ContentAlignment.MiddleCenter;

        kaart.Controls.AddRange(new Control[]
        {
            pais, pealkiri, alapealkiri,
            lblEmail, txtEmail,
            lblParool, txtParool, btnNaita,
            btnLogin, lblVeateade
        });

        Controls.Add(kaart);

        AcceptButton = btnLogin;

        RakendaKeel();

        // Kui kasutaja logib välja, näitame viimast keelt
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

        Text = en ? "Login" : "Sisselogimine";

        poeNimi.Text = en ? "Shop" : "Pood";
        kirjeldus.Text = en
            ? "Sales management system"
            : "Müügihaldussüsteem";

        pealkiri.Text = en ? "Login" : "Sisselogimine";

        alapealkiri.Text = en
            ? "Sign in to use the shop management system."
            : "Logi sisse, et kasutada poe haldussüsteemi.";

        lblEmail.Text = en ? "✉  Email" : "✉  E-post";
        lblParool.Text = en ? "🔒  Password" : "🔒  Parool";

        txtEmail.PlaceholderText = en
            ? "Enter your email address"
            : "Sisesta e-posti aadress";

        txtParool.PlaceholderText = en
            ? "Enter password"
            : "Sisesta parool";

        btnNaita.Text = txtParool.UseSystemPasswordChar
            ? (en ? "Show" : "Näita")
            : (en ? "Hide" : "Peida");

        btnLogin.Text = en
            ? "→   Sign in"
            : "→   Logi sisse";

        btnEST.BackColor = !en
            ? Color.FromArgb(0, 105, 168)
            : Color.FromArgb(12, 139, 218);

        btnENG.BackColor = en
            ? Color.FromArgb(0, 105, 168)
            : Color.FromArgb(12, 139, 218);
    }

    private void btnLogin_Click(object? sender, EventArgs e)
    {
        string email = txtEmail.Text.Trim().ToLowerInvariant();

        using var db = new ToodedContext();

        var kasutaja = db.Kasutajad
            .FirstOrDefault(k => k.Email == email);

        if (kasutaja == null ||
            !PasswordHelper.Verify(
                txtParool.Text, kasutaja.ParoolHash))
        {
            lblVeateade.Text = English
                ? "Incorrect email or password."
                : "Vale e-post või parool.";

            txtParool.Clear();
            return;
        }

        AktiivneKasutaja.Id = kasutaja.Id;
        AktiivneKasutaja.Nimi = kasutaja.Nimi;
        AktiivneKasutaja.Email = kasutaja.Email;
        AktiivneKasutaja.Roll = kasutaja.Roll;

        txtParool.Clear();
        lblVeateade.Text = "";

        var menyy = new FormMain();

        menyy.FormClosed += (s, a) =>
        {
            AktiivneKasutaja.LogiValja();
            txtEmail.Clear();
            txtParool.Clear();
            lblVeateade.Text = "";
            Show();
            RakendaKeel();
        };

        menyy.Show();
        Hide();
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

    private void LoginForm_Load(object sender, EventArgs e)
    {
    }
}

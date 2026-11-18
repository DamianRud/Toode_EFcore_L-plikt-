
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading;

namespace Tooded_EFCore;

public partial class Form3 : Form
{
    private readonly ToodedContext _db = new();

    private readonly TabControl tabPood = new();
    private readonly ComboBox comboLanguage = new();
    private readonly Button btnOstukorv = new();

    private readonly TextBox txtOtsing = new();
    private readonly NumericUpDown numMinHind = new();
    private readonly NumericUpDown numMaxHind = new();
    private readonly Label lblMinHind = new();
    private readonly Label lblMaxHind = new();
    private readonly Label lblSort = new();
    private readonly ComboBox comboSort = new();
    private readonly Button btnReset = new();

    private readonly Label lblOtsing = new();
    private readonly Label lblTulemused = new();

    private readonly ComponentResourceManager _resources =
        new(typeof(Form3));

    private string _language = "et-EE";
    private bool _keelLaetud = false;
    private bool _sortUpdating = false;

    public Form3()
    {
        InitializeComponent();

        string salvestatudKeel =
            Toode_EFcore.Settings.Default.UserLanguage;

        _language = salvestatudKeel == "en-US"
            ? "en-US"
            : "et-EE";

        Thread.CurrentThread.CurrentUICulture =
            CultureInfo.GetCultureInfo(_language);

        Thread.CurrentThread.CurrentCulture =
            CultureInfo.GetCultureInfo(_language);

        Text = "Pood";
        ClientSize = new Size(1040, 745);
        MinimumSize = new Size(1060, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(235, 245, 253);
        Font = new System.Drawing.Font("Segoe UI", 10);
        DoubleBuffered = true;

        Controls.Clear();

        // ---------- VÄRVILINE PÄIS ----------
        var pais = new Panel
        {
            Dock = DockStyle.Top,
            Height = 105
        };

        pais.Paint += (s, e) =>
        {
            using var pintsel = new LinearGradientBrush(
                pais.ClientRectangle,
                Color.FromArgb(12, 139, 218),
                Color.FromArgb(38, 193, 117),
                LinearGradientMode.Horizontal);

            e.Graphics.FillRectangle(
                pintsel, pais.ClientRectangle);
        };

        var logo = new Label
        {
            Text = "🛒",
            Bounds = new Rectangle(22, 22, 65, 60),
            Font = new System.Drawing.Font(
                "Segoe UI Emoji", 28),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent
        };

        var pealkiri = new Label
        {
            Text = "TOODETE POOD",
            Bounds = new Rectangle(100, 19, 470, 43),
            Font = new System.Drawing.Font(
                "Segoe UI", 23, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };

        var alapealkiri = new Label
        {
            Text = "Leia oma lemmiktooted",
            Bounds = new Rectangle(103, 66, 450, 26),
            Font = new System.Drawing.Font(
                "Segoe UI", 11),
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };

        var lblKeel = new Label
        {
            Text = "Keel / Language",
            AutoSize = true,
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Font = new System.Drawing.Font(
                "Segoe UI", 10, FontStyle.Bold)
        };

        var keelePaneel = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 255,
            Padding = new Padding(0, 35, 20, 0),
            BackColor = Color.Transparent,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        lblKeel.Margin = new Padding(0, 6, 10, 0);

        comboLanguage.Name = "comboLanguage";
        comboLanguage.DropDownStyle =
            ComboBoxStyle.DropDownList;

        comboLanguage.Width = 105;
        comboLanguage.Height = 30;
        comboLanguage.Font = new System.Drawing.Font(
            "Segoe UI", 10);

        comboLanguage.Items.AddRange(new object[]
        {
            "Eesti", "English"
        });

        comboLanguage.SelectedItem =
            _language == "en-US" ? "English" : "Eesti";

        comboLanguage.SelectedIndexChanged +=
            comboLanguage_SelectedIndexChanged;

        _keelLaetud = true;

        keelePaneel.Controls.Add(lblKeel);
        keelePaneel.Controls.Add(comboLanguage);

        pais.Controls.AddRange(new Control[]
        {
            logo,
            pealkiri,
            alapealkiri,
            keelePaneel
        });

        // ---------- FILTRITE PANEEL ----------
        var filtrid = new Panel
        {
            Dock = DockStyle.Top,
            Height = 158,
            BackColor = Color.White,
            Padding = new Padding(18, 10, 18, 8)
        };

        lblOtsing.Text = "Otsi toodet";
        lblOtsing.SetBounds(22, 12, 260, 25);
        lblOtsing.Font = new System.Drawing.Font(
            "Segoe UI", 10, FontStyle.Bold);
        lblOtsing.ForeColor = Color.FromArgb(21, 49, 81);

        txtOtsing.Name = "txtOtsing";
        txtOtsing.SetBounds(22, 42, 355, 32);
        txtOtsing.Font = new System.Drawing.Font(
            "Segoe UI", 11);
        txtOtsing.BorderStyle = BorderStyle.FixedSingle;
        txtOtsing.BackColor =
            Color.FromArgb(248, 251, 255);
        txtOtsing.TextChanged += (s, e) => LaePood();

        lblMinHind.SetBounds(22, 91, 90, 25);
        lblMinHind.ForeColor =
            Color.FromArgb(21, 49, 81);

        numMinHind.SetBounds(112, 88, 105, 31);
        numMinHind.Minimum = 0;
        numMinHind.Maximum = 1000000;
        numMinHind.DecimalPlaces = 2;
        numMinHind.Increment = 0.50M;
        numMinHind.ValueChanged += (s, e) => LaePood();

        lblMaxHind.SetBounds(230, 91, 90, 25);
        lblMaxHind.ForeColor =
            Color.FromArgb(21, 49, 81);

        numMaxHind.SetBounds(322, 88, 105, 31);
        numMaxHind.Minimum = 0;
        numMaxHind.Maximum = 1000000;
        numMaxHind.DecimalPlaces = 2;
        numMaxHind.Increment = 0.50M;
        numMaxHind.Value = 1000000;
        numMaxHind.ValueChanged += (s, e) => LaePood();

        lblSort.SetBounds(452, 91, 85, 25);
        lblSort.ForeColor =
            Color.FromArgb(21, 49, 81);

        comboSort.SetBounds(535, 88, 245, 31);
        comboSort.DropDownStyle =
            ComboBoxStyle.DropDownList;

        comboSort.Font = new System.Drawing.Font(
            "Segoe UI", 10);

        comboSort.SelectedIndexChanged += (s, e) =>
        {
            if (!_sortUpdating)
                LaePood();
        };

        btnReset.SetBounds(799, 88, 205, 34);
        btnReset.BackColor =
            Color.FromArgb(232, 241, 249);
        btnReset.ForeColor =
            Color.FromArgb(21, 49, 81);
        btnReset.FlatStyle = FlatStyle.Flat;
        btnReset.FlatAppearance.BorderSize = 0;
        btnReset.Font = new System.Drawing.Font(
            "Segoe UI", 10, FontStyle.Bold);
        btnReset.Cursor = Cursors.Hand;

        btnReset.Click += (s, e) =>
        {
            txtOtsing.Clear();
            numMinHind.Value = 0;
            numMaxHind.Value = numMaxHind.Maximum;
            comboSort.SelectedIndex = 0;
            LaePood();
        };

        lblTulemused.SetBounds(22, 126, 950, 25);
        lblTulemused.ForeColor =
            Color.FromArgb(100, 115, 130);
        lblTulemused.Font = new System.Drawing.Font(
            "Segoe UI", 9);

        filtrid.Controls.AddRange(new Control[]
        {
            lblOtsing,
            txtOtsing,
            lblMinHind,
            numMinHind,
            lblMaxHind,
            numMaxHind,
            lblSort,
            comboSort,
            btnReset,
            lblTulemused
        });

        // ---------- KATEGOORIAD ----------
        tabPood.Dock = DockStyle.Fill;
        tabPood.Font = new System.Drawing.Font(
            "Segoe UI", 10, FontStyle.Bold);
        tabPood.Padding = new Point(16, 9);

        // ---------- OSTUKORVI NUPP ----------
        btnOstukorv.Dock = DockStyle.Bottom;
        btnOstukorv.Height = 55;
        btnOstukorv.BackColor =
            Color.FromArgb(34, 166, 110);
        btnOstukorv.ForeColor = Color.White;
        btnOstukorv.FlatStyle = FlatStyle.Flat;
        btnOstukorv.FlatAppearance.BorderSize = 0;
        btnOstukorv.Font = new System.Drawing.Font(
            "Segoe UI", 13, FontStyle.Bold);
        btnOstukorv.Cursor = Cursors.Hand;

        btnOstukorv.MouseEnter += (s, e) =>
            btnOstukorv.BackColor =
                Color.FromArgb(24, 142, 96);

        btnOstukorv.MouseLeave += (s, e) =>
            btnOstukorv.BackColor =
                Color.FromArgb(34, 166, 110);

        btnOstukorv.Click += (s, e) =>
        {
            using var arve = new ArveForm();
            arve.ShowDialog(this);

            // Värskendame laoseisu pärast ostu
            LaePood();
        };

        Controls.Add(tabPood);
        Controls.Add(btnOstukorv);
        Controls.Add(filtrid);
        Controls.Add(pais);

        RakendaKeel();
    }

    // ---------- RESSURSID ----------
    private string T(string nimi, string vaikeTekst)
    {
        return _resources.GetString(
            nimi,
            CultureInfo.GetCultureInfo(_language))
            ?? vaikeTekst;
    }

    // ---------- KEELE VAHETAMINE ----------
    private void comboLanguage_SelectedIndexChanged(
        object? sender, EventArgs e)
    {
        if (!_keelLaetud)
            return;

        string lang =
            comboLanguage.SelectedItem?.ToString()
                == "English"
                ? "en-US"
                : "et-EE";

        ChangeLanguage(lang);
    }

    private void ChangeLanguage(string lang)
    {
        _language = lang;

        Toode_EFcore.Settings.Default.UserLanguage = lang;
        Toode_EFcore.Settings.Default.Save();

        Thread.CurrentThread.CurrentUICulture =
            CultureInfo.GetCultureInfo(lang);

        Thread.CurrentThread.CurrentCulture =
            CultureInfo.GetCultureInfo(lang);

        RakendaKeel();
    }

    private void RakendaKeel()
    {
        Text = T("ShopTitle", "Pood");

        btnOstukorv.Text =
            "🛒  " +
            T("GoToCart", "Mine ostukorvi") +
            "  →";

        lblOtsing.Text = _language == "en-US"
            ? "Search products"
            : "Otsi toodet";

        txtOtsing.PlaceholderText = _language == "en-US"
            ? "Search products..."
            : "Otsi toodet...";

        lblMinHind.Text = _language == "en-US"
            ? "Min price:"
            : "Min hind:";

        lblMaxHind.Text = _language == "en-US"
            ? "Max price:"
            : "Max hind:";

        lblSort.Text = _language == "en-US"
            ? "Sort by:"
            : "Sorteeri:";

        btnReset.Text = _language == "en-US"
            ? "↻  Reset filters"
            : "↻  Tühjenda filtrid";

        int eelmineValik = Math.Max(
            0, comboSort.SelectedIndex);

        _sortUpdating = true;
        comboSort.Items.Clear();

        if (_language == "en-US")
        {
            comboSort.Items.AddRange(new object[]
            {
                "Name: A–Z",
                "Name: Z–A",
                "Price: low to high",
                "Price: high to low"
            });
        }
        else
        {
            comboSort.Items.AddRange(new object[]
            {
                "Nimi: A–Z",
                "Nimi: Z–A",
                "Hind: odavam enne",
                "Hind: kallim enne"
            });
        }

        comboSort.SelectedIndex = Math.Min(
            eelmineValik,
            comboSort.Items.Count - 1);

        _sortUpdating = false;

        LaePood();
    }

    // ---------- POE LAADIMINE ----------
    private void LaePood()
    {
        int valitudVaheleht = tabPood.SelectedIndex;

        while (tabPood.TabPages.Count > 0)
        {
            var vanaLeht = tabPood.TabPages[0];
            tabPood.TabPages.RemoveAt(0);
            vanaLeht.Dispose();
        }

        string otsing = txtOtsing.Text.Trim();
        decimal minHind = numMinHind.Value;
        decimal maxHind = numMaxHind.Value;

        var kategooriad = _db.Kategooriatabel
            .AsNoTracking()
            .Include(k => k.Tooded)
            .OrderBy(k => k.Kategooria_nimetus)
            .ToList();

        int kokkuLeitud = 0;

        foreach (var kat in kategooriad)
        {
            var leht = new TabPage(
                T("Category_" + kat.Id,
                    kat.Kategooria_nimetus))
            {
                AutoScroll = true,
                BackColor =
                    Color.FromArgb(238, 246, 252),
                Padding = new Padding(8)
            };

            var filtreeritud = kat.Tooded
                .Where(t =>
                    t.Toodenimetus.Contains(
                        otsing,
                        StringComparison.OrdinalIgnoreCase)
                    && t.Hind >= minHind
                    && t.Hind <= maxHind);

            var tooted = (comboSort.SelectedIndex switch
            {
                1 => filtreeritud.OrderByDescending(
                    t => t.Toodenimetus),

                2 => filtreeritud.OrderBy(
                    t => t.Hind)
                    .ThenBy(t => t.Toodenimetus),

                3 => filtreeritud.OrderByDescending(
                    t => t.Hind)
                    .ThenBy(t => t.Toodenimetus),

                _ => filtreeritud.OrderBy(
                    t => t.Toodenimetus)

            }).ToList();

            kokkuLeitud += tooted.Count;

            int i = 0;

            foreach (var toode in tooted)
            {
                var kaart = LooTootekaart(toode);

                // 5 toodet ühes reas
                kaart.Location = new Point(
                    16 + (i % 5) * 198,
                    16 + (i / 5) * 280);

                leht.Controls.Add(kaart);
                i++;
            }

            if (tooted.Count == 0)
            {
                var puudub = new Label
                {
                    Text = _language == "en-US"
                        ? "No products found."
                        : "Tooteid ei leitud.",

                    AutoSize = true,
                    Location = new Point(25, 30),
                    Font = new System.Drawing.Font(
                        "Segoe UI", 12),
                    ForeColor =
                        Color.FromArgb(110, 125, 140)
                };

                leht.Controls.Add(puudub);
            }

            tabPood.TabPages.Add(leht);
        }

        lblTulemused.Text = _language == "en-US"
            ? $"Products found: {kokkuLeitud}"
            : $"Leitud tooteid: {kokkuLeitud}";

        if (tabPood.TabPages.Count > 0 &&
            valitudVaheleht >= 0)
        {
            tabPood.SelectedIndex = Math.Min(
                valitudVaheleht,
                tabPood.TabPages.Count - 1);
        }
    }

    // ---------- TOOTEKAART ----------
    private Panel LooTootekaart(Toode t)
    {
        var kaart = new Panel
        {
            Size = new Size(183, 260),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var pilt = new PictureBox
        {
            Bounds = new Rectangle(12, 10, 157, 115),
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = Kaustad.LaePilt(t.Pilt),
            BackColor = Color.FromArgb(248, 251, 255),
            Cursor = Cursors.Hand
        };

        var siltNimi = new Label
        {
            Bounds = new Rectangle(8, 132, 165, 29),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = t.Toodenimetus,
            Font = new System.Drawing.Font(
                "Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(21, 49, 81),
            AutoEllipsis = true
        };

        var siltHind = new Label
        {
            Bounds = new Rectangle(8, 164, 165, 30),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = $"{t.Hind:F2} €",
            Font = new System.Drawing.Font(
                "Segoe UI", 13, FontStyle.Bold),
            ForeColor = Color.FromArgb(27, 160, 108)
        };

        var laoseis = new Label
        {
            Bounds = new Rectangle(8, 195, 165, 18),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = t.Kogus > 0
                ? (_language == "en-US"
                    ? $"In stock: {t.Kogus}"
                    : $"Laos: {t.Kogus}")
                : (_language == "en-US"
                    ? "Out of stock"
                    : "Laost otsas"),

            Font = new System.Drawing.Font(
                "Segoe UI", 8),
            ForeColor = t.Kogus > 0
                ? Color.FromArgb(95, 115, 130)
                : Color.FromArgb(220, 75, 75)
        };

        var kogus = new NumericUpDown
        {
            Bounds = new Rectangle(8, 222, 53, 27),
            Minimum = 1,
            Maximum = Math.Max(1, t.Kogus),
            Value = 1,
            Font = new System.Drawing.Font(
                "Segoe UI", 9)
        };

        var nupp = new Button
        {
            Bounds = new Rectangle(67, 220, 106, 31),
            Text = T("AddToCart", "Lisa korvi"),
            Enabled = t.Kogus > 0,
            BackColor = Color.FromArgb(40, 158, 207),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font(
                "Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        nupp.FlatAppearance.BorderSize = 0;

        nupp.Click += (s, e) =>
        {
            if (!Ostukorv.Lisa(t, (int)kogus.Value))
            {
                MessageBox.Show(
                    _language == "en-US"
                        ? "Not enough stock. Check the quantity in your cart."
                        : "Laos pole piisavalt toodet. Kontrolli ostukorvi kogust.");

                LaePood();
                return;
            }

            string teade = _language == "en-US"
                ? $"{t.Toodenimetus} ({kogus.Value} pcs) added to cart."
                : $"{t.Toodenimetus} ({kogus.Value} tk) lisati ostukorvi.";

            MessageBox.Show(teade);
        };

        pilt.Click += (s, e) =>
        {
            string info =
                $"{t.Toodenimetus}\n" +
                $"{T("Price", "Hind")}: {t.Hind:F2} €\n" +
                $"{T("InStock", "Laos")}: {t.Kogus}";

            MessageBox.Show(
                info,
                T("ProductInfo", "Toote info"));
        };

        kaart.Controls.AddRange(new Control[]
        {
            pilt,
            siltNimi,
            siltHind,
            laoseis,
            kogus,
            nupp
        });

        return kaart;
    }

    protected override void OnFormClosed(
        FormClosedEventArgs e)
    {
        _db.Dispose();
        base.OnFormClosed(e);
    }

    private void Form3_Load(
        object sender, EventArgs e)
    {
    }
}

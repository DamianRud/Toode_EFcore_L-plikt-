
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace Tooded_EFCore;

public partial class Form3 : Form
{
    private readonly ToodedContext _db = new();
    private readonly TabControl tabPood = new();
    private readonly ComboBox comboLanguage = new();
    private readonly Button btnOstukorv = new();

    // Otsing ja hinnafilter
    private readonly TextBox txtOtsing = new();
    private readonly NumericUpDown numMinHind = new();
    private readonly NumericUpDown numMaxHind = new();
    private readonly Label lblMinHind = new();
    private readonly Label lblMaxHind = new();
    private readonly Label lblSort = new();
    private readonly ComboBox comboSort = new();
    private readonly Button btnReset = new();
    private bool _sortUpdating = false;

    private readonly ComponentResourceManager _resources =
        new(typeof(Form3));

    private string _language = "et-EE";
    private bool _keelLaetud = false;

    public Form3()
    {
        InitializeComponent();

        // Viimati valitud keel
        string salvestatudKeel =
            Toode_EFcore.Settings.Default.UserLanguage;

        _language = salvestatudKeel == "en-US"
            ? "en-US"
            : "et-EE";

        Thread.CurrentThread.CurrentUICulture =
            CultureInfo.GetCultureInfo(_language);

        Thread.CurrentThread.CurrentCulture =
            CultureInfo.GetCultureInfo(_language);

        ClientSize = new Size(930, 630);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new System.Drawing.Font("Segoe UI", 10);

        // ---------- Ülemine paneel ----------
        var paneel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 102,
            BackColor = Color.White
        };

        var lblKeel = new Label
        {
            Text = "Keel / Language:",
            Location = new Point(615, 14),
            AutoSize = true,
            Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold)
        };

        comboLanguage.Name = "comboLanguage";
        comboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
        comboLanguage.SetBounds(755, 9, 150, 30);

        comboLanguage.Items.Add("Eesti");
        comboLanguage.Items.Add("English");
        comboLanguage.SelectedItem = _language == "en-US" ? "English" : "Eesti";
        comboLanguage.SelectedIndexChanged += comboLanguage_SelectedIndexChanged;
        _keelLaetud = true;

        // ---------- Otsing ----------
        txtOtsing.Name = "txtOtsing";
        txtOtsing.SetBounds(15, 10, 350, 30);
        txtOtsing.TextChanged += (s, e) => LaePood();

        // ---------- Hind ----------
        lblMinHind.SetBounds(15, 60, 85, 25);
        numMinHind.SetBounds(100, 55, 105, 30);
        numMinHind.Minimum = 0;
        numMinHind.Maximum = 1000000;
        numMinHind.DecimalPlaces = 2;
        numMinHind.Increment = 0.50M;
        numMinHind.ValueChanged += (s, e) => LaePood();

        lblMaxHind.SetBounds(215, 60, 85, 25);
        numMaxHind.SetBounds(300, 55, 105, 30);
        numMaxHind.Minimum = 0;
        numMaxHind.Maximum = 1000000;
        numMaxHind.DecimalPlaces = 2;
        numMaxHind.Increment = 0.50M;
        numMaxHind.Value = 1000000;
        numMaxHind.ValueChanged += (s, e) => LaePood();

        // ---------- Sorteerimine ----------
        lblSort.SetBounds(420, 60, 85, 25);
        comboSort.SetBounds(505, 55, 245, 30);
        comboSort.DropDownStyle = ComboBoxStyle.DropDownList;
        comboSort.SelectedIndexChanged += (s, e) =>
        {
            if (!_sortUpdating) LaePood();
        };

        // ---------- Filtrite lähtestamine ----------
        btnReset.SetBounds(770, 55, 135, 30);
        btnReset.FlatStyle = FlatStyle.Flat;
        btnReset.BackColor = Color.FromArgb(230, 235, 241);
        btnReset.FlatAppearance.BorderSize = 0;
        btnReset.Cursor = Cursors.Hand;
        btnReset.Click += (s, e) =>
        {
            txtOtsing.Clear();
            numMinHind.Value = 0;
            numMaxHind.Value = numMaxHind.Maximum;
            comboSort.SelectedIndex = 0;
            LaePood();
        };

        paneel.Controls.AddRange(new Control[]
        {
            lblKeel, comboLanguage, txtOtsing,
            lblMinHind, numMinHind, lblMaxHind, numMaxHind,
            lblSort, comboSort, btnReset
        });

        // ---------- Poe vahelehed ----------
        tabPood.Dock = DockStyle.Fill;
        tabPood.Font = new System.Drawing.Font(
            "Segoe UI", 10, FontStyle.Bold);

        // ---------- Ostukorvi nupp ----------
        btnOstukorv.Dock = DockStyle.Bottom;
        btnOstukorv.Height = 45;
        btnOstukorv.BackColor =
            Color.FromArgb(46, 160, 100);
        btnOstukorv.ForeColor = Color.White;
        btnOstukorv.FlatStyle = FlatStyle.Flat;
        btnOstukorv.FlatAppearance.BorderSize = 0;
        btnOstukorv.Font = new System.Drawing.Font(
            "Segoe UI", 11, FontStyle.Bold);
        btnOstukorv.Cursor = Cursors.Hand;

        btnOstukorv.Click += (s, e) =>
        {
            using var arve = new ArveForm();
            arve.ShowDialog(this);
        };

        Controls.Add(tabPood);
        Controls.Add(btnOstukorv);
        Controls.Add(paneel);

        RakendaKeel();
    }

    // ---------- Ressursside lugemine ----------
    private string T(string nimi, string vaikeTekst)
    {
        return _resources.GetString(
            nimi,
            CultureInfo.GetCultureInfo(_language))
            ?? vaikeTekst;
    }

    // ---------- Keele vahetamine ----------
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

        // Salvesta keel
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
            T("GoToCart", "Mine ostukorvi") + " →";

        // Otsingu ja filtrite tõlked
        txtOtsing.PlaceholderText = _language == "en-US"
            ? "Search products..."
            : "Otsi toodet...";

        lblMinHind.Text = _language == "en-US"
            ? "Min price:"
            : "Min hind:";

        lblMaxHind.Text = _language == "en-US"
            ? "Max price:"
            : "Max hind:";

        lblSort.Text = _language == "en-US" ? "Sort by:" : "Sorteeri:";
        btnReset.Text = _language == "en-US" ? "Reset filters" : "Tühjenda";

        int eelmineValik = Math.Max(0, comboSort.SelectedIndex);
        _sortUpdating = true;
        comboSort.Items.Clear();
        if (_language == "en-US")
            comboSort.Items.AddRange(new object[]
            {
                "Name: A–Z", "Name: Z–A", "Price: low to high", "Price: high to low"
            });
        else
            comboSort.Items.AddRange(new object[]
            {
                "Nimi: A–Z", "Nimi: Z–A", "Hind: odavam enne", "Hind: kallim enne"
            });
        comboSort.SelectedIndex = Math.Min(eelmineValik, comboSort.Items.Count - 1);
        _sortUpdating = false;

        LaePood();
    }

    // ---------- Poe laadimine ja filtreerimine ----------
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
            .Include(k => k.Tooded)
            .OrderBy(k => k.Kategooria_nimetus)
            .ToList();

        foreach (var kat in kategooriad)
        {
            var leht = new TabPage(
                T("Category_" + kat.Id, kat.Kategooria_nimetus))
            {
                AutoScroll = true,
                BackColor =
                    Color.FromArgb(245, 247, 250)
            };

            // Otsime nime ja hinna järgi
            var filtreeritud = kat.Tooded
                .Where(t =>
                    t.Toodenimetus.Contains(
                        otsing,
                        StringComparison.OrdinalIgnoreCase)
                    && t.Hind >= minHind
                    && t.Hind <= maxHind);

            var tooted = (comboSort.SelectedIndex switch
            {
                1 => filtreeritud.OrderByDescending(t => t.Toodenimetus),
                2 => filtreeritud.OrderBy(t => t.Hind).ThenBy(t => t.Toodenimetus),
                3 => filtreeritud.OrderByDescending(t => t.Hind).ThenBy(t => t.Toodenimetus),
                _ => filtreeritud.OrderBy(t => t.Toodenimetus)
            }).ToList();

            int i = 0;

            foreach (var toode in tooted)
            {
                var kaart = LooTootekaart(toode);

                kaart.Location = new Point(
                    10 + (i % 5) * 175,
                    10 + (i / 5) * 250
                );

                leht.Controls.Add(kaart);
                i++;
            }

            tabPood.TabPages.Add(leht);
        }

        if (tabPood.TabPages.Count > 0 && valitudVaheleht >= 0)
            tabPood.SelectedIndex = Math.Min(valitudVaheleht, tabPood.TabPages.Count - 1);
    }

    // ---------- Tootekaart ----------
    private Panel LooTootekaart(Toode t)
    {
        var kaart = new Panel
        {
            Size = new Size(160, 235),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };

        // Toote pilt
        var pilt = new PictureBox
        {
            Bounds = new Rectangle(10, 8, 138, 125),
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = Kaustad.LaePilt(t.Pilt),
            Cursor = Cursors.Hand
        };

        // Toote nimetus
        var siltNimi = new Label
        {
            Bounds = new Rectangle(5, 137, 148, 25),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = t.Toodenimetus,
            Font = new System.Drawing.Font(
                "Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80)
        };

        // Toote hind
        var siltHind = new Label
        {
            Bounds = new Rectangle(5, 164, 148, 25),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = $"{t.Hind:F2} €",
            Font = new System.Drawing.Font(
                "Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(39, 160, 96)
        };

        // Koguse valik
        var kogus = new NumericUpDown
        {
            Bounds = new Rectangle(7, 197, 55, 27),
            Minimum = 1,
            Maximum = Math.Max(1, t.Kogus),
            Value = 1,
            Font = new System.Drawing.Font(
                "Segoe UI", 9)
        };

        // Lisa ostukorvi
        var nupp = new Button
        {
            Bounds = new Rectangle(67, 195, 86, 30),
            Text = T("AddToCart", "Lisa korvi"),
            Enabled = t.Kogus > 0,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font(
                "Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        nupp.FlatAppearance.BorderSize = 0;

        nupp.Click += (s, e) =>
        {
            Ostukorv.Lisa(t, (int)kogus.Value);

            string teade = _language == "en-US"
                ? $"{t.Toodenimetus} ({kogus.Value} pcs) added to cart."
                : $"{t.Toodenimetus} ({kogus.Value} tk) lisati ostukorvi.";

            MessageBox.Show(teade);
        };

        // Toote info
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

using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace Tooded_EFCore;

public class MuugistatistikaForm : Form
{
    private readonly Label lblKaive = new();
    private readonly Label lblOstud = new();
    private readonly Label lblTooteid = new();
    private readonly DataGridView tabel = new();
    private readonly Panel graafik = new();
    private readonly Button btnUuenda = new();
    private readonly Label pealkiri = new();
    private readonly Label lblGraafik = new();
    private readonly Label lblPopulaarsed = new();
    private readonly List<(DateTime Paev, decimal Summa)> paevad = new();

    private decimal kaive;
    private int ostudeArv;
    private int myydudKogus;

    public MuugistatistikaForm()
    {
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(880, 650);
        MinimumSize = new Size(800, 650);
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new System.Drawing.Font("Segoe UI", 10);

        pealkiri.Font = new System.Drawing.Font("Segoe UI", 17, FontStyle.Bold);
        pealkiri.ForeColor = Color.FromArgb(44, 62, 80);
        pealkiri.Bounds = new Rectangle(25, 15, 550, 42);

        lblKaive.SetBounds(25, 75, 260, 45);
        lblOstud.SetBounds(305, 75, 260, 45);
        lblTooteid.SetBounds(585, 75, 265, 45);
        foreach (var silt in new[] { lblKaive, lblOstud, lblTooteid })
        {
            silt.BackColor = Color.White;
            silt.BorderStyle = BorderStyle.FixedSingle;
            silt.TextAlign = ContentAlignment.MiddleCenter;
            silt.Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold);
        }

        lblGraafik.Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold);
        lblGraafik.Bounds = new Rectangle(25, 140, 550, 30);

        graafik.SetBounds(25, 175, 825, 205);
        graafik.BackColor = Color.White;
        graafik.BorderStyle = BorderStyle.FixedSingle;
        graafik.Paint += JoonistaGraafik;

        lblPopulaarsed.Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold);
        lblPopulaarsed.Bounds = new Rectangle(25, 398, 500, 30);

        tabel.SetBounds(25, 435, 825, 155);
        tabel.ReadOnly = true;
        tabel.AllowUserToAddRows = false;
        tabel.AllowUserToDeleteRows = false;
        tabel.RowHeadersVisible = false;
        tabel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tabel.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tabel.BackgroundColor = Color.White;
        tabel.Columns.Add("Toode", "Toode");
        tabel.Columns.Add("Kogus", "Müüdud kogus");
        tabel.Columns.Add("Summa", "Käive (€)");

        btnUuenda.SetBounds(640, 20, 210, 38);
        btnUuenda.BackColor = Color.FromArgb(52, 152, 219);
        btnUuenda.ForeColor = Color.White;
        btnUuenda.FlatStyle = FlatStyle.Flat;
        btnUuenda.FlatAppearance.BorderSize = 0;
        btnUuenda.Click += (s, e) => LaeStatistika();

        Controls.AddRange(new Control[]
        {
            pealkiri, btnUuenda, lblKaive, lblOstud, lblTooteid,
            lblGraafik, graafik, lblPopulaarsed, tabel
        });

        RakendaKeel();
        Shown += (s, e) => LaeStatistika();
        Activated += (s, e) => RakendaKeel();
    }

    private void RakendaKeel()
    {
        Text = Keel.T("Müügistatistika", "Sales statistics");
        pealkiri.Text = Keel.T("MÜÜGISTATISTIKA", "SALES STATISTICS");
        lblGraafik.Text = Keel.T(
            "Müük viimase 7 päeva jooksul", "Sales over the last 7 days");
        lblPopulaarsed.Text = Keel.T(
            "Enim müüdud tooted", "Best-selling products");
        btnUuenda.Text = Keel.T("Uuenda statistikat", "Refresh statistics");
        lblKaive.Text = $"{Keel.T("Käive", "Revenue")}: {kaive:F2} €";
        lblOstud.Text = $"{Keel.T("Oste", "Orders")}: {ostudeArv}";
        lblTooteid.Text = $"{Keel.T("Müüdud tk", "Units sold")}: {myydudKogus}";
        tabel.Columns["Toode"]!.HeaderText = Keel.T("Toode", "Product");
        tabel.Columns["Kogus"]!.HeaderText = Keel.T("Müüdud kogus", "Units sold");
        tabel.Columns["Summa"]!.HeaderText = Keel.T("Käive (€)", "Revenue (€)");
        graafik.Invalidate();
    }

    private void LaeStatistika()
    {
        try
        {
            using var db = new ToodedContext();
            var ostud = db.Ostud.AsNoTracking().Include(o => o.Read).ToList();

            kaive = ostud.Sum(o => o.Kokku);
            ostudeArv = ostud.Count;
            myydudKogus = ostud.SelectMany(o => o.Read).Sum(r => r.Kogus);

            tabel.Rows.Clear();
            var populaarsed = ostud.SelectMany(o => o.Read)
                .GroupBy(r => r.ToodeId)
                .Select(g => new
                {
                    Nimi = g.First().TooteNimi,
                    Kogus = g.Sum(r => r.Kogus),
                    Summa = g.Sum(r => r.Summa)
                })
                .OrderByDescending(t => t.Kogus)
                .Take(10);

            foreach (var toode in populaarsed)
                tabel.Rows.Add(toode.Nimi, toode.Kogus, $"{toode.Summa:F2}");

            paevad.Clear();
            DateTime tana = DateTime.Today;
            for (int i = 6; i >= 0; i--)
            {
                DateTime paev = tana.AddDays(-i);
                decimal summa = ostud
                    .Where(o => o.Kuupaev.Date == paev)
                    .Sum(o => o.Kokku);
                paevad.Add((paev, summa));
            }

            RakendaKeel();
        }
        catch (Exception ex)
        {
            MessageBox.Show(Keel.T(
                "Statistika laadimine ebaõnnestus: ",
                "Failed to load statistics: ") + ex.Message);
        }
    }

    private void JoonistaGraafik(object? sender, PaintEventArgs e)
    {
        if (paevad.Count == 0) return;

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        int algusX = 65;
        int alumineY = graafik.ClientSize.Height - 35;
        int graafikuKorgus = graafik.ClientSize.Height - 65;
        int samm = (graafik.ClientSize.Width - 100) / 7;
        decimal maksimum = Math.Max(1m, paevad.Max(p => p.Summa));

        using var sinine = new SolidBrush(Color.FromArgb(52, 152, 219));
        using var telg = new Pen(Color.LightGray);
        using var tekstFont = new System.Drawing.Font("Segoe UI", 9);
        e.Graphics.DrawLine(telg, algusX - 10, alumineY,
            graafik.ClientSize.Width - 20, alumineY);

        for (int i = 0; i < paevad.Count; i++)
        {
            var (paev, summa) = paevad[i];
            int x = algusX + i * samm;
            int korgus = (int)(summa / maksimum * graafikuKorgus);
            if (korgus > 0)
                e.Graphics.FillRectangle(sinine, x, alumineY - korgus, 48, korgus);

            e.Graphics.DrawString(paev.ToString("dd.MM"), tekstFont,
                Brushes.DimGray, x, alumineY + 7);
            e.Graphics.DrawString($"{summa:F0} €", tekstFont,
                Brushes.Black, x, Math.Max(3, alumineY - korgus - 20));
        }
    }
}


using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

public partial class Form3 : Form
{
    private readonly ToodedContext _db = new();
    private readonly TabControl tabPood = new();

    public Form3()
    {
        InitializeComponent();

        Text = "Pood";
        ClientSize = new Size(930, 630);
        StartPosition = FormStartPosition.CenterScreen;

        BackColor = Color.FromArgb(245, 247, 250);
        Font = new Font("Segoe UI", 10);

        tabPood.Dock = DockStyle.Fill;
        tabPood.Font = new Font("Segoe UI", 10, FontStyle.Bold);

        Controls.Add(tabPood);

        LaePood();
    }

    // ---------- Poe laadimine ----------
    private void LaePood()
    {
        tabPood.TabPages.Clear();

        var kategooriad = _db.Kategooriatabel
            .Include(k => k.Tooded)
            .OrderBy(k => k.Kategooria_nimetus)
            .ToList();

        foreach (var kat in kategooriad)
        {
            var leht = new TabPage(kat.Kategooria_nimetus)
            {
                AutoScroll = true,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            int i = 0;

            foreach (var toode in kat.Tooded.OrderBy(t => t.Toodenimetus))
            {
                var kaart = LooTootekaart(toode);

                // Viis toodet ühes reas
                kaart.Location = new Point(
                    10 + (i % 5) * 175,
                    10 + (i / 5) * 250
                );

                leht.Controls.Add(kaart);
                i++;
            }

            tabPood.TabPages.Add(leht);
        }
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
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80)
        };

        // Toote hind
        var siltHind = new Label
        {
            Bounds = new Rectangle(5, 164, 148, 25),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = $"{t.Hind:F2} €",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(39, 160, 96)
        };

        // Koguse valik
        var kogus = new NumericUpDown
        {
            Bounds = new Rectangle(7, 197, 55, 27),
            Minimum = 1,
            Maximum = Math.Max(1, t.Kogus),
            Value = 1,
            Font = new Font("Segoe UI", 9)
        };

        // Lisa ostukorvi nupp
        var nupp = new Button
        {
            Bounds = new Rectangle(67, 195, 86, 30),
            Text = "Lisa korvi",
            Enabled = t.Kogus > 0,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        nupp.FlatAppearance.BorderSize = 0;

        // Ostukorvi lisamine
        nupp.Click += (s, e) =>
        {
            Ostukorv.Lisa(t, (int)kogus.Value);

            MessageBox.Show(
                $"{t.Toodenimetus} ({kogus.Value} tk) lisati ostukorvi.");
        };

        // Toote info kuvamine
        pilt.Click += (s, e) =>
        {
            MessageBox.Show(
                $"{t.Toodenimetus}\n" +
                $"Hind: {t.Hind:F2} €\n" +
                $"Laos: {t.Kogus} tk",
                "Toote info");
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

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _db.Dispose();
        base.OnFormClosed(e);
    }

    private void Form3_Load(object sender, EventArgs e)
    {
    }
}


using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

public class KasutajaProfiilForm : Form
{
    private readonly Label lblKasutaja = new();
    private readonly Label lblRoll = new();
    private readonly DataGridView tabel = new();

    private readonly TextBox txtVanaParool = new();
    private readonly TextBox txtUusParool = new();
    private readonly TextBox txtKordaParool = new();

    public KasutajaProfiilForm()
    {
        Text = "Minu profiil";
        ClientSize = new Size(760, 590);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new System.Drawing.Font("Segoe UI", 10);

        var pealkiri = new Label
        {
            Text = "MINU PROFIIL",
            Location = new Point(20, 15),
            AutoSize = true,
            Font = new System.Drawing.Font(
                "Segoe UI", 16, FontStyle.Bold)
        };

        lblKasutaja.SetBounds(20, 58, 710, 28);
        lblRoll.SetBounds(20, 88, 710, 28);

        var ajalugu = new Label
        {
            Text = "Minu ostude ajalugu",
            Location = new Point(20, 130),
            AutoSize = true,
            Font = new System.Drawing.Font(
                "Segoe UI", 12, FontStyle.Bold)
        };

        tabel.SetBounds(20, 165, 720, 195);
        tabel.ReadOnly = true;
        tabel.AllowUserToAddRows = false;
        tabel.AllowUserToDeleteRows = false;
        tabel.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        tabel.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;
        tabel.RowHeadersVisible = false;

        var paroolPealkiri = new Label
        {
            Text = "Muuda parooli",
            Location = new Point(20, 380),
            AutoSize = true,
            Font = new System.Drawing.Font(
                "Segoe UI", 12, FontStyle.Bold)
        };

        var lblVana = new Label
        {
            Text = "Praegune parool",
            Location = new Point(20, 415),
            AutoSize = true
        };

        var lblUus = new Label
        {
            Text = "Uus parool",
            Location = new Point(20, 455),
            AutoSize = true
        };

        var lblKorda = new Label
        {
            Text = "Korda uut parooli",
            Location = new Point(20, 495),
            AutoSize = true
        };

        txtVanaParool.SetBounds(205, 411, 255, 28);
        txtUusParool.SetBounds(205, 451, 255, 28);
        txtKordaParool.SetBounds(205, 491, 255, 28);

        txtVanaParool.UseSystemPasswordChar = true;
        txtUusParool.UseSystemPasswordChar = true;
        txtKordaParool.UseSystemPasswordChar = true;

        var btnSalvesta = new Button
        {
            Text = "Salvesta uus parool",
            Bounds = new Rectangle(490, 450, 245, 40),
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        btnSalvesta.FlatAppearance.BorderSize = 0;
        btnSalvesta.Click += SalvestaParool;

        Controls.AddRange(new Control[]
        {
            pealkiri,
            lblKasutaja,
            lblRoll,
            ajalugu,
            tabel,
            paroolPealkiri,
            lblVana,
            lblUus,
            lblKorda,
            txtVanaParool,
            txtUusParool,
            txtKordaParool,
            btnSalvesta
        });

        LaeAndmed();
    }

    private void LaeAndmed()
    {
        using var db = new ToodedContext();

        var kasutaja = db.Kasutajad.AsNoTracking()
            .FirstOrDefault(k => k.Id == AktiivneKasutaja.Id);

        if (kasutaja == null)
        {
            MessageBox.Show("Kasutajat ei leitud.");
            Close();
            return;
        }

        lblKasutaja.Text =
            $"Nimi: {kasutaja.Nimi}     E-post: {kasutaja.Email}";

        lblRoll.Text = $"Roll: {kasutaja.Roll}";

        var ostud = db.Ostud.AsNoTracking()
            .Where(o => o.KasutajaId == AktiivneKasutaja.Id)
            .OrderByDescending(o => o.Kuupaev)
            .Select(o => new
            {
                Number = o.Id,
                Date = o.Kuupaev,
                Total = o.Kokku
            })
            .ToList()
            .Select(o => new
            {
                Ost = o.Number,
                Kuupäev = o.Date.ToString("dd.MM.yyyy HH:mm"),
                Summa = $"{o.Total:F2} €"
            })
            .ToList();

        tabel.DataSource = ostud;
    }

    private void SalvestaParool(object? sender, EventArgs e)
    {
        string vana = txtVanaParool.Text;
        string uus = txtUusParool.Text;

        if (uus.Length < 8)
        {
            MessageBox.Show(
                "Uus parool peab olema vähemalt 8 tähemärki.");
            return;
        }

        if (uus != txtKordaParool.Text)
        {
            MessageBox.Show("Uued paroolid ei ühti.");
            return;
        }

        try
        {
            using var db = new ToodedContext();

            var kasutaja = db.Kasutajad
                .FirstOrDefault(k =>
                    k.Id == AktiivneKasutaja.Id);

            if (kasutaja == null ||
                !PasswordHelper.Verify(
                    vana, kasutaja.ParoolHash))
            {
                MessageBox.Show("Praegune parool on vale.");
                return;
            }

            kasutaja.ParoolHash =
                BCrypt.Net.BCrypt.HashPassword(uus);

            db.SaveChanges();

            txtVanaParool.Clear();
            txtUusParool.Clear();
            txtKordaParool.Clear();

            MessageBox.Show("Parool muudetud!");
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Parooli muutmine ebaõnnestus: " + ex.Message);
        }
    }
}

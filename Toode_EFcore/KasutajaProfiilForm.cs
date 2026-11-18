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

    private readonly Label pealkiri = new();
    private readonly Label ajalugu = new();
    private readonly Label paroolPealkiri = new();
    private readonly Label lblVana = new();
    private readonly Label lblUus = new();
    private readonly Label lblKorda = new();
    private readonly Button btnSalvesta = new();

    public KasutajaProfiilForm()
    {
        ClientSize = new Size(760, 590);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new System.Drawing.Font("Segoe UI", 10);

        pealkiri.Location = new Point(20, 15);
        pealkiri.AutoSize = true;
        pealkiri.Font = new System.Drawing.Font("Segoe UI", 16, FontStyle.Bold);

        lblKasutaja.SetBounds(20, 58, 710, 28);
        lblRoll.SetBounds(20, 88, 710, 28);

        ajalugu.Location = new Point(20, 130);
        ajalugu.AutoSize = true;
        ajalugu.Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold);

        tabel.SetBounds(20, 165, 720, 195);
        tabel.ReadOnly = true;
        tabel.AllowUserToAddRows = false;
        tabel.AllowUserToDeleteRows = false;
        tabel.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tabel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tabel.RowHeadersVisible = false;

        paroolPealkiri.Location = new Point(20, 380);
        paroolPealkiri.AutoSize = true;
        paroolPealkiri.Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold);

        lblVana.Location = new Point(20, 415);
        lblVana.AutoSize = true;
        lblUus.Location = new Point(20, 455);
        lblUus.AutoSize = true;
        lblKorda.Location = new Point(20, 495);
        lblKorda.AutoSize = true;

        txtVanaParool.SetBounds(205, 411, 255, 28);
        txtUusParool.SetBounds(205, 451, 255, 28);
        txtKordaParool.SetBounds(205, 491, 255, 28);
        txtVanaParool.UseSystemPasswordChar = true;
        txtUusParool.UseSystemPasswordChar = true;
        txtKordaParool.UseSystemPasswordChar = true;

        btnSalvesta.Bounds = new Rectangle(490, 450, 245, 40);
        btnSalvesta.BackColor = Color.FromArgb(52, 152, 219);
        btnSalvesta.ForeColor = Color.White;
        btnSalvesta.FlatStyle = FlatStyle.Flat;
        btnSalvesta.FlatAppearance.BorderSize = 0;
        btnSalvesta.Click += SalvestaParool;

        Controls.AddRange(new Control[]
        {
            pealkiri, lblKasutaja, lblRoll, ajalugu, tabel,
            paroolPealkiri, lblVana, lblUus, lblKorda,
            txtVanaParool, txtUusParool, txtKordaParool, btnSalvesta
        });

        RakendaKeel();
        LaeAndmed();
        Activated += (s, e) => RakendaKeel();
    }

    private void RakendaKeel()
    {
        Text = Keel.T("Minu profiil", "My profile");
        pealkiri.Text = Keel.T("MINU PROFIIL", "MY PROFILE");
        ajalugu.Text = Keel.T("Minu ostude ajalugu", "My purchase history");
        paroolPealkiri.Text = Keel.T("Muuda parooli", "Change password");
        lblVana.Text = Keel.T("Praegune parool", "Current password");
        lblUus.Text = Keel.T("Uus parool", "New password");
        lblKorda.Text = Keel.T("Korda uut parooli", "Repeat new password");
        btnSalvesta.Text = Keel.T("Salvesta uus parool", "Save new password");

        if (tabel.Columns.Contains("Ost"))
            tabel.Columns["Ost"]!.HeaderText = Keel.T("Ost", "Order");
        if (tabel.Columns.Contains("Kuupäev"))
            tabel.Columns["Kuupäev"]!.HeaderText = Keel.T("Kuupäev", "Date");
        if (tabel.Columns.Contains("Summa"))
            tabel.Columns["Summa"]!.HeaderText = Keel.T("Summa", "Total");

        if (!string.IsNullOrEmpty(AktiivneKasutaja.Email))
        {
            lblKasutaja.Text =
                $"{Keel.T("Nimi", "Name")}: {AktiivneKasutaja.Nimi}     " +
                $"{Keel.T("E-post", "Email")}: {AktiivneKasutaja.Email}";
            lblRoll.Text = $"{Keel.T("Roll", "Role")}: " +
                (AktiivneKasutaja.OnAdmin
                    ? Keel.T("Administraator", "Administrator")
                    : Keel.T("Kasutaja", "User"));
        }
    }

    private void LaeAndmed()
    {
        using var db = new ToodedContext();
        var kasutaja = db.Kasutajad.AsNoTracking()
            .FirstOrDefault(k => k.Id == AktiivneKasutaja.Id);

        if (kasutaja == null)
        {
            MessageBox.Show(Keel.T("Kasutajat ei leitud.", "User not found."));
            Close();
            return;
        }

        lblKasutaja.Text =
            $"{Keel.T("Nimi", "Name")}: {kasutaja.Nimi}     " +
            $"{Keel.T("E-post", "Email")}: {kasutaja.Email}";
        lblRoll.Text = $"{Keel.T("Roll", "Role")}: " +
            (AktiivneKasutaja.OnAdmin
                ? Keel.T("Administraator", "Administrator")
                : Keel.T("Kasutaja", "User"));

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
        RakendaKeel();
    }

    private void SalvestaParool(object? sender, EventArgs e)
    {
        string vana = txtVanaParool.Text;
        string uus = txtUusParool.Text;

        if (uus.Length < 8)
        {
            MessageBox.Show(Keel.T(
                "Uus parool peab olema vähemalt 8 tähemärki.",
                "The new password must be at least 8 characters long."));
            return;
        }

        if (uus != txtKordaParool.Text)
        {
            MessageBox.Show(Keel.T(
                "Uued paroolid ei ühti.", "The new passwords do not match."));
            return;
        }

        try
        {
            using var db = new ToodedContext();
            var kasutaja = db.Kasutajad
                .FirstOrDefault(k => k.Id == AktiivneKasutaja.Id);

            if (kasutaja == null ||
                !PasswordHelper.Verify(vana, kasutaja.ParoolHash))
            {
                MessageBox.Show(Keel.T(
                    "Praegune parool on vale.", "Current password is incorrect."));
                return;
            }

            kasutaja.ParoolHash = BCrypt.Net.BCrypt.HashPassword(uus);
            db.SaveChanges();

            txtVanaParool.Clear();
            txtUusParool.Clear();
            txtKordaParool.Clear();

            MessageBox.Show(Keel.T("Parool muudetud!", "Password changed!"));
        }
        catch (Exception ex)
        {
            MessageBox.Show(Keel.T(
                "Parooli muutmine ebaõnnestus: ",
                "Failed to change password: ") + ex.Message);
        }
    }
}

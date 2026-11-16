using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

public class OstudeAjaluguForm : Form
{
    private readonly DataGridView tabel = new();
    private readonly DataGridView read = new();
    private readonly Label lblKokku = new();

    public OstudeAjaluguForm()
    {
        Text = "Ostude ajalugu";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(830, 560);
        BackColor = Color.WhiteSmoke;
        Font = new System.Drawing.Font("Segoe UI", 10);

        var pealkiri = new Label
        {
            Text = "OSTUDE AJALUGU",
            Dock = DockStyle.Top,
            Height = 45,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new System.Drawing.Font("Segoe UI", 15, FontStyle.Bold)
        };

        tabel.Dock = DockStyle.Top;
        tabel.Height = 235;
        tabel.ReadOnly = true;
        tabel.AllowUserToAddRows = false;
        tabel.AllowUserToDeleteRows = false;
        tabel.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tabel.MultiSelect = false;
        tabel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tabel.RowHeadersVisible = false;
        tabel.Columns.Add("Id", "Ostu nr");
        tabel.Columns.Add("Kuupaev", "Kuupäev");
        tabel.Columns.Add("Kokku", "Kokku (€)");
        tabel.SelectionChanged += (s, e) => NaitaOstuRead();

        var lblRead = new Label
        {
            Text = "Ostu tooted",
            Dock = DockStyle.Top,
            Height = 34,
            TextAlign = ContentAlignment.MiddleLeft
        };

        read.Dock = DockStyle.Fill;
        read.ReadOnly = true;
        read.AllowUserToAddRows = false;
        read.AllowUserToDeleteRows = false;
        read.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        read.RowHeadersVisible = false;
        read.Columns.Add("Toode", "Toode");
        read.Columns.Add("Kogus", "Kogus");
        read.Columns.Add("Hind", "Hind (€)");
        read.Columns.Add("Summa", "Summa (€)");

        lblKokku.Dock = DockStyle.Bottom;
        lblKokku.Height = 40;
        lblKokku.TextAlign = ContentAlignment.MiddleRight;
        lblKokku.Padding = new Padding(0, 0, 15, 0);
        lblKokku.Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold);

        Controls.Add(read);
        Controls.Add(lblKokku);
        Controls.Add(lblRead);
        Controls.Add(tabel);
        Controls.Add(pealkiri);

        LaeOstud();
    }

    private void LaeOstud()
    {
        using var db = new ToodedContext();
        var ostud = db.Ostud.AsNoTracking()
            .OrderByDescending(o => o.Kuupaev)
            .ThenByDescending(o => o.Id)
            .ToList();

        tabel.Rows.Clear();
        foreach (var ost in ostud)
            tabel.Rows.Add(ost.Id, ost.Kuupaev.ToString("dd.MM.yyyy HH:mm"), ost.Kokku.ToString("F2"));

        if (tabel.Rows.Count > 0)
            tabel.Rows[0].Selected = true;
        NaitaOstuRead();
    }

    private void NaitaOstuRead()
    {
        read.Rows.Clear();
        lblKokku.Text = "";

        if (tabel.SelectedRows.Count == 0)
            return;

        int ostuId = Convert.ToInt32(tabel.SelectedRows[0].Cells[0].Value);

        using var db = new ToodedContext();
        var ost = db.Ostud.AsNoTracking()
            .Include(o => o.Read)
            .FirstOrDefault(o => o.Id == ostuId);

        if (ost == null)
            return;

        foreach (var rida in ost.Read)
            read.Rows.Add(rida.TooteNimi, rida.Kogus,
                rida.Hind.ToString("F2"), rida.Summa.ToString("F2"));

        lblKokku.Text = $"Kokku: {ost.Kokku:F2} €";
    }
}

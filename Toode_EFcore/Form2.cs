
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

public partial class Form2 : Form
{
    private readonly ToodedContext _db = new();
    private string? valitudPilt;

    private readonly DataGridView dataGridViewTooted = new();
    private readonly TextBox txtNimetus = new(), txtKogus = new(), txtHind = new();
    private readonly ComboBox comboKategooria = new();
    private readonly PictureBox picturePilt = new();

    private readonly Button btnValiPilt = new(), btnLisa = new(),
        btnUuenda = new(), btnKustuta = new(), btnTyhjenda = new();

    public Form2()
    {
        InitializeComponent();

        LooLiides();
        RakendaDisain();
        LaeKategooriad();
        LaeTooted();

        if (!AktiivneKasutaja.OnAdmin)
        {
            btnLisa.Enabled = false;
            btnUuenda.Enabled = false;
            btnKustuta.Enabled = false;
            btnValiPilt.Enabled = false;
        }
    }

    // ---------- Kasutajaliides ----------
    private void LooLiides()
    {
        Text = "Toodete haldus";
        ClientSize = new Size(860, 560);

        dataGridViewTooted.SetBounds(12, 12, 600, 530);
        dataGridViewTooted.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewTooted.MultiSelect = false;
        dataGridViewTooted.ReadOnly = true;
        dataGridViewTooted.AllowUserToAddRows = false;
        dataGridViewTooted.CellClick += dataGridViewTooted_CellClick;

        txtNimetus.SetBounds(630, 12, 200, 27);
        txtNimetus.PlaceholderText = "Toote nimetus";

        txtKogus.SetBounds(630, 50, 200, 27);
        txtKogus.PlaceholderText = "Kogus";

        txtHind.SetBounds(630, 88, 200, 27);
        txtHind.PlaceholderText = "Hind €, nt 2,50";

        comboKategooria.SetBounds(630, 126, 200, 27);
        comboKategooria.DropDownStyle = ComboBoxStyle.DropDownList;

        picturePilt.SetBounds(630, 166, 150, 150);
        picturePilt.SizeMode = PictureBoxSizeMode.Zoom;
        picturePilt.BorderStyle = BorderStyle.FixedSingle;

        SeaNupp(btnValiPilt, "Vali pilt", 326, btnValiPilt_Click);
        SeaNupp(btnLisa, "Lisa", 376, btnLisa_Click);
        SeaNupp(btnUuenda, "Uuenda", 416, btnUuenda_Click);
        SeaNupp(btnKustuta, "Kustuta", 456, btnKustuta_Click);
        SeaNupp(btnTyhjenda, "Tühjenda väljad", 506,
            (s, e) => TyhjendaVorm());

        Controls.AddRange(new Control[]
        {
            dataGridViewTooted,
            txtNimetus,
            txtKogus,
            txtHind,
            comboKategooria,
            picturePilt,
            btnValiPilt,
            btnLisa,
            btnUuenda,
            btnKustuta,
            btnTyhjenda
        });
    }

    private void SeaNupp(Button nupp, string tekst, int y,
        EventHandler kasitleja)
    {
        nupp.Text = tekst;
        nupp.SetBounds(630, y, 200, 32);
        nupp.Click += kasitleja;
    }

    // ---------- Kujundus ----------
    private void RakendaDisain()
    {
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new Font("Segoe UI", 10);
        StartPosition = FormStartPosition.CenterScreen;

        // Tabel
        dataGridViewTooted.BackgroundColor = Color.White;
        dataGridViewTooted.BorderStyle = BorderStyle.None;
        dataGridViewTooted.RowHeadersVisible = false;
        dataGridViewTooted.EnableHeadersVisualStyles = false;

        dataGridViewTooted.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        dataGridViewTooted.ColumnHeadersHeight = 38;
        dataGridViewTooted.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        dataGridViewTooted.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(41, 98, 160);

        dataGridViewTooted.ColumnHeadersDefaultCellStyle.ForeColor =
            Color.White;

        dataGridViewTooted.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 10, FontStyle.Bold);

        dataGridViewTooted.RowTemplate.Height = 30;

        dataGridViewTooted.DefaultCellStyle.SelectionBackColor =
            Color.FromArgb(209, 229, 248);

        dataGridViewTooted.DefaultCellStyle.SelectionForeColor =
            Color.Black;

        dataGridViewTooted.AlternatingRowsDefaultCellStyle.BackColor =
            Color.FromArgb(245, 249, 253);

        // Sisestusväljad
        txtNimetus.BackColor = Color.White;
        txtKogus.BackColor = Color.White;
        txtHind.BackColor = Color.White;
        comboKategooria.BackColor = Color.White;

        // Toote pilt
        picturePilt.BackColor = Color.White;

        // Nupud
        StiiliNupp(btnValiPilt, Color.FromArgb(52, 152, 219));
        StiiliNupp(btnLisa, Color.FromArgb(46, 160, 100));
        StiiliNupp(btnUuenda, Color.FromArgb(220, 150, 30));
        StiiliNupp(btnKustuta, Color.FromArgb(210, 70, 70));
        StiiliNupp(btnTyhjenda, Color.FromArgb(110, 125, 140));
    }

    private void StiiliNupp(Button nupp, Color varv)
    {
        nupp.BackColor = varv;
        nupp.ForeColor = Color.White;
        nupp.FlatStyle = FlatStyle.Flat;
        nupp.FlatAppearance.BorderSize = 0;
        nupp.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        nupp.Cursor = Cursors.Hand;
    }

    // ---------- Andmete laadimine ----------
    private void LaeKategooriad()
    {
        comboKategooria.DataSource = _db.Kategooriatabel
            .OrderBy(k => k.Kategooria_nimetus)
            .ToList();

        comboKategooria.DisplayMember = "Kategooria_nimetus";
        comboKategooria.ValueMember = "Id";
    }

    private void LaeTooted()
    {
        dataGridViewTooted.DataSource = _db.Toodetabel
            .Select(t => new
            {
                t.Id,
                t.Toodenimetus,
                t.Kogus,
                t.Hind,
                t.KategooriaId,
                Kategooria = t.Kategooria.Kategooria_nimetus,
                t.Pilt
            })
            .ToList();

        dataGridViewTooted.Columns["KategooriaId"]!.Visible = false;
    }

    // ---------- Pildi valimine ----------
    private void btnValiPilt_Click(object? sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "Pildid|*.jpg;*.jpeg;*.png"
        };

        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        valitudPilt = ofd.FileName;

        using var ajutine = Image.FromFile(valitudPilt);
        picturePilt.Image = new Bitmap(ajutine);
    }

    // ---------- Vormi andmete kontroll ----------
    private bool LoeVorm(Toode toode)
    {
        if (string.IsNullOrWhiteSpace(txtNimetus.Text))
        {
            MessageBox.Show("Sisesta toote nimetus.");
            return false;
        }

        if (!int.TryParse(txtKogus.Text, out int kogus) || kogus < 0)
        {
            MessageBox.Show("Kogus peab olema täisarv (0 või rohkem).");
            return false;
        }

        if (!decimal.TryParse(
            txtHind.Text.Replace(',', '.'),
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out decimal hind) || hind < 0)
        {
            MessageBox.Show("Hind peab olema arv, nt 2,50.");
            return false;
        }

        if (comboKategooria.SelectedValue is not int kategooriaId)
        {
            MessageBox.Show("Vali kategooria.");
            return false;
        }

        toode.Toodenimetus = txtNimetus.Text.Trim();
        toode.Kogus = kogus;
        toode.Hind = hind;
        toode.KategooriaId = kategooriaId;

        if (valitudPilt != null)
        {
            toode.Pilt = Path.GetFileName(valitudPilt);

            string siht = Path.Combine(Kaustad.Pildid, toode.Pilt);

            if (!string.Equals(
                Path.GetFullPath(valitudPilt),
                siht,
                StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(valitudPilt, siht, true);
            }
        }

        return true;
    }

    private int? ValitudId()
    {
        if (dataGridViewTooted.SelectedRows.Count == 0)
        {
            MessageBox.Show("Vali tabelist toode.");
            return null;
        }

        return (int)dataGridViewTooted
            .SelectedRows[0].Cells["Id"].Value;
    }

    private void TyhjendaVorm()
    {
        txtNimetus.Clear();
        txtKogus.Clear();
        txtHind.Clear();

        valitudPilt = null;
        picturePilt.Image = null;
    }

    // ---------- CRUD ----------
    private void btnLisa_Click(object? sender, EventArgs e)
    {
        var uus = new Toode();

        if (!LoeVorm(uus))
            return;

        _db.Toodetabel.Add(uus);
        _db.SaveChanges();

        TyhjendaVorm();
        LaeTooted();
    }

    private void dataGridViewTooted_CellClick(
        object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        var rida = dataGridViewTooted.Rows[e.RowIndex];

        txtNimetus.Text =
            rida.Cells["Toodenimetus"].Value?.ToString();

        txtKogus.Text =
            rida.Cells["Kogus"].Value?.ToString();

        txtHind.Text =
            rida.Cells["Hind"].Value?.ToString();

        comboKategooria.SelectedValue =
            rida.Cells["KategooriaId"].Value;

        valitudPilt = null;

        picturePilt.Image = Kaustad.LaePilt(
            rida.Cells["Pilt"].Value?.ToString());
    }

    private void btnUuenda_Click(object? sender, EventArgs e)
    {
        if (ValitudId() is not int id)
            return;

        var toode = _db.Toodetabel.Find(id);

        if (toode == null || !LoeVorm(toode))
            return;

        _db.SaveChanges();
        LaeTooted();
    }

    private void btnKustuta_Click(object? sender, EventArgs e)
    {
        if (ValitudId() is not int id)
            return;

        if (MessageBox.Show(
            "Kas kustutan valitud toote?",
            "Kinnitus",
            MessageBoxButtons.YesNo) != DialogResult.Yes)
        {
            return;
        }

        var toode = _db.Toodetabel.Find(id);

        if (toode == null)
            return;

        _db.Toodetabel.Remove(toode);
        _db.SaveChanges();

        TyhjendaVorm();
        LaeTooted();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _db.Dispose();
        base.OnFormClosed(e);
    }

    private void Form2_Load(object sender, EventArgs e)
    {
    }
}

using System.Net;
using System.Net.Mail;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Tooded_EFCore;

public partial class ArveForm : Form
{
    private readonly Label lblPealkiri = new();
    private readonly ListBox listOstukorv = new();
    private readonly Label lblKokku = new();
    private readonly TextBox txtEmail = new();
    private readonly Button btnEemalda = new();
    private readonly Button btnGenPDF = new();
    private readonly Button btnSaadaEmail = new();
    private readonly Button btnKinnitaOst = new();
    private readonly Button btnOstudeAjalugu = new();

    private string? viimanePdf;

    public ArveForm()
    {
        InitializeComponent();

        Text = "Ostukorv ja arve";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(640, 460);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.WhiteSmoke;
        Font = new System.Drawing.Font("Segoe UI", 10);

        lblPealkiri.Text = "OSTUKORV JA ARVE";
        lblPealkiri.SetBounds(20, 15, 250, 25);
        lblPealkiri.Font = new System.Drawing.Font("Segoe UI", 13, FontStyle.Bold);
        lblPealkiri.ForeColor = Color.FromArgb(44, 62, 80);

        listOstukorv.SetBounds(20, 50, 340, 330);
        listOstukorv.Font = new System.Drawing.Font("Segoe UI", 10);
        listOstukorv.BorderStyle = BorderStyle.FixedSingle;

        lblKokku.SetBounds(20, 395, 340, 30);
        lblKokku.Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold);
        lblKokku.ForeColor = Color.FromArgb(39, 174, 96);

        var lblEmailTekst = new Label
        {
            Text = "Saaja e-post",
            Location = new Point(390, 20),
            Size = new Size(150, 20),
            Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold)
        };

        txtEmail.SetBounds(390, 50, 210, 30);
        txtEmail.PlaceholderText = "näiteks nimi@gmail.com";

        SeaNupp(btnEemalda, "Eemalda valitud rida", 105, btnEemalda_Click);
        SeaNupp(btnGenPDF, "Genereeri PDF", 155, btnGenPDF_Click);
        SeaNupp(btnSaadaEmail, "Saada e-postiga", 205, btnSaadaEmail_Click);
        SeaNupp(btnKinnitaOst, "Kinnita ost", 255, btnKinnitaOst_Click);
        btnKinnitaOst.BackColor = Color.FromArgb(39, 174, 96);
        SeaNupp(btnOstudeAjalugu, "Ostude ajalugu", 305, btnOstudeAjalugu_Click);

        Controls.AddRange(new Control[]
        {
            lblPealkiri, listOstukorv, lblKokku,
            lblEmailTekst, txtEmail, btnEemalda,
            btnGenPDF, btnSaadaEmail, btnKinnitaOst, btnOstudeAjalugu
        });

        NaitaKorvi();
    }

    private void SeaNupp(Button nupp, string tekst, int y, EventHandler kasitleja)
    {
        nupp.Text = tekst;
        nupp.SetBounds(390, y, 210, 38);
        nupp.BackColor = Color.FromArgb(52, 152, 219);
        nupp.ForeColor = Color.White;
        nupp.FlatStyle = FlatStyle.Flat;
        nupp.FlatAppearance.BorderSize = 0;
        nupp.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
        nupp.Cursor = Cursors.Hand;
        nupp.Click += kasitleja;
    }

    private void NaitaKorvi()
    {
        listOstukorv.Items.Clear();
        foreach (var rida in Ostukorv.Ridad)
            listOstukorv.Items.Add(rida);
        lblKokku.Text = $"Kokku: {Ostukorv.Kokku:F2} €";
    }

    private void btnEemalda_Click(object? sender, EventArgs e)
    {
        if (listOstukorv.SelectedItem is OstukorviRida rida)
        {
            Ostukorv.Ridad.Remove(rida);
            viimanePdf = null;
            NaitaKorvi();
        }
    }

    private void btnKinnitaOst_Click(object? sender, EventArgs e)
    {
        if (Ostukorv.Ridad.Count == 0)
        {
            MessageBox.Show("Ostukorv on tühi.");
            return;
        }

        if (MessageBox.Show("Kas kinnitad ostu?", "Ostu kinnitamine",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            var ost = new Ost
            {
                Kuupaev = DateTime.Now,
                Kokku = Ostukorv.Kokku,
                Read = Ostukorv.Ridad.Select(r => new Osturida
                {
                    ToodeId = r.Toode.Id,
                    TooteNimi = r.Toode.Toodenimetus,
                    Kogus = r.Kogus,
                    Hind = r.Toode.Hind,
                    Summa = r.Summa
                }).ToList()
            };

            using var db = new ToodedContext();
            db.Ostud.Add(ost);
            db.SaveChanges();

            Ostukorv.Tyhjenda();
            NaitaKorvi();
            MessageBox.Show($"Ost nr {ost.Id} salvestati andmebaasi!");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ostu salvestamine ebaõnnestus: " + ex.Message);
        }
    }

    private void btnOstudeAjalugu_Click(object? sender, EventArgs e)
    {
        using var ajalugu = new OstudeAjaluguForm();
        ajalugu.ShowDialog(this);
    }

    private void btnGenPDF_Click(object? sender, EventArgs e)
    {
        if (Ostukorv.Ridad.Count == 0)
        {
            MessageBox.Show("Ostukorv on tühi.");
            return;
        }

        try
        {
            Directory.CreateDirectory(Kaustad.Arved);
            viimanePdf = Path.Combine(
                Kaustad.Arved,
                $"Arve_{DateTime.Now:yyyyMMdd_HHmmss_fff}.pdf");

            using var dokument = new PdfDocument();
            PdfPage leht = dokument.AddPage();

            using (XGraphics gfx = XGraphics.FromPdfPage(leht))
            {
                XFont pealkiri = new XFont("Arial", 20, XFontStyle.Bold);
                XFont alapealkiri = new XFont("Arial", 14, XFontStyle.Bold);
                XFont tavaline = new XFont("Arial", 11);
                XFont paks = new XFont("Arial", 12, XFontStyle.Bold);

                gfx.DrawString("TOODETE POOD", pealkiri, XBrushes.DarkBlue,
                    new XRect(0, 25, leht.Width, 30), XStringFormats.TopCenter);

                gfx.DrawString("ARVE", alapealkiri, XBrushes.Black,
                    new XRect(0, 55, leht.Width, 25), XStringFormats.TopCenter);

                gfx.DrawString($"Kuupäev: {DateTime.Now:dd.MM.yyyy}",
                    tavaline, XBrushes.Black, 40, 95);

                gfx.DrawLine(XPens.Gray, 40, 110, leht.Width - 40, 110);

                gfx.DrawString("Toode", paks, XBrushes.Black, 40, 130);
                gfx.DrawString("Kogus", paks, XBrushes.Black, 270, 130);
                gfx.DrawString("Hind", paks, XBrushes.Black, 350, 130);
                gfx.DrawString("Summa", paks, XBrushes.Black, 450, 130);

                double y = 155;
                foreach (var rida in Ostukorv.Ridad)
                {
                    gfx.DrawString(rida.Toode.Toodenimetus, tavaline, XBrushes.Black, 40, y);
                    gfx.DrawString($"{rida.Kogus} tk", tavaline, XBrushes.Black, 270, y);
                    gfx.DrawString($"{rida.Toode.Hind:F2} €", tavaline, XBrushes.Black, 350, y);
                    gfx.DrawString($"{rida.Summa:F2} €", tavaline, XBrushes.Black, 450, y);
                    y += 22;
                }

                gfx.DrawLine(XPens.Gray, 40, y + 5, leht.Width - 40, y + 5);
                gfx.DrawString($"Kokku: {Ostukorv.Kokku:F2} €",
                    new XFont("Arial", 14, XFontStyle.Bold),
                    XBrushes.DarkGreen, 40, y + 30);
            }

            dokument.Save(viimanePdf);
            MessageBox.Show($"PDF on loodud:\n{viimanePdf}");
        }
        catch (Exception ex)
        {
            MessageBox.Show("PDF-i loomine ebaõnnestus: " + ex.Message);
        }
    }

    private void btnSaadaEmail_Click(object? sender, EventArgs e)
    {
        if (viimanePdf == null || !File.Exists(viimanePdf))
        {
            MessageBox.Show("Genereeri enne PDF.");
            return;
        }

        string saaja = txtEmail.Text.Trim();
        if (!MailAddress.TryCreate(saaja, out _))
        {
            MessageBox.Show("Sisesta korrektne e-posti aadress.");
            return;
        }

        // Turvalisuse huvides ei salvestata Gmaili parooli lähtekoodi.
        string saatja = Environment.GetEnvironmentVariable("TOODED_EMAIL") ?? "";
        string rakenduseParool = Environment.GetEnvironmentVariable("TOODED_APP_PASSWORD") ?? "";

        if (string.IsNullOrWhiteSpace(saatja) || string.IsNullOrWhiteSpace(rakenduseParool))
        {
            MessageBox.Show("E-posti saatmine pole seadistatud.");
            return;
        }

        try
        {
            using var kiri = new MailMessage(saatja, saaja,
                "Arve", "Arve on manuses.");
            kiri.Attachments.Add(new Attachment(viimanePdf));

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(saatja, rakenduseParool),
                EnableSsl = true
            };

            smtp.Send(kiri);
            MessageBox.Show("Arve saadetud!");
            txtEmail.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Viga: " + ex.Message);
        }
    }

    private void ArveForm_Load(object sender, EventArgs e)
    {
    }
}

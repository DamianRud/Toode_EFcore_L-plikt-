
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Mail;

namespace Tooded_EFCore;

public partial class ArveForm : Form
{
    private readonly Label lblPealkiri = new();
    private readonly Label lblPais = new();
    private readonly Label lblKirjeldus = new();
    private readonly Label lblEmailTekst = new();

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

        ClientSize = new Size(920, 600);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.FromArgb(234, 244, 255);
        Font = new System.Drawing.Font("Segoe UI", 10);
        DoubleBuffered = true;

        Controls.Clear();

        // ---------- PÄIS ----------
        var pais = new Panel
        {
            Bounds = new Rectangle(0, 0, 920, 125)
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
            Font = new System.Drawing.Font(
                "Segoe UI Emoji", 30),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
            Bounds = new Rectangle(28, 25, 70, 70)
        };

        lblPais.Font = new System.Drawing.Font(
            "Segoe UI", 24, FontStyle.Bold);
        lblPais.ForeColor = Color.White;
        lblPais.BackColor = Color.Transparent;
        lblPais.SetBounds(110, 27, 750, 50);

        lblKirjeldus.Font =
            new System.Drawing.Font("Segoe UI", 11);
        lblKirjeldus.ForeColor = Color.White;
        lblKirjeldus.BackColor = Color.Transparent;
        lblKirjeldus.SetBounds(113, 77, 750, 28);

        pais.Controls.AddRange(new Control[]
        {
            logo, lblPais, lblKirjeldus
        });

        // ---------- OSTUKORV ----------
        var korviKaart = new Panel
        {
            Bounds = new Rectangle(25, 150, 525, 415),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        lblPealkiri.SetBounds(22, 20, 460, 35);
        lblPealkiri.Font = new System.Drawing.Font(
            "Segoe UI", 16, FontStyle.Bold);
        lblPealkiri.ForeColor =
            Color.FromArgb(21, 49, 81);

        listOstukorv.SetBounds(20, 65, 480, 275);
        listOstukorv.Font =
            new System.Drawing.Font("Segoe UI", 11);
        listOstukorv.BorderStyle =
            BorderStyle.FixedSingle;
        listOstukorv.BackColor =
            Color.FromArgb(248, 251, 255);
        listOstukorv.IntegralHeight = false;
        listOstukorv.HorizontalScrollbar = true;

        lblKokku.SetBounds(20, 355, 480, 40);
        lblKokku.Font = new System.Drawing.Font(
            "Segoe UI", 17, FontStyle.Bold);
        lblKokku.ForeColor =
            Color.FromArgb(27, 160, 108);

        korviKaart.Controls.AddRange(new Control[]
        {
            lblPealkiri, listOstukorv, lblKokku
        });

        // ---------- TOIMINGUD ----------
        var tegevusteKaart = new Panel
        {
            Bounds = new Rectangle(570, 150, 315, 415),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        lblEmailTekst.SetBounds(18, 18, 275, 28);
        lblEmailTekst.Font = new System.Drawing.Font(
            "Segoe UI", 11, FontStyle.Bold);
        lblEmailTekst.ForeColor =
            Color.FromArgb(21, 49, 81);

        txtEmail.SetBounds(18, 53, 277, 32);
        txtEmail.Font =
            new System.Drawing.Font("Segoe UI", 10);
        txtEmail.BorderStyle = BorderStyle.FixedSingle;

        SeaNupp(btnEemalda, 105, btnEemalda_Click);
        btnEemalda.BackColor =
            Color.FromArgb(231, 99, 101);

        SeaNupp(btnGenPDF, 154, btnGenPDF_Click);
        SeaNupp(btnSaadaEmail, 203, btnSaadaEmail_Click);

        SeaNupp(btnKinnitaOst, 252, btnKinnitaOst_Click);
        btnKinnitaOst.BackColor =
            Color.FromArgb(34, 166, 110);

        SeaNupp(
            btnOstudeAjalugu, 301,
            btnOstudeAjalugu_Click);

        tegevusteKaart.Controls.AddRange(new Control[]
        {
            lblEmailTekst,
            txtEmail,
            btnEemalda,
            btnGenPDF,
            btnSaadaEmail,
            btnKinnitaOst,
            btnOstudeAjalugu
        });

        Controls.AddRange(new Control[]
        {
            pais, korviKaart, tegevusteKaart
        });

        RakendaKeel();
        Activated += (s, e) => RakendaKeel();
    }

    private static void SeaNupp(
        Button nupp, int y, EventHandler kasitleja)
    {
        nupp.SetBounds(18, y, 277, 41);
        nupp.Font = new System.Drawing.Font(
            "Segoe UI", 10, FontStyle.Bold);
        nupp.BackColor = Color.FromArgb(52, 152, 219);
        nupp.ForeColor = Color.White;
        nupp.FlatStyle = FlatStyle.Flat;
        nupp.FlatAppearance.BorderSize = 0;
        nupp.Cursor = Cursors.Hand;
        nupp.Click += kasitleja;
    }

    // ---------- TÕLKED ----------
    private void RakendaKeel()
    {
        Text = Keel.T("Ostukorv ja arve", "Cart and invoice");

        lblPais.Text =
            Keel.T("Ostukorv ja arve", "Cart and invoice");

        lblKirjeldus.Text = Keel.T(
            "Ostude kinnitamine ja arvete haldamine",
            "Checkout and invoice management");

        lblPealkiri.Text =
            Keel.T("MINU OSTUKORV", "MY SHOPPING CART");

        lblEmailTekst.Text =
            Keel.T("Saaja e-post", "Recipient email");

        txtEmail.PlaceholderText = Keel.T(
            "näiteks nimi@gmail.com",
            "e.g. name@gmail.com");

        btnEemalda.Text = Keel.T(
            "✕  Eemalda valitud rida",
            "✕  Remove selected item");

        btnGenPDF.Text = Keel.T(
            "▤  Genereeri PDF",
            "▤  Generate PDF");

        btnSaadaEmail.Text = Keel.T(
            "✉  Saada e-postiga",
            "✉  Send by email");

        btnKinnitaOst.Text = Keel.T(
            "✓  Kinnita ost",
            "✓  Confirm purchase");

        btnOstudeAjalugu.Text = Keel.T(
            "▥  Ostude ajalugu",
            "▥  Purchase history");

        NaitaKorvi();
    }

    // ---------- OSTUKORV ----------
    private void NaitaKorvi()
    {
        listOstukorv.Items.Clear();

        foreach (var rida in Ostukorv.Ridad)
        {
            listOstukorv.Items.Add(
                $"{rida.Toode.Toodenimetus} - " +
                $"{rida.Kogus} " +
                Keel.T("tk", "pcs") +
                $" × {rida.Toode.Hind:F2} € = " +
                $"{rida.Summa:F2} €");
        }

        lblKokku.Text =
            $"{Keel.T("Kokku", "Total")}: " +
            $"{Ostukorv.Kokku:F2} €";
    }

    private void btnEemalda_Click(
        object? sender, EventArgs e)
    {
        int index = listOstukorv.SelectedIndex;

        if (index >= 0 && index < Ostukorv.Ridad.Count)
        {
            Ostukorv.Ridad.RemoveAt(index);
            viimanePdf = null;
            NaitaKorvi();
        }
    }

    // ---------- OSTU KINNITAMINE ----------
    private void btnKinnitaOst_Click(
        object? sender, EventArgs e)
    {
        if (Ostukorv.Ridad.Count == 0)
        {
            MessageBox.Show(
                Keel.T("Ostukorv on tühi.", "Your cart is empty."));
            return;
        }

        if (MessageBox.Show(
            Keel.T("Kas kinnitad ostu?", "Confirm purchase?"),
            Keel.T("Ostu kinnitamine", "Confirm purchase"),
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        Ost ost;

        try
        {
            using var db = new ToodedContext();
            using var tehing = db.Database.BeginTransaction();

            foreach (var rida in Ostukorv.Ridad)
            {
                if (rida.Kogus <= 0)
                {
                    throw new InvalidOperationException(
                        Keel.T(
                            "Vigane kogus ostukorvis.",
                            "Invalid quantity in cart."));
                }

                int muudetud = db.Toodetabel
                    .Where(t =>
                        t.Id == rida.Toode.Id &&
                        t.Kogus >= rida.Kogus)
                    .ExecuteUpdate(seaded => seaded
                        .SetProperty(
                            t => t.Kogus,
                            t => t.Kogus - rida.Kogus));

                if (muudetud != 1)
                {
                    throw new InvalidOperationException(
                        Keel.T(
                            $"Toodet '{rida.Toode.Toodenimetus}' " +
                            "ei ole piisavalt laos.",
                            $"Not enough stock for " +
                            $"'{rida.Toode.Toodenimetus}'."));
                }
            }

            ost = new Ost
            {
                Kuupaev = DateTime.Now,
                KasutajaId = AktiivneKasutaja.Id > 0
                    ? AktiivneKasutaja.Id
                    : null,
                Kokku = Ostukorv.Kokku,

                Read = Ostukorv.Ridad.Select(r =>
                    new Osturida
                    {
                        ToodeId = r.Toode.Id,
                        TooteNimi = r.Toode.Toodenimetus,
                        Kogus = r.Kogus,
                        Hind = r.Toode.Hind,
                        Summa = r.Summa
                    }).ToList()
            };

            db.Ostud.Add(ost);
            db.SaveChanges();
            tehing.Commit();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                Keel.T(
                    "Ostu kinnitamine ebaõnnestus:",
                    "Purchase confirmation failed:") +
                "\n" + ex.Message);
            return;
        }

        Ostukorv.Tyhjenda();
        viimanePdf = null;
        NaitaKorvi();

        try
        {
            viimanePdf = LooPdf(ost);

            MessageBox.Show(
                Keel.T(
                    $"Ost nr {ost.Id} kinnitatud!\n\n" +
                    "Laoseis uuendati.\n" +
                    "Ost salvestati ajalukku.\n" +
                    $"PDF-arve loodi:\n{viimanePdf}",

                    $"Purchase #{ost.Id} confirmed!\n\n" +
                    "Stock updated.\n" +
                    "Purchase saved to history.\n" +
                    $"PDF invoice created:\n{viimanePdf}"),

                Keel.T("Ost õnnestus", "Purchase successful"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                Keel.T(
                    $"Ost nr {ost.Id} salvestati edukalt, " +
                    "kuid PDF-arvet ei õnnestunud luua.",
                    $"Purchase #{ost.Id} was saved, " +
                    "but the PDF invoice could not be created.") +
                "\n" + ex.Message,

                Keel.T("PDF-i viga", "PDF error"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    // ---------- PDF ----------
    private string LooPdf(Ost ost)
    {
        Directory.CreateDirectory(Kaustad.Arved);

        string failiNimi = ost.Id > 0
            ? $"Arve_{ost.Id}_{ost.Kuupaev:yyyyMMdd_HHmmss}.pdf"
            : $"Arve_Eelvaade_{DateTime.Now:yyyyMMdd_HHmmss_fff}.pdf";

        string fail = Path.Combine(Kaustad.Arved, failiNimi);

        using var dokument = new PdfDocument();
        PdfPage leht = dokument.AddPage();

        XFont pealkiri =
            new XFont("Arial", 20, XFontStyle.Bold);
        XFont alapealkiri =
            new XFont("Arial", 14, XFontStyle.Bold);
        XFont tavaline = new XFont("Arial", 11);
        XFont paks =
            new XFont("Arial", 12, XFontStyle.Bold);
        XFont kokkuFont =
            new XFont("Arial", 14, XFontStyle.Bold);

        void JoonistaPais(XGraphics gfx, PdfPage lk)
        {
            gfx.DrawString(
                Keel.T("TOODETE POOD", "PRODUCT STORE"),
                pealkiri,
                XBrushes.DarkBlue,
                new XRect(0, 25, lk.Width, 30),
                XStringFormats.TopCenter);

            string arvePealkiri = ost.Id > 0
                ? Keel.T($"ARVE NR {ost.Id}",
                         $"INVOICE #{ost.Id}")
                : Keel.T("ARVE EELVAADE", "INVOICE PREVIEW");

            gfx.DrawString(
                arvePealkiri, alapealkiri,
                XBrushes.Black,
                new XRect(0, 55, lk.Width, 25),
                XStringFormats.TopCenter);

            gfx.DrawString(
                $"{Keel.T("Kuupäev", "Date")}: " +
                $"{ost.Kuupaev:dd.MM.yyyy HH:mm}",
                tavaline, XBrushes.Black, 40, 95);

            gfx.DrawLine(
                XPens.Gray, 40, 110, lk.Width - 40, 110);

            gfx.DrawString(
                Keel.T("Toode", "Product"),
                paks, XBrushes.Black, 40, 130);

            gfx.DrawString(
                Keel.T("Kogus", "Quantity"),
                paks, XBrushes.Black, 270, 130);

            gfx.DrawString(
                Keel.T("Hind", "Price"),
                paks, XBrushes.Black, 350, 130);

            gfx.DrawString(
                Keel.T("Summa", "Amount"),
                paks, XBrushes.Black, 450, 130);
        }

        XGraphics gfx = XGraphics.FromPdfPage(leht);

        try
        {
            JoonistaPais(gfx, leht);
            double y = 155;

            foreach (var rida in ost.Read)
            {
                if (y > leht.Height - 100)
                {
                    gfx.Dispose();
                    leht = dokument.AddPage();
                    gfx = XGraphics.FromPdfPage(leht);
                    JoonistaPais(gfx, leht);
                    y = 155;
                }

                gfx.DrawString(
                    rida.TooteNimi,
                    tavaline, XBrushes.Black, 40, y);

                gfx.DrawString(
                    $"{rida.Kogus} {Keel.T("tk", "pcs")}",
                    tavaline, XBrushes.Black, 270, y);

                gfx.DrawString(
                    $"{rida.Hind:F2} €",
                    tavaline, XBrushes.Black, 350, y);

                gfx.DrawString(
                    $"{rida.Summa:F2} €",
                    tavaline, XBrushes.Black, 450, y);

                y += 22;
            }

            if (y > leht.Height - 85)
            {
                gfx.Dispose();
                leht = dokument.AddPage();
                gfx = XGraphics.FromPdfPage(leht);
                y = 50;
            }

            gfx.DrawLine(
                XPens.Gray, 40, y + 5,
                leht.Width - 40, y + 5);

            gfx.DrawString(
                $"{Keel.T("Kokku", "Total")}: " +
                $"{ost.Kokku:F2} €",
                kokkuFont,
                XBrushes.DarkGreen,
                40, y + 35);
        }
        finally
        {
            gfx.Dispose();
        }

        dokument.Save(fail);
        return fail;
    }

    // ---------- PDF EELVAADE ----------
    private void btnGenPDF_Click(
        object? sender, EventArgs e)
    {
        if (Ostukorv.Ridad.Count == 0)
        {
            MessageBox.Show(
                Keel.T("Ostukorv on tühi.", "Your cart is empty."));
            return;
        }

        try
        {
            var eelvaade = new Ost
            {
                Kuupaev = DateTime.Now,
                Kokku = Ostukorv.Kokku,

                Read = Ostukorv.Ridad.Select(r =>
                    new Osturida
                    {
                        ToodeId = r.Toode.Id,
                        TooteNimi = r.Toode.Toodenimetus,
                        Kogus = r.Kogus,
                        Hind = r.Toode.Hind,
                        Summa = r.Summa
                    }).ToList()
            };

            viimanePdf = LooPdf(eelvaade);

            MessageBox.Show(
                Keel.T(
                    $"PDF eelvaade on loodud:\n{viimanePdf}",
                    $"PDF preview created:\n{viimanePdf}"));
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                Keel.T(
                    "PDF-i loomine ebaõnnestus: ",
                    "Failed to create PDF: ") +
                ex.Message);
        }
    }

    // ---------- OSTUDE AJALUGU ----------
    private void btnOstudeAjalugu_Click(
        object? sender, EventArgs e)
    {
        using var ajalugu = new OstudeAjaluguForm();
        ajalugu.ShowDialog(this);
    }

    // ---------- E-POST ----------
    private void btnSaadaEmail_Click(
        object? sender, EventArgs e)
    {
        if (viimanePdf == null ||
            !File.Exists(viimanePdf))
        {
            MessageBox.Show(
                Keel.T(
                    "Kinnita ost või genereeri enne PDF.",
                    "Confirm the purchase or generate a PDF first."));
            return;
        }

        string saaja = txtEmail.Text.Trim();

        if (!MailAddress.TryCreate(saaja, out _))
        {
            MessageBox.Show(
                Keel.T(
                    "Sisesta korrektne e-posti aadress.",
                    "Enter a valid email address."));
            return;
        }

        string saatja =
            Environment.GetEnvironmentVariable(
                "TOODED_EMAIL") ?? "";

        string rakenduseParool =
            Environment.GetEnvironmentVariable(
                "TOODED_APP_PASSWORD") ?? "";

        if (string.IsNullOrWhiteSpace(saatja) ||
            string.IsNullOrWhiteSpace(rakenduseParool))
        {
            MessageBox.Show(
                Keel.T(
                    "E-posti saatmine pole seadistatud.",
                    "Email sending is not configured."));
            return;
        }

        try
        {
            using var kiri = new MailMessage(
                saatja, saaja,
                Keel.T("Arve", "Invoice"),
                Keel.T(
                    "Arve on manuses.",
                    "The invoice is attached."));

            kiri.Attachments.Add(
                new Attachment(viimanePdf));

            using var smtp = new SmtpClient(
                "smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    saatja, rakenduseParool),
                EnableSsl = true
            };

            smtp.Send(kiri);

            MessageBox.Show(
                Keel.T("Arve saadetud!", "Invoice sent!"));

            txtEmail.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                Keel.T("Viga: ", "Error: ") + ex.Message);
        }
    }

    private void ArveForm_Load(
        object sender, EventArgs e)
    {
    }
}

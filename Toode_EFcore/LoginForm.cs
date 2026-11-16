
namespace Tooded_EFCore;

public partial class LoginForm : Form
{
    private readonly TextBox txtEmail = new();
    private readonly TextBox txtParool = new();
    private readonly Button btnLogin = new();
    private readonly Label lblVeateade = new();

    public LoginForm()
    {
        Text = "Sisselogimine";
        ClientSize = new Size(300, 210);
        StartPosition = FormStartPosition.CenterScreen;

        txtEmail.SetBounds(30, 30, 240, 27);
        txtEmail.PlaceholderText = "E-post";

        txtParool.SetBounds(30, 70, 240, 27);
        txtParool.PlaceholderText = "Parool";
        txtParool.UseSystemPasswordChar = true;

        btnLogin.SetBounds(30, 110, 240, 35);
        btnLogin.Text = "Logi sisse";
        btnLogin.Click += btnLogin_Click;

        lblVeateade.SetBounds(30, 155, 240, 40);
        lblVeateade.ForeColor = Color.Red;

        AcceptButton = btnLogin;

        Controls.AddRange(new Control[]
        {
            txtEmail, txtParool, btnLogin, lblVeateade
        });
    }

    private void btnLogin_Click(object? sender, EventArgs e)
    {
        string email = txtEmail.Text.Trim().ToLowerInvariant();

        using var db = new ToodedContext();

        var kasutaja = db.Kasutajad
            .FirstOrDefault(k => k.Email == email);

        if (kasutaja == null ||
            !PasswordHelper.Verify(
                txtParool.Text, kasutaja.ParoolHash))
        {
            lblVeateade.Text = "Vale e-post või parool.";
            txtParool.Clear();
            return;
        }

        AktiivneKasutaja.Id = kasutaja.Id;
        AktiivneKasutaja.Nimi = kasutaja.Nimi;
        AktiivneKasutaja.Email = kasutaja.Email;
        AktiivneKasutaja.Roll = kasutaja.Roll;

        txtParool.Clear();
        lblVeateade.Text = "";

        var menyy = new FormMain();

        menyy.FormClosed += (s, a) =>
        {
            AktiivneKasutaja.LogiValja();
            txtEmail.Clear();
            txtParool.Clear();
            Show();
        };

        menyy.Show();
        Hide();
    }

    private void LoginForm_Load(object sender, EventArgs e)
    {
    }
}

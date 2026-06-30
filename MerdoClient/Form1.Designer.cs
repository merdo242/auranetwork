namespace MerdoClient;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    
    // Login Panel
    private System.Windows.Forms.Panel pnlLogin;
    private System.Windows.Forms.Panel pnlLeftNewsCard;
    private System.Windows.Forms.Label lblNewsTitle;
    private System.Windows.Forms.Panel pnlNewsLine;
    private System.Windows.Forms.Label lblNewsSubtitle;
    private System.Windows.Forms.Label lblNewsText;
    private System.Windows.Forms.Button btnNewsPrev;
    private System.Windows.Forms.Button btnNewsNext;
    
    private System.Windows.Forms.Panel pnlRightLoginCard;
    private System.Windows.Forms.Label lblLoginTitle;
    private System.Windows.Forms.Panel pnlUserContainer;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Panel pnlPassContainer;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.CheckBox chkRemember;
    private System.Windows.Forms.Label lblSavedAccountsTitle;
    private System.Windows.Forms.Panel pnlSavedAccounts;
    private System.Windows.Forms.Label lblSavedAccountsPlaceholder;
    private System.Windows.Forms.Label lblForgotPassword;
    private System.Windows.Forms.Label lblRegister;
    
    // Bottom Bar (inside pnlLogin)
    private System.Windows.Forms.Button btnShop;
    private System.Windows.Forms.Button btnWeb;
    private System.Windows.Forms.Button btnDiscord;
    private System.Windows.Forms.Button btnSettings;
    private System.Windows.Forms.Button btnWebsiteLink;

    // Home Panel
    private System.Windows.Forms.Panel pnlHome;
    private System.Windows.Forms.Panel pnlHomeLeftCard;
    private System.Windows.Forms.Panel pnlOnlinePlayers;
    private System.Windows.Forms.Label lblHomeLeftTitle;
    private System.Windows.Forms.Panel pnlHomeLeftLine;
    private System.Windows.Forms.Label lblHomeLeftText;
    
    private System.Windows.Forms.Panel pnlHomeRightCard;
    private System.Windows.Forms.Panel pnlAvatar;
    private System.Windows.Forms.Label lblWelcome;
    private System.Windows.Forms.Panel pnlRoleBadge;
    private System.Windows.Forms.Label lblRole;
    private System.Windows.Forms.Button btnPlay;
    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.Label lblConnectionsTitle;
    private System.Windows.Forms.Button btnRules;
    private System.Windows.Forms.Button btnWebsite;
    private System.Windows.Forms.Button btnDiscordLink;
    
    // Global Status Controls (inside pnlHome)
    private System.Windows.Forms.ProgressBar pbLaunch;
    private System.Windows.Forms.Label lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.pnlLogin = new System.Windows.Forms.Panel();
        this.pnlLeftNewsCard = new System.Windows.Forms.Panel();
        this.lblNewsTitle = new System.Windows.Forms.Label();
        this.pnlNewsLine = new System.Windows.Forms.Panel();
        this.lblNewsSubtitle = new System.Windows.Forms.Label();
        this.lblNewsText = new System.Windows.Forms.Label();
        this.btnNewsPrev = new System.Windows.Forms.Button();
        this.btnNewsNext = new System.Windows.Forms.Button();
        
        this.pnlRightLoginCard = new System.Windows.Forms.Panel();
        this.lblLoginTitle = new System.Windows.Forms.Label();
        this.pnlUserContainer = new System.Windows.Forms.Panel();
        this.txtUsername = new System.Windows.Forms.TextBox();
        this.pnlPassContainer = new System.Windows.Forms.Panel();
        this.txtPassword = new System.Windows.Forms.TextBox();
        this.btnLogin = new System.Windows.Forms.Button();
        this.chkRemember = new System.Windows.Forms.CheckBox();
        this.lblSavedAccountsTitle = new System.Windows.Forms.Label();
        this.pnlSavedAccounts = new System.Windows.Forms.Panel();
        this.lblSavedAccountsPlaceholder = new System.Windows.Forms.Label();
        this.lblForgotPassword = new System.Windows.Forms.Label();
        this.lblRegister = new System.Windows.Forms.Label();
        
        this.btnShop = new System.Windows.Forms.Button();
        this.btnWeb = new System.Windows.Forms.Button();
        this.btnDiscord = new System.Windows.Forms.Button();
        this.btnSettings = new System.Windows.Forms.Button();
        this.btnWebsiteLink = new System.Windows.Forms.Button();

        this.pnlHome = new System.Windows.Forms.Panel();
        this.pnlHomeLeftCard = new System.Windows.Forms.Panel();
        this.pnlOnlinePlayers = new System.Windows.Forms.Panel();
        this.lblHomeLeftTitle = new System.Windows.Forms.Label();
        this.pnlHomeLeftLine = new System.Windows.Forms.Panel();
        this.lblHomeLeftText = new System.Windows.Forms.Label();
        
        this.pnlHomeRightCard = new System.Windows.Forms.Panel();
        this.pnlAvatar = new System.Windows.Forms.Panel();
        this.lblWelcome = new System.Windows.Forms.Label();
        this.pnlRoleBadge = new System.Windows.Forms.Panel();
        this.lblRole = new System.Windows.Forms.Label();
        this.btnPlay = new System.Windows.Forms.Button();
        this.btnExit = new System.Windows.Forms.Button();
        this.lblConnectionsTitle = new System.Windows.Forms.Label();
        this.btnRules = new System.Windows.Forms.Button();
        this.btnWebsite = new System.Windows.Forms.Button();
        this.btnDiscordLink = new System.Windows.Forms.Button();
        
        this.pbLaunch = new System.Windows.Forms.ProgressBar();
        this.lblStatus = new System.Windows.Forms.Label();

        this.pnlLogin.SuspendLayout();
        this.pnlLeftNewsCard.SuspendLayout();
        this.pnlRightLoginCard.SuspendLayout();
        this.pnlUserContainer.SuspendLayout();
        this.pnlPassContainer.SuspendLayout();
        this.pnlSavedAccounts.SuspendLayout();
        this.pnlHome.SuspendLayout();
        this.pnlHomeLeftCard.SuspendLayout();
        this.pnlHomeRightCard.SuspendLayout();
        this.pnlRoleBadge.SuspendLayout();
        this.SuspendLayout();

        // 
        // Form1
        // 
        this.ClientSize = new System.Drawing.Size(1200, 700);
        this.Controls.Add(this.pnlLogin);
        this.Controls.Add(this.pnlHome);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Merdo Launcher - Giriş Yap";
        this.BackColor = System.Drawing.Color.FromArgb(8, 8, 10);

        // 
        // pnlLogin
        // 
        this.pnlLogin.BackColor = System.Drawing.Color.FromArgb(8, 8, 10);
        this.pnlLogin.Controls.Add(this.pnlLeftNewsCard);
        this.pnlLogin.Controls.Add(this.pnlRightLoginCard);
        this.pnlLogin.Controls.Add(this.btnShop);
        this.pnlLogin.Controls.Add(this.btnWeb);
        this.pnlLogin.Controls.Add(this.btnDiscord);
        this.pnlLogin.Controls.Add(this.btnSettings);
        this.pnlLogin.Controls.Add(this.btnWebsiteLink);
        this.pnlLogin.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlLogin.Location = new System.Drawing.Point(0, 0);
        this.pnlLogin.Name = "pnlLogin";
        this.pnlLogin.Size = new System.Drawing.Size(1200, 700);

        // 
        // pnlLeftNewsCard
        // 
        this.pnlLeftNewsCard.BackColor = System.Drawing.Color.Transparent;
        this.pnlLeftNewsCard.Controls.Add(this.lblNewsTitle);
        this.pnlLeftNewsCard.Controls.Add(this.pnlNewsLine);
        this.pnlLeftNewsCard.Controls.Add(this.lblNewsSubtitle);
        this.pnlLeftNewsCard.Controls.Add(this.lblNewsText);
        this.pnlLeftNewsCard.Controls.Add(this.btnNewsPrev);
        this.pnlLeftNewsCard.Controls.Add(this.btnNewsNext);
        this.pnlLeftNewsCard.Location = new System.Drawing.Point(50, 120);
        this.pnlLeftNewsCard.Name = "pnlLeftNewsCard";
        this.pnlLeftNewsCard.Size = new System.Drawing.Size(550, 400);

        // 
        // lblNewsTitle
        // 
        this.lblNewsTitle.AutoSize = true;
        this.lblNewsTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblNewsTitle.ForeColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.lblNewsTitle.Location = new System.Drawing.Point(30, 30);
        this.lblNewsTitle.Name = "lblNewsTitle";
        this.lblNewsTitle.Size = new System.Drawing.Size(201, 37);
        this.lblNewsTitle.Text = "TURNUVALAR";

        // 
        // pnlNewsLine
        // 
        this.pnlNewsLine.BackColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.pnlNewsLine.Location = new System.Drawing.Point(30, 72);
        this.pnlNewsLine.Name = "pnlNewsLine";
        this.pnlNewsLine.Size = new System.Drawing.Size(490, 3);

        // 
        // lblNewsSubtitle
        // 
        this.lblNewsSubtitle.AutoSize = true;
        this.lblNewsSubtitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblNewsSubtitle.ForeColor = System.Drawing.Color.White;
        this.lblNewsSubtitle.Location = new System.Drawing.Point(30, 85);
        this.lblNewsSubtitle.Name = "lblNewsSubtitle";
        this.lblNewsSubtitle.Size = new System.Drawing.Size(186, 21);
        this.lblNewsSubtitle.Text = "KAZANMAYA HAZIR OL";

        // 
        // lblNewsText
        // 
        this.lblNewsText.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblNewsText.ForeColor = System.Drawing.Color.FromArgb(170, 170, 180);
        this.lblNewsText.Location = new System.Drawing.Point(30, 120);
        this.lblNewsText.Name = "lblNewsText";
        this.lblNewsText.Size = new System.Drawing.Size(490, 180);
        this.lblNewsText.Text = "Haftalık turnuvalara katıl, rakiplerinle yarış ve büyük ödüllerin sahibi ol!";

        // 
        // btnNewsPrev
        // 
        this.btnNewsPrev.BackColor = System.Drawing.Color.Transparent;
        this.btnNewsPrev.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnNewsPrev.FlatAppearance.BorderSize = 1;
        this.btnNewsPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnNewsPrev.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnNewsPrev.ForeColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnNewsPrev.Location = new System.Drawing.Point(30, 320);
        this.btnNewsPrev.Name = "btnNewsPrev";
        this.btnNewsPrev.Size = new System.Drawing.Size(44, 44);
        this.btnNewsPrev.Text = "◀";
        this.btnNewsPrev.UseVisualStyleBackColor = false;

        // 
        // btnNewsNext
        // 
        this.btnNewsNext.BackColor = System.Drawing.Color.Transparent;
        this.btnNewsNext.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnNewsNext.FlatAppearance.BorderSize = 1;
        this.btnNewsNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnNewsNext.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnNewsNext.ForeColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnNewsNext.Location = new System.Drawing.Point(78, 320);
        this.btnNewsNext.Name = "btnNewsNext";
        this.btnNewsNext.Size = new System.Drawing.Size(44, 44);
        this.btnNewsNext.Text = "▶";
        this.btnNewsNext.UseVisualStyleBackColor = false;

        // 
        // pnlRightLoginCard
        // 
        this.pnlRightLoginCard.BackColor = System.Drawing.Color.Transparent;
        this.pnlRightLoginCard.Controls.Add(this.lblLoginTitle);
        this.pnlRightLoginCard.Controls.Add(this.pnlUserContainer);
        this.pnlRightLoginCard.Controls.Add(this.pnlPassContainer);
        this.pnlRightLoginCard.Controls.Add(this.btnLogin);
        this.pnlRightLoginCard.Controls.Add(this.chkRemember);
        this.pnlRightLoginCard.Controls.Add(this.lblSavedAccountsTitle);
        this.pnlRightLoginCard.Controls.Add(this.pnlSavedAccounts);
        this.pnlRightLoginCard.Controls.Add(this.lblForgotPassword);
        this.pnlRightLoginCard.Controls.Add(this.lblRegister);
        this.pnlRightLoginCard.Location = new System.Drawing.Point(650, 85);
        this.pnlRightLoginCard.Name = "pnlRightLoginCard";
        this.pnlRightLoginCard.Size = new System.Drawing.Size(420, 520);

        // 
        // lblLoginTitle
        // 
        this.lblLoginTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblLoginTitle.ForeColor = System.Drawing.Color.White;
        this.lblLoginTitle.Location = new System.Drawing.Point(0, 35);
        this.lblLoginTitle.Name = "lblLoginTitle";
        this.lblLoginTitle.Size = new System.Drawing.Size(420, 48);
        this.lblLoginTitle.Text = "GİRİŞ";
        this.lblLoginTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // 
        // pnlUserContainer
        // 
        this.pnlUserContainer.BackColor = System.Drawing.Color.Transparent;
        this.pnlUserContainer.Controls.Add(this.txtUsername);
        this.pnlUserContainer.Location = new System.Drawing.Point(45, 108);
        this.pnlUserContainer.Name = "pnlUserContainer";
        this.pnlUserContainer.Size = new System.Drawing.Size(330, 48);

        // 
        // txtUsername
        // 
        this.txtUsername.BackColor = System.Drawing.Color.FromArgb(13, 13, 15);
        this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtUsername.ForeColor = System.Drawing.Color.White;
        this.txtUsername.Location = new System.Drawing.Point(15, 14);
        this.txtUsername.Name = "txtUsername";
        this.txtUsername.Size = new System.Drawing.Size(300, 18);

        // 
        // pnlPassContainer
        // 
        this.pnlPassContainer.BackColor = System.Drawing.Color.Transparent;
        this.pnlPassContainer.Controls.Add(this.txtPassword);
        this.pnlPassContainer.Location = new System.Drawing.Point(45, 175);
        this.pnlPassContainer.Name = "pnlPassContainer";
        this.pnlPassContainer.Size = new System.Drawing.Size(330, 48);

        // 
        // txtPassword
        // 
        this.txtPassword.BackColor = System.Drawing.Color.FromArgb(13, 13, 15);
        this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtPassword.ForeColor = System.Drawing.Color.White;
        this.txtPassword.Location = new System.Drawing.Point(15, 14);
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.Size = new System.Drawing.Size(300, 18);
        this.txtPassword.UseSystemPasswordChar = true;

        // 
        // btnLogin
        // 
        this.btnLogin.BackColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnLogin.FlatAppearance.BorderSize = 0;
        this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnLogin.ForeColor = System.Drawing.Color.Black;
        this.btnLogin.Location = new System.Drawing.Point(45, 248);
        this.btnLogin.Name = "btnLogin";
        this.btnLogin.Size = new System.Drawing.Size(330, 48);
        this.btnLogin.Text = "Giriş Yap";
        this.btnLogin.UseVisualStyleBackColor = false;
        this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

        // 
        // chkRemember
        // 
        this.chkRemember.AutoSize = true;
        this.chkRemember.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.chkRemember.ForeColor = System.Drawing.Color.FromArgb(170, 170, 180);
        this.chkRemember.Location = new System.Drawing.Point(45, 308);
        this.chkRemember.Name = "chkRemember";
        this.chkRemember.Size = new System.Drawing.Size(121, 19);
        this.chkRemember.Text = "🔒 Hesabı kaydet";
        this.chkRemember.UseVisualStyleBackColor = true;

        // 
        // lblSavedAccountsTitle
        // 
        this.lblSavedAccountsTitle.AutoSize = true;
        this.lblSavedAccountsTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblSavedAccountsTitle.ForeColor = System.Drawing.Color.White;
        this.lblSavedAccountsTitle.Location = new System.Drawing.Point(45, 342);
        this.lblSavedAccountsTitle.Name = "lblSavedAccountsTitle";
        this.lblSavedAccountsTitle.Size = new System.Drawing.Size(106, 15);
        this.lblSavedAccountsTitle.Text = "👥 Kayıtlı Hesaplar";

        // 
        // pnlSavedAccounts
        // 
        this.pnlSavedAccounts.BackColor = System.Drawing.Color.Transparent;
        this.pnlSavedAccounts.Controls.Add(this.lblSavedAccountsPlaceholder);
        this.pnlSavedAccounts.Location = new System.Drawing.Point(45, 368);
        this.pnlSavedAccounts.Name = "pnlSavedAccounts";
        this.pnlSavedAccounts.Size = new System.Drawing.Size(330, 50);

        // 
        // lblSavedAccountsPlaceholder
        // 
        this.lblSavedAccountsPlaceholder.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblSavedAccountsPlaceholder.ForeColor = System.Drawing.Color.FromArgb(100, 100, 110);
        this.lblSavedAccountsPlaceholder.Location = new System.Drawing.Point(0, 15);
        this.lblSavedAccountsPlaceholder.Name = "lblSavedAccountsPlaceholder";
        this.lblSavedAccountsPlaceholder.Size = new System.Drawing.Size(330, 20);
        this.lblSavedAccountsPlaceholder.Text = "Henüz kayıtlı hesap bulunmuyor";
        this.lblSavedAccountsPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // 
        // lblForgotPassword
        // 
        this.lblForgotPassword.Cursor = System.Windows.Forms.Cursors.Hand;
        this.lblForgotPassword.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
        this.lblForgotPassword.ForeColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.lblForgotPassword.Location = new System.Drawing.Point(0, 445);
        this.lblForgotPassword.Name = "lblForgotPassword";
        this.lblForgotPassword.Size = new System.Drawing.Size(420, 18);
        this.lblForgotPassword.Text = "Şifremi mi unuttun?";
        this.lblForgotPassword.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // 
        // lblRegister
        // 
        this.lblRegister.Cursor = System.Windows.Forms.Cursors.Hand;
        this.lblRegister.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
        this.lblRegister.ForeColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.lblRegister.Location = new System.Drawing.Point(0, 470);
        this.lblRegister.Name = "lblRegister";
        this.lblRegister.Size = new System.Drawing.Size(420, 18);
        this.lblRegister.Text = "Yeni hesap oluştur";
        this.lblRegister.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.lblRegister.Click += new System.EventHandler(this.lblRegister_Click);

        // 
        // btnShop
        // 
        this.btnShop.BackColor = System.Drawing.Color.FromArgb(21, 21, 24);
        this.btnShop.FlatAppearance.BorderSize = 0;
        this.btnShop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnShop.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnShop.ForeColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnShop.Location = new System.Drawing.Point(50, 605);
        this.btnShop.Name = "btnShop";
        this.btnShop.Size = new System.Drawing.Size(36, 36);
        this.btnShop.Text = "🛒";
        this.btnShop.UseVisualStyleBackColor = false;

        // 
        // btnWeb
        // 
        this.btnWeb.BackColor = System.Drawing.Color.FromArgb(21, 21, 24);
        this.btnWeb.FlatAppearance.BorderSize = 0;
        this.btnWeb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnWeb.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnWeb.ForeColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnWeb.Location = new System.Drawing.Point(92, 605);
        this.btnWeb.Name = "btnWeb";
        this.btnWeb.Size = new System.Drawing.Size(36, 36);
        this.btnWeb.Text = "🌐";
        this.btnWeb.UseVisualStyleBackColor = false;

        // 
        // btnDiscord
        // 
        this.btnDiscord.BackColor = System.Drawing.Color.FromArgb(21, 21, 24);
        this.btnDiscord.FlatAppearance.BorderSize = 0;
        this.btnDiscord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnDiscord.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnDiscord.ForeColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnDiscord.Location = new System.Drawing.Point(134, 605);
        this.btnDiscord.Name = "btnDiscord";
        this.btnDiscord.Size = new System.Drawing.Size(36, 36);
        this.btnDiscord.Text = "💬";
        this.btnDiscord.UseVisualStyleBackColor = false;

        // 
        // btnSettings
        // 
        this.btnSettings.BackColor = System.Drawing.Color.FromArgb(21, 21, 24);
        this.btnSettings.FlatAppearance.BorderSize = 0;
        this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnSettings.ForeColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnSettings.Location = new System.Drawing.Point(176, 605);
        this.btnSettings.Name = "btnSettings";
        this.btnSettings.Size = new System.Drawing.Size(36, 36);
        this.btnSettings.Text = "⚙️";
        this.btnSettings.UseVisualStyleBackColor = false;

        // 
        // btnWebsiteLink
        // 
        this.btnWebsiteLink.BackColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnWebsiteLink.FlatAppearance.BorderSize = 0;
        this.btnWebsiteLink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnWebsiteLink.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnWebsiteLink.ForeColor = System.Drawing.Color.Black;
        this.btnWebsiteLink.Location = new System.Drawing.Point(1060, 648);
        this.btnWebsiteLink.Name = "btnWebsiteLink";
        this.btnWebsiteLink.Size = new System.Drawing.Size(120, 34);
        this.btnWebsiteLink.Text = "chickennw.com";
        this.btnWebsiteLink.UseVisualStyleBackColor = false;

        // 
        // pnlHome
        // 
        this.pnlHome.BackColor = System.Drawing.Color.FromArgb(8, 8, 10);
        this.pnlHome.Controls.Add(this.pnlHomeLeftCard);
        this.pnlHome.Controls.Add(this.pnlHomeRightCard);
        this.pnlHome.Controls.Add(this.pbLaunch);
        this.pnlHome.Controls.Add(this.lblStatus);
        this.pnlHome.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlHome.Location = new System.Drawing.Point(0, 0);
        this.pnlHome.Name = "pnlHome";
        this.pnlHome.Size = new System.Drawing.Size(1200, 700);
        this.pnlHome.Visible = false;

        // 
        // pnlHomeLeftCard
        // 
        this.pnlHomeLeftCard.BackColor = System.Drawing.Color.Transparent;
        this.pnlHomeLeftCard.Controls.Add(this.pnlOnlinePlayers);
        this.pnlHomeLeftCard.Controls.Add(this.lblHomeLeftTitle);
        this.pnlHomeLeftCard.Controls.Add(this.pnlHomeLeftLine);
        this.pnlHomeLeftCard.Controls.Add(this.lblHomeLeftText);
        this.pnlHomeLeftCard.Location = new System.Drawing.Point(50, 120);
        this.pnlHomeLeftCard.Name = "pnlHomeLeftCard";
        this.pnlHomeLeftCard.Size = new System.Drawing.Size(550, 400);

        // 
        // pnlOnlinePlayers
        // 
        this.pnlOnlinePlayers.BackColor = System.Drawing.Color.Transparent;
        this.pnlOnlinePlayers.Location = new System.Drawing.Point(30, 30);
        this.pnlOnlinePlayers.Name = "pnlOnlinePlayers";
        this.pnlOnlinePlayers.Size = new System.Drawing.Size(490, 34);

        // 
        // lblHomeLeftTitle
        // 
        this.lblHomeLeftTitle.AutoSize = true;
        this.lblHomeLeftTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblHomeLeftTitle.ForeColor = System.Drawing.Color.White;
        this.lblHomeLeftTitle.Location = new System.Drawing.Point(30, 85);
        this.lblHomeLeftTitle.Name = "lblHomeLeftTitle";
        this.lblHomeLeftTitle.Size = new System.Drawing.Size(325, 45);
        this.lblHomeLeftTitle.Text = "CHICKEN NETWORK";

        // 
        // pnlHomeLeftLine
        // 
        this.pnlHomeLeftLine.BackColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.pnlHomeLeftLine.Location = new System.Drawing.Point(30, 136);
        this.pnlHomeLeftLine.Name = "pnlHomeLeftLine";
        this.pnlHomeLeftLine.Size = new System.Drawing.Size(490, 3);

        // 
        // lblHomeLeftText
        // 
        this.lblHomeLeftText.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblHomeLeftText.ForeColor = System.Drawing.Color.FromArgb(170, 170, 180);
        this.lblHomeLeftText.Location = new System.Drawing.Point(30, 155);
        this.lblHomeLeftText.Name = "lblHomeLeftText";
        this.lblHomeLeftText.Size = new System.Drawing.Size(490, 220);
        this.lblHomeLeftText.Text = "Chicken Network sunucusunda Skyblock, Survival, OP Skyblock dahil bir sürü eğlenceli oyun modunda oynayabilir ve eğlenebilirsin. Arkadaşlarınla klan/ada kurabilir ve diğer oyuncularla savaşarak ganimet elde edebilirsin.\r\n\r\nYaşadığın tüm sorunlar için site desteğinden bizimle iletişime geçmekten lütfen çekinme. Ayrıca sunucu oylama ve çekilişlerine katılmak için Discord sunucumuza katılabilirsin. Discord sunucusuna katılan oyuncular 1 haftalık VIP üyelik kazanır.";

        // 
        // pnlHomeRightCard
        // 
        this.pnlHomeRightCard.BackColor = System.Drawing.Color.Transparent;
        this.pnlHomeRightCard.Controls.Add(this.pnlAvatar);
        this.pnlHomeRightCard.Controls.Add(this.lblWelcome);
        this.pnlHomeRightCard.Controls.Add(this.pnlRoleBadge);
        this.pnlHomeRightCard.Controls.Add(this.btnPlay);
        this.pnlHomeRightCard.Controls.Add(this.btnExit);
        this.pnlHomeRightCard.Controls.Add(this.lblConnectionsTitle);
        this.pnlHomeRightCard.Controls.Add(this.btnRules);
        this.pnlHomeRightCard.Controls.Add(this.btnWebsite);
        this.pnlHomeRightCard.Controls.Add(this.btnDiscordLink);
        this.pnlHomeRightCard.Location = new System.Drawing.Point(630, 120);
        this.pnlHomeRightCard.Name = "pnlHomeRightCard";
        this.pnlHomeRightCard.Size = new System.Drawing.Size(320, 400);

        // 
        // pnlAvatar
        // 
        this.pnlAvatar.BackColor = System.Drawing.Color.Transparent;
        this.pnlAvatar.Location = new System.Drawing.Point(115, 30);
        this.pnlAvatar.Name = "pnlAvatar";
        this.pnlAvatar.Size = new System.Drawing.Size(90, 90);

        // 
        // lblWelcome
        // 
        this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblWelcome.ForeColor = System.Drawing.Color.White;
        this.lblWelcome.Location = new System.Drawing.Point(0, 135);
        this.lblWelcome.Name = "lblWelcome";
        this.lblWelcome.Size = new System.Drawing.Size(320, 30);
        this.lblWelcome.Text = "MERDO242";
        this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // 
        // pnlRoleBadge
        // 
        this.pnlRoleBadge.BackColor = System.Drawing.Color.Transparent;
        this.pnlRoleBadge.Controls.Add(this.lblRole);
        this.pnlRoleBadge.Location = new System.Drawing.Point(120, 175);
        this.pnlRoleBadge.Name = "pnlRoleBadge";
        this.pnlRoleBadge.Size = new System.Drawing.Size(80, 20);

        // 
        // lblRole
        // 
        this.lblRole.BackColor = System.Drawing.Color.Transparent;
        this.lblRole.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblRole.ForeColor = System.Drawing.Color.Black;
        this.lblRole.Location = new System.Drawing.Point(0, 0);
        this.lblRole.Name = "lblRole";
        this.lblRole.Size = new System.Drawing.Size(80, 20);
        this.lblRole.Text = "OYUNCU";
        this.lblRole.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // 
        // btnPlay
        // 
        this.btnPlay.BackColor = System.Drawing.Color.FromArgb(255, 204, 0);
        this.btnPlay.FlatAppearance.BorderSize = 0;
        this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnPlay.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnPlay.ForeColor = System.Drawing.Color.Black;
        this.btnPlay.Location = new System.Drawing.Point(30, 215);
        this.btnPlay.Name = "btnPlay";
        this.btnPlay.Size = new System.Drawing.Size(260, 38);
        this.btnPlay.Text = "➔ OYNA";
        this.btnPlay.UseVisualStyleBackColor = false;
        this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);

        // 
        // btnExit
        // 
        this.btnExit.BackColor = System.Drawing.Color.Transparent;
        this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 50, 50);
        this.btnExit.FlatAppearance.BorderSize = 1;
        this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnExit.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnExit.ForeColor = System.Drawing.Color.FromArgb(220, 50, 50);
        this.btnExit.Location = new System.Drawing.Point(30, 265);
        this.btnExit.Name = "btnExit";
        this.btnExit.Size = new System.Drawing.Size(260, 34);
        this.btnExit.Text = "↺ ÇIKIŞ YAP";
        this.btnExit.UseVisualStyleBackColor = false;
        this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

        // 
        // lblConnectionsTitle
        // 
        this.lblConnectionsTitle.AutoSize = true;
        this.lblConnectionsTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblConnectionsTitle.ForeColor = System.Drawing.Color.White;
        this.lblConnectionsTitle.Location = new System.Drawing.Point(30, 318);
        this.lblConnectionsTitle.Name = "lblConnectionsTitle";
        this.lblConnectionsTitle.Size = new System.Drawing.Size(95, 15);
        this.lblConnectionsTitle.Text = "🔗 BAĞLANTILAR";

        // 
        // btnRules
        // 
        this.btnRules.BackColor = System.Drawing.Color.FromArgb(35, 35, 40);
        this.btnRules.FlatAppearance.BorderSize = 0;
        this.btnRules.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnRules.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnRules.ForeColor = System.Drawing.Color.White;
        this.btnRules.Location = new System.Drawing.Point(30, 345);
        this.btnRules.Name = "btnRules";
        this.btnRules.Size = new System.Drawing.Size(80, 32);
        this.btnRules.Text = "KURALLAR";
        this.btnRules.UseVisualStyleBackColor = false;

        // 
        // btnWebsite
        // 
        this.btnWebsite.BackColor = System.Drawing.Color.FromArgb(35, 35, 40);
        this.btnWebsite.FlatAppearance.BorderSize = 0;
        this.btnWebsite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnWebsite.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnWebsite.ForeColor = System.Drawing.Color.White;
        this.btnWebsite.Location = new System.Drawing.Point(120, 345);
        this.btnWebsite.Name = "btnWebsite";
        this.btnWebsite.Size = new System.Drawing.Size(80, 32);
        this.btnWebsite.Text = "SİTE";
        this.btnWebsite.UseVisualStyleBackColor = false;

        // 
        // btnDiscordLink
        // 
        this.btnDiscordLink.BackColor = System.Drawing.Color.FromArgb(35, 35, 40);
        this.btnDiscordLink.FlatAppearance.BorderSize = 0;
        this.btnDiscordLink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnDiscordLink.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnDiscordLink.ForeColor = System.Drawing.Color.White;
        this.btnDiscordLink.Location = new System.Drawing.Point(210, 345);
        this.btnDiscordLink.Name = "btnDiscordLink";
        this.btnDiscordLink.Size = new System.Drawing.Size(80, 32);
        this.btnDiscordLink.Text = "DISCORD";
        this.btnDiscordLink.UseVisualStyleBackColor = false;

        // 
        // pbLaunch
        // 
        this.pbLaunch.Location = new System.Drawing.Point(50, 542);
        this.pbLaunch.Name = "pbLaunch";
        this.pbLaunch.Size = new System.Drawing.Size(900, 6);
        this.pbLaunch.Value = 0;

        // 
        // lblStatus
        // 
        this.lblStatus.AutoSize = true;
        this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(170, 170, 180);
        this.lblStatus.Location = new System.Drawing.Point(50, 555);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(34, 15);
        this.lblStatus.Text = "Hazır";

        this.pnlLogin.ResumeLayout(false);
        this.pnlLogin.PerformLayout();
        this.pnlLeftNewsCard.ResumeLayout(false);
        this.pnlLeftNewsCard.PerformLayout();
        this.pnlRightLoginCard.ResumeLayout(false);
        this.pnlUserContainer.ResumeLayout(false);
        this.pnlUserContainer.PerformLayout();
        this.pnlPassContainer.ResumeLayout(false);
        this.pnlPassContainer.PerformLayout();
        this.pnlSavedAccounts.ResumeLayout(false);
        this.pnlHome.ResumeLayout(false);
        this.pnlHome.PerformLayout();
        this.pnlHomeLeftCard.ResumeLayout(false);
        this.pnlHomeLeftCard.PerformLayout();
        this.pnlHomeRightCard.ResumeLayout(false);
        this.pnlHomeRightCard.PerformLayout();
        this.pnlRoleBadge.ResumeLayout(false);
        this.ResumeLayout(false);
    }
}

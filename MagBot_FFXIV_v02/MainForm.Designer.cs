namespace MagBot_FFXIV_v02
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btExpFarming = new System.Windows.Forms.Button();
            this.hp = new System.Windows.Forms.Label();
            this.lbHP = new System.Windows.Forms.Label();
            this.lbMP = new System.Windows.Forms.Label();
            this.mp = new System.Windows.Forms.Label();
            this.lbTP = new System.Windows.Forms.Label();
            this.tp = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbMaxHP = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbMaxMP = new System.Windows.Forms.Label();
            this.lbMaxTP = new System.Windows.Forms.Label();
            this.lbZCoordinate = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbXCoordinate = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbYCoordinate = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lbFacing = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tab = new System.Windows.Forms.TabControl();
            this.IntroTab = new System.Windows.Forms.TabPage();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ExpFarmingTab = new System.Windows.Forms.TabPage();
            this.nudMinutes = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.nudHours = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.btStopExpFarming = new System.Windows.Forms.Button();
            this.btSaveRoutes = new System.Windows.Forms.Button();
            this.btLoadRoutes = new System.Windows.Forms.Button();
            this.cbEscapeRoutes = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btDelWaypoint = new System.Windows.Forms.Button();
            this.btNewRoute = new System.Windows.Forms.Button();
            this.btRecWaypoint = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.lvWaypoints = new System.Windows.Forms.ListView();
            this.columnRoute = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnWaypoint = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnX = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnY = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnZ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ChatLogTab = new System.Windows.Forms.TabPage();
            this.lvwChatLog = new System.Windows.Forms.ListView();
            this.chatlog = new System.Windows.Forms.Label();
            this.cbStandStill = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tab.SuspendLayout();
            this.IntroTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.ExpFarmingTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHours)).BeginInit();
            this.ChatLogTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // btExpFarming
            // 
            this.btExpFarming.Enabled = false;
            this.btExpFarming.Location = new System.Drawing.Point(16, 468);
            this.btExpFarming.Name = "btExpFarming";
            this.btExpFarming.Size = new System.Drawing.Size(100, 27);
            this.btExpFarming.TabIndex = 1;
            this.btExpFarming.Text = "Start";
            this.btExpFarming.UseVisualStyleBackColor = true;
            this.btExpFarming.Click += new System.EventHandler(this.btStartExpFarming_Click);
            // 
            // hp
            // 
            this.hp.AutoSize = true;
            this.hp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hp.Location = new System.Drawing.Point(14, 38);
            this.hp.Name = "hp";
            this.hp.Size = new System.Drawing.Size(28, 13);
            this.hp.TabIndex = 3;
            this.hp.Text = "HP:";
            // 
            // lbHP
            // 
            this.lbHP.AutoSize = true;
            this.lbHP.Location = new System.Drawing.Point(50, 38);
            this.lbHP.Name = "lbHP";
            this.lbHP.Size = new System.Drawing.Size(25, 13);
            this.lbHP.TabIndex = 4;
            this.lbHP.Text = "000";
            // 
            // lbMP
            // 
            this.lbMP.AutoSize = true;
            this.lbMP.Location = new System.Drawing.Point(50, 61);
            this.lbMP.Name = "lbMP";
            this.lbMP.Size = new System.Drawing.Size(25, 13);
            this.lbMP.TabIndex = 6;
            this.lbMP.Text = "000";
            // 
            // mp
            // 
            this.mp.AutoSize = true;
            this.mp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mp.Location = new System.Drawing.Point(14, 61);
            this.mp.Name = "mp";
            this.mp.Size = new System.Drawing.Size(29, 13);
            this.mp.TabIndex = 5;
            this.mp.Text = "MP:";
            // 
            // lbTP
            // 
            this.lbTP.AutoSize = true;
            this.lbTP.Location = new System.Drawing.Point(50, 83);
            this.lbTP.Name = "lbTP";
            this.lbTP.Size = new System.Drawing.Size(25, 13);
            this.lbTP.TabIndex = 8;
            this.lbTP.Text = "000";
            // 
            // tp
            // 
            this.tp.AutoSize = true;
            this.tp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tp.Location = new System.Drawing.Point(14, 83);
            this.tp.Name = "tp";
            this.tp.Size = new System.Drawing.Size(27, 13);
            this.tp.TabIndex = 7;
            this.tp.Text = "TP:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(77, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "/";
            // 
            // lbMaxHP
            // 
            this.lbMaxHP.AutoSize = true;
            this.lbMaxHP.Location = new System.Drawing.Point(93, 38);
            this.lbMaxHP.Name = "lbMaxHP";
            this.lbMaxHP.Size = new System.Drawing.Size(25, 13);
            this.lbMaxHP.TabIndex = 11;
            this.lbMaxHP.Text = "000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(77, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "/";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(77, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "/";
            // 
            // lbMaxMP
            // 
            this.lbMaxMP.AutoSize = true;
            this.lbMaxMP.Location = new System.Drawing.Point(93, 61);
            this.lbMaxMP.Name = "lbMaxMP";
            this.lbMaxMP.Size = new System.Drawing.Size(25, 13);
            this.lbMaxMP.TabIndex = 14;
            this.lbMaxMP.Text = "000";
            // 
            // lbMaxTP
            // 
            this.lbMaxTP.AutoSize = true;
            this.lbMaxTP.Location = new System.Drawing.Point(93, 83);
            this.lbMaxTP.Name = "lbMaxTP";
            this.lbMaxTP.Size = new System.Drawing.Size(25, 13);
            this.lbMaxTP.TabIndex = 15;
            this.lbMaxTP.Text = "000";
            // 
            // lbZCoordinate
            // 
            this.lbZCoordinate.Location = new System.Drawing.Point(287, 83);
            this.lbZCoordinate.Name = "lbZCoordinate";
            this.lbZCoordinate.Size = new System.Drawing.Size(40, 13);
            this.lbZCoordinate.TabIndex = 19;
            this.lbZCoordinate.Text = "000.00";
            this.lbZCoordinate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(214, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Z (Height):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Player Stats";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(214, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Player Coordinates";
            // 
            // lbXCoordinate
            // 
            this.lbXCoordinate.Location = new System.Drawing.Point(287, 38);
            this.lbXCoordinate.Name = "lbXCoordinate";
            this.lbXCoordinate.Size = new System.Drawing.Size(40, 13);
            this.lbXCoordinate.TabIndex = 23;
            this.lbXCoordinate.Text = "000.00";
            this.lbXCoordinate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(214, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "X:";
            // 
            // lbYCoordinate
            // 
            this.lbYCoordinate.Location = new System.Drawing.Point(287, 61);
            this.lbYCoordinate.Name = "lbYCoordinate";
            this.lbYCoordinate.Size = new System.Drawing.Size(40, 13);
            this.lbYCoordinate.TabIndex = 25;
            this.lbYCoordinate.Text = "000.00";
            this.lbYCoordinate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(214, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(19, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Y:";
            // 
            // lbFacing
            // 
            this.lbFacing.Location = new System.Drawing.Point(287, 106);
            this.lbFacing.Name = "lbFacing";
            this.lbFacing.Size = new System.Drawing.Size(40, 13);
            this.lbFacing.TabIndex = 27;
            this.lbFacing.Text = "000.00";
            this.lbFacing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(214, 106);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Facing:";
            // 
            // tab
            // 
            this.tab.Controls.Add(this.IntroTab);
            this.tab.Controls.Add(this.ExpFarmingTab);
            this.tab.Controls.Add(this.ChatLogTab);
            this.tab.Location = new System.Drawing.Point(0, 0);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(351, 530);
            this.tab.TabIndex = 29;
            this.tab.SelectedIndexChanged += new System.EventHandler(this.tab_SelectedIndexChanged);
            // 
            // IntroTab
            // 
            this.IntroTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.IntroTab.Controls.Add(this.pictureBox2);
            this.IntroTab.Controls.Add(this.pictureBox1);
            this.IntroTab.Location = new System.Drawing.Point(4, 22);
            this.IntroTab.Name = "IntroTab";
            this.IntroTab.Padding = new System.Windows.Forms.Padding(3);
            this.IntroTab.Size = new System.Drawing.Size(343, 504);
            this.IntroTab.TabIndex = 1;
            this.IntroTab.Text = "Welcome";
            this.IntroTab.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.ErrorImage = null;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(-1, 225);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(344, 279);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(20, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(327, 216);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // ExpFarmingTab
            // 
            this.ExpFarmingTab.Controls.Add(this.cbStandStill);
            this.ExpFarmingTab.Controls.Add(this.label15);
            this.ExpFarmingTab.Controls.Add(this.nudMinutes);
            this.ExpFarmingTab.Controls.Add(this.label14);
            this.ExpFarmingTab.Controls.Add(this.label13);
            this.ExpFarmingTab.Controls.Add(this.nudHours);
            this.ExpFarmingTab.Controls.Add(this.label12);
            this.ExpFarmingTab.Controls.Add(this.btStopExpFarming);
            this.ExpFarmingTab.Controls.Add(this.btSaveRoutes);
            this.ExpFarmingTab.Controls.Add(this.btLoadRoutes);
            this.ExpFarmingTab.Controls.Add(this.cbEscapeRoutes);
            this.ExpFarmingTab.Controls.Add(this.label6);
            this.ExpFarmingTab.Controls.Add(this.btDelWaypoint);
            this.ExpFarmingTab.Controls.Add(this.btNewRoute);
            this.ExpFarmingTab.Controls.Add(this.btRecWaypoint);
            this.ExpFarmingTab.Controls.Add(this.label11);
            this.ExpFarmingTab.Controls.Add(this.lvWaypoints);
            this.ExpFarmingTab.Controls.Add(this.btExpFarming);
            this.ExpFarmingTab.Controls.Add(this.lbFacing);
            this.ExpFarmingTab.Controls.Add(this.hp);
            this.ExpFarmingTab.Controls.Add(this.label9);
            this.ExpFarmingTab.Controls.Add(this.lbHP);
            this.ExpFarmingTab.Controls.Add(this.lbYCoordinate);
            this.ExpFarmingTab.Controls.Add(this.mp);
            this.ExpFarmingTab.Controls.Add(this.label10);
            this.ExpFarmingTab.Controls.Add(this.lbMP);
            this.ExpFarmingTab.Controls.Add(this.lbXCoordinate);
            this.ExpFarmingTab.Controls.Add(this.tp);
            this.ExpFarmingTab.Controls.Add(this.label8);
            this.ExpFarmingTab.Controls.Add(this.lbTP);
            this.ExpFarmingTab.Controls.Add(this.label5);
            this.ExpFarmingTab.Controls.Add(this.label1);
            this.ExpFarmingTab.Controls.Add(this.label2);
            this.ExpFarmingTab.Controls.Add(this.lbMaxHP);
            this.ExpFarmingTab.Controls.Add(this.lbZCoordinate);
            this.ExpFarmingTab.Controls.Add(this.label3);
            this.ExpFarmingTab.Controls.Add(this.label7);
            this.ExpFarmingTab.Controls.Add(this.label4);
            this.ExpFarmingTab.Controls.Add(this.lbMaxTP);
            this.ExpFarmingTab.Controls.Add(this.lbMaxMP);
            this.ExpFarmingTab.Location = new System.Drawing.Point(4, 22);
            this.ExpFarmingTab.Name = "ExpFarmingTab";
            this.ExpFarmingTab.Padding = new System.Windows.Forms.Padding(3);
            this.ExpFarmingTab.Size = new System.Drawing.Size(343, 504);
            this.ExpFarmingTab.TabIndex = 0;
            this.ExpFarmingTab.Text = "Exp. Farming";
            this.ExpFarmingTab.UseVisualStyleBackColor = true;
            // 
            // nudMinutes
            // 
            this.nudMinutes.Location = new System.Drawing.Point(293, 426);
            this.nudMinutes.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudMinutes.Name = "nudMinutes";
            this.nudMinutes.Size = new System.Drawing.Size(34, 20);
            this.nudMinutes.TabIndex = 44;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(231, 428);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(55, 13);
            this.label14.TabIndex = 43;
            this.label14.Text = "Minutes:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(243, 406);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 13);
            this.label13.TabIndex = 42;
            this.label13.Text = "Hours:";
            // 
            // nudHours
            // 
            this.nudHours.Location = new System.Drawing.Point(293, 404);
            this.nudHours.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.nudHours.Name = "nudHours";
            this.nudHours.Size = new System.Drawing.Size(34, 20);
            this.nudHours.TabIndex = 41;
            this.nudHours.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(216, 385);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(111, 13);
            this.label12.TabIndex = 39;
            this.label12.Text = "Run for how long?";
            // 
            // btStopExpFarming
            // 
            this.btStopExpFarming.Enabled = false;
            this.btStopExpFarming.Location = new System.Drawing.Point(227, 468);
            this.btStopExpFarming.Name = "btStopExpFarming";
            this.btStopExpFarming.Size = new System.Drawing.Size(100, 27);
            this.btStopExpFarming.TabIndex = 38;
            this.btStopExpFarming.Text = "Stop";
            this.btStopExpFarming.UseVisualStyleBackColor = true;
            this.btStopExpFarming.Click += new System.EventHandler(this.btStopExpFarming_Click);
            // 
            // btSaveRoutes
            // 
            this.btSaveRoutes.Enabled = false;
            this.btSaveRoutes.Location = new System.Drawing.Point(175, 342);
            this.btSaveRoutes.Name = "btSaveRoutes";
            this.btSaveRoutes.Size = new System.Drawing.Size(152, 27);
            this.btSaveRoutes.TabIndex = 37;
            this.btSaveRoutes.Text = "Save Route(s)";
            this.btSaveRoutes.UseVisualStyleBackColor = true;
            this.btSaveRoutes.Click += new System.EventHandler(this.btSaveRoutes_Click);
            // 
            // btLoadRoutes
            // 
            this.btLoadRoutes.Location = new System.Drawing.Point(16, 342);
            this.btLoadRoutes.Name = "btLoadRoutes";
            this.btLoadRoutes.Size = new System.Drawing.Size(152, 27);
            this.btLoadRoutes.TabIndex = 36;
            this.btLoadRoutes.Text = "Load Route(s)";
            this.btLoadRoutes.UseVisualStyleBackColor = true;
            this.btLoadRoutes.Click += new System.EventHandler(this.btLoadRoutes_Click);
            // 
            // cbEscapeRoutes
            // 
            this.cbEscapeRoutes.FormattingEnabled = true;
            this.cbEscapeRoutes.Location = new System.Drawing.Point(16, 403);
            this.cbEscapeRoutes.Name = "cbEscapeRoutes";
            this.cbEscapeRoutes.Size = new System.Drawing.Size(89, 21);
            this.cbEscapeRoutes.TabIndex = 35;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(14, 385);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "Escape Route:";
            // 
            // btDelWaypoint
            // 
            this.btDelWaypoint.Location = new System.Drawing.Point(227, 306);
            this.btDelWaypoint.Name = "btDelWaypoint";
            this.btDelWaypoint.Size = new System.Drawing.Size(100, 27);
            this.btDelWaypoint.TabIndex = 33;
            this.btDelWaypoint.Text = "Delete Waypoint";
            this.btDelWaypoint.UseVisualStyleBackColor = true;
            this.btDelWaypoint.Click += new System.EventHandler(this.btDelWaypoint_Click);
            // 
            // btNewRoute
            // 
            this.btNewRoute.Location = new System.Drawing.Point(16, 306);
            this.btNewRoute.Name = "btNewRoute";
            this.btNewRoute.Size = new System.Drawing.Size(100, 27);
            this.btNewRoute.TabIndex = 32;
            this.btNewRoute.Text = "New Route";
            this.btNewRoute.UseVisualStyleBackColor = true;
            this.btNewRoute.Click += new System.EventHandler(this.btNewRoute_Click);
            // 
            // btRecWaypoint
            // 
            this.btRecWaypoint.Enabled = false;
            this.btRecWaypoint.Location = new System.Drawing.Point(122, 306);
            this.btRecWaypoint.Name = "btRecWaypoint";
            this.btRecWaypoint.Size = new System.Drawing.Size(100, 27);
            this.btRecWaypoint.TabIndex = 31;
            this.btRecWaypoint.Text = "Record Waypoint";
            this.btRecWaypoint.UseVisualStyleBackColor = true;
            this.btRecWaypoint.Click += new System.EventHandler(this.btRecWaypoint_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(14, 134);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(66, 13);
            this.label11.TabIndex = 30;
            this.label11.Text = "Waypoints";
            // 
            // lvWaypoints
            // 
            this.lvWaypoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnRoute,
            this.columnWaypoint,
            this.columnX,
            this.columnY,
            this.columnZ});
            this.lvWaypoints.FullRowSelect = true;
            this.lvWaypoints.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvWaypoints.HideSelection = false;
            this.lvWaypoints.Location = new System.Drawing.Point(17, 150);
            this.lvWaypoints.Name = "lvWaypoints";
            this.lvWaypoints.Size = new System.Drawing.Size(310, 150);
            this.lvWaypoints.TabIndex = 29;
            this.lvWaypoints.UseCompatibleStateImageBehavior = false;
            this.lvWaypoints.View = System.Windows.Forms.View.Details;
            this.lvWaypoints.SelectedIndexChanged += new System.EventHandler(this.lvWaypoints_SelectedIndexChanged);
            // 
            // columnRoute
            // 
            this.columnRoute.Text = "Route";
            // 
            // columnWaypoint
            // 
            this.columnWaypoint.Text = "Waypoint";
            // 
            // columnX
            // 
            this.columnX.Text = "X";
            this.columnX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnX.Width = 62;
            // 
            // columnY
            // 
            this.columnY.Text = "Y";
            this.columnY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnY.Width = 62;
            // 
            // columnZ
            // 
            this.columnZ.Text = "Z";
            this.columnZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnZ.Width = 62;
            // 
            // ChatLogTab
            // 
            this.ChatLogTab.Controls.Add(this.lvwChatLog);
            this.ChatLogTab.Controls.Add(this.chatlog);
            this.ChatLogTab.Location = new System.Drawing.Point(4, 22);
            this.ChatLogTab.Name = "ChatLogTab";
            this.ChatLogTab.Size = new System.Drawing.Size(343, 504);
            this.ChatLogTab.TabIndex = 2;
            this.ChatLogTab.Text = "Chat Log Tracker";
            this.ChatLogTab.UseVisualStyleBackColor = true;
            // 
            // lvwChatLog
            // 
            this.lvwChatLog.Location = new System.Drawing.Point(8, 192);
            this.lvwChatLog.Name = "lvwChatLog";
            this.lvwChatLog.Size = new System.Drawing.Size(324, 201);
            this.lvwChatLog.TabIndex = 10;
            this.lvwChatLog.UseCompatibleStateImageBehavior = false;
            // 
            // chatlog
            // 
            this.chatlog.AutoSize = true;
            this.chatlog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatlog.Location = new System.Drawing.Point(8, 176);
            this.chatlog.Name = "chatlog";
            this.chatlog.Size = new System.Drawing.Size(62, 13);
            this.chatlog.TabIndex = 11;
            this.chatlog.Text = "Chat Log:";
            // 
            // cbStandStill
            // 
            this.cbStandStill.AutoSize = true;
            this.cbStandStill.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbStandStill.Location = new System.Drawing.Point(90, 437);
            this.cbStandStill.Name = "cbStandStill";
            this.cbStandStill.Size = new System.Drawing.Size(15, 14);
            this.cbStandStill.TabIndex = 46;
            this.cbStandStill.UseVisualStyleBackColor = true;
            this.cbStandStill.CheckedChanged += new System.EventHandler(this.cbStandStill_CheckChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(14, 437);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 13);
            this.label15.TabIndex = 45;
            this.label15.Text = "Stand still?";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 526);
            this.Controls.Add(this.tab);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "MagBot_FFXIV_v02";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tab.ResumeLayout(false);
            this.IntroTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ExpFarmingTab.ResumeLayout(false);
            this.ExpFarmingTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHours)).EndInit();
            this.ChatLogTab.ResumeLayout(false);
            this.ChatLogTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btExpFarming;
        private System.Windows.Forms.Label hp;
        private System.Windows.Forms.Label lbHP;
        private System.Windows.Forms.Label lbMP;
        private System.Windows.Forms.Label mp;
        private System.Windows.Forms.Label lbTP;
        private System.Windows.Forms.Label tp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbMaxHP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbMaxMP;
        private System.Windows.Forms.Label lbMaxTP;
        private System.Windows.Forms.Label lbZCoordinate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbXCoordinate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbYCoordinate;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbFacing;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabControl tab;
        private System.Windows.Forms.TabPage ExpFarmingTab;
        private System.Windows.Forms.TabPage IntroTab;
        private System.Windows.Forms.Button btRecWaypoint;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListView lvWaypoints;
        private System.Windows.Forms.ColumnHeader columnRoute;
        private System.Windows.Forms.ColumnHeader columnX;
        private System.Windows.Forms.ColumnHeader columnY;
        private System.Windows.Forms.ColumnHeader columnZ;
        private System.Windows.Forms.Button btNewRoute;
        private System.Windows.Forms.Button btDelWaypoint;
        private System.Windows.Forms.ColumnHeader columnWaypoint;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbEscapeRoutes;
        private System.Windows.Forms.Button btSaveRoutes;
        private System.Windows.Forms.Button btLoadRoutes;
        private System.Windows.Forms.Button btStopExpFarming;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabPage ChatLogTab;
        private System.Windows.Forms.ListView lvwChatLog;
        private System.Windows.Forms.Label chatlog;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.NumericUpDown nudMinutes;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudHours;
        private System.Windows.Forms.CheckBox cbStandStill;
        private System.Windows.Forms.Label label15;
    }
}


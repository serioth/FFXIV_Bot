using System;

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
            this.btStartExpFarming = new System.Windows.Forms.Button();
            this.tab = new System.Windows.Forms.TabControl();
            this.IntroTab = new System.Windows.Forms.TabPage();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ExpFarmingTab = new System.Windows.Forms.TabPage();
            this.lbExpFarmingElapsedTime = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.cbStandStill = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
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
            this.chatlog = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbMaxExp = new System.Windows.Forms.Label();
            this.lbMinExp = new System.Windows.Forms.Label();
            this.lbExpPerSec = new System.Windows.Forms.Label();
            this.lbChatLog = new System.Windows.Forms.ListBox();
            this.tab.SuspendLayout();
            this.IntroTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.ExpFarmingTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHours)).BeginInit();
            this.ChatLogTab.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btStartExpFarming
            // 
            this.btStartExpFarming.Enabled = false;
            this.btStartExpFarming.Location = new System.Drawing.Point(16, 468);
            this.btStartExpFarming.Name = "btStartExpFarming";
            this.btStartExpFarming.Size = new System.Drawing.Size(100, 27);
            this.btStartExpFarming.TabIndex = 1;
            this.btStartExpFarming.Text = "Start";
            this.btStartExpFarming.UseVisualStyleBackColor = true;
            this.btStartExpFarming.Click += new System.EventHandler(this.btStartExpFarming_Click);
            // 
            // tab
            // 
            this.tab.Controls.Add(this.IntroTab);
            this.tab.Controls.Add(this.ExpFarmingTab);
            this.tab.Controls.Add(this.ChatLogTab);
            this.tab.Location = new System.Drawing.Point(0, 27);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(354, 531);
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
            this.IntroTab.Size = new System.Drawing.Size(346, 505);
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
            this.pictureBox2.Size = new System.Drawing.Size(348, 284);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(9, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(327, 216);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // ExpFarmingTab
            // 
            this.ExpFarmingTab.Controls.Add(this.lbExpPerSec);
            this.ExpFarmingTab.Controls.Add(this.lbMinExp);
            this.ExpFarmingTab.Controls.Add(this.lbMaxExp);
            this.ExpFarmingTab.Controls.Add(this.label4);
            this.ExpFarmingTab.Controls.Add(this.label3);
            this.ExpFarmingTab.Controls.Add(this.label2);
            this.ExpFarmingTab.Controls.Add(this.label1);
            this.ExpFarmingTab.Controls.Add(this.lbExpFarmingElapsedTime);
            this.ExpFarmingTab.Controls.Add(this.label16);
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
            this.ExpFarmingTab.Controls.Add(this.btStartExpFarming);
            this.ExpFarmingTab.Location = new System.Drawing.Point(4, 22);
            this.ExpFarmingTab.Name = "ExpFarmingTab";
            this.ExpFarmingTab.Padding = new System.Windows.Forms.Padding(3);
            this.ExpFarmingTab.Size = new System.Drawing.Size(346, 505);
            this.ExpFarmingTab.TabIndex = 0;
            this.ExpFarmingTab.Text = "Exp. Farming";
            this.ExpFarmingTab.UseVisualStyleBackColor = true;
            // 
            // lbExpFarmingElapsedTime
            // 
            this.lbExpFarmingElapsedTime.AutoSize = true;
            this.lbExpFarmingElapsedTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbExpFarmingElapsedTime.Location = new System.Drawing.Point(248, 437);
            this.lbExpFarmingElapsedTime.Name = "lbExpFarmingElapsedTime";
            this.lbExpFarmingElapsedTime.Size = new System.Drawing.Size(79, 13);
            this.lbExpFarmingElapsedTime.TabIndex = 48;
            this.lbExpFarmingElapsedTime.Text = "00h:00m:00s";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(166, 437);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(83, 13);
            this.label16.TabIndex = 47;
            this.label16.Text = "Elapsed time:";
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
            // nudMinutes
            // 
            this.nudMinutes.Location = new System.Drawing.Point(268, 404);
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
            this.label14.Location = new System.Drawing.Point(301, 406);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(26, 13);
            this.label14.TabIndex = 43;
            this.label14.Text = "min";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(245, 406);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(18, 13);
            this.label13.TabIndex = 42;
            this.label13.Text = "hr";
            // 
            // nudHours
            // 
            this.nudHours.Location = new System.Drawing.Point(211, 404);
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
            this.label11.Location = new System.Drawing.Point(14, 105);
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
            this.lvWaypoints.Location = new System.Drawing.Point(17, 121);
            this.lvWaypoints.Name = "lvWaypoints";
            this.lvWaypoints.Size = new System.Drawing.Size(310, 179);
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
            this.ChatLogTab.Controls.Add(this.lbChatLog);
            this.ChatLogTab.Controls.Add(this.chatlog);
            this.ChatLogTab.Location = new System.Drawing.Point(4, 22);
            this.ChatLogTab.Name = "ChatLogTab";
            this.ChatLogTab.Size = new System.Drawing.Size(346, 505);
            this.ChatLogTab.TabIndex = 2;
            this.ChatLogTab.Text = "Chat Log Tracker";
            this.ChatLogTab.UseVisualStyleBackColor = true;
            // 
            // chatlog
            // 
            this.chatlog.AutoSize = true;
            this.chatlog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatlog.Location = new System.Drawing.Point(5, 12);
            this.chatlog.Name = "chatlog";
            this.chatlog.Size = new System.Drawing.Size(62, 13);
            this.chatlog.TabIndex = 11;
            this.chatlog.Text = "Chat Log:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(347, 24);
            this.menuStrip1.TabIndex = 30;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.printToolStripMenuItem,
            this.printPreviewToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "&Open";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripMenuItem.Image")));
            this.printToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.printToolStripMenuItem.Text = "&Print";
            // 
            // printPreviewToolStripMenuItem
            // 
            this.printPreviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printPreviewToolStripMenuItem.Image")));
            this.printPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.printPreviewToolStripMenuItem.Text = "Print Pre&view";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator3,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(141, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkValuesToolStripMenuItem,
            this.customizeToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // checkValuesToolStripMenuItem
            // 
            this.checkValuesToolStripMenuItem.Name = "checkValuesToolStripMenuItem";
            this.checkValuesToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.checkValuesToolStripMenuItem.Text = "Chec&k Values";
            this.checkValuesToolStripMenuItem.Click += new System.EventHandler(this.checkValuesToolStripMenuItem_Click);
            // 
            // customizeToolStripMenuItem
            // 
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            this.customizeToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.customizeToolStripMenuItem.Text = "&Customize";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(119, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "Real Time Stats:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 50;
            this.label2.Text = "Max EXP:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "Min EXP:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 52;
            this.label4.Text = "EXP per second:";
            // 
            // lbMaxExp
            // 
            this.lbMaxExp.AutoSize = true;
            this.lbMaxExp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxExp.Location = new System.Drawing.Point(119, 35);
            this.lbMaxExp.Name = "lbMaxExp";
            this.lbMaxExp.Size = new System.Drawing.Size(37, 13);
            this.lbMaxExp.TabIndex = 53;
            this.lbMaxExp.Text = "00000";
            // 
            // lbMinExp
            // 
            this.lbMinExp.AutoSize = true;
            this.lbMinExp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMinExp.Location = new System.Drawing.Point(119, 53);
            this.lbMinExp.Name = "lbMinExp";
            this.lbMinExp.Size = new System.Drawing.Size(37, 13);
            this.lbMinExp.TabIndex = 54;
            this.lbMinExp.Text = "00000";
            // 
            // lbExpPerSec
            // 
            this.lbExpPerSec.AutoSize = true;
            this.lbExpPerSec.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbExpPerSec.Location = new System.Drawing.Point(119, 71);
            this.lbExpPerSec.Name = "lbExpPerSec";
            this.lbExpPerSec.Size = new System.Drawing.Size(40, 13);
            this.lbExpPerSec.TabIndex = 55;
            this.lbExpPerSec.Text = "00.000";
            // 
            // lbChatLog
            // 
            this.lbChatLog.FormattingEnabled = true;
            this.lbChatLog.Location = new System.Drawing.Point(8, 28);
            this.lbChatLog.Name = "lbChatLog";
            this.lbChatLog.Size = new System.Drawing.Size(323, 459);
            this.lbChatLog.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 555);
            this.Controls.Add(this.tab);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
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
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btStartExpFarming;
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
        private System.Windows.Forms.Label chatlog;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.NumericUpDown nudMinutes;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudHours;
        private System.Windows.Forms.CheckBox cbStandStill;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lbExpFarmingElapsedTime;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkValuesToolStripMenuItem;
        private System.Windows.Forms.Label lbExpPerSec;
        private System.Windows.Forms.Label lbMinExp;
        private System.Windows.Forms.Label lbMaxExp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbChatLog;
    }
}


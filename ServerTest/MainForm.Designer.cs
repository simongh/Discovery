namespace ServerTest
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
			this.btnStart = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnAnnounce = new System.Windows.Forms.Button();
			this.btnBye = new System.Windows.Forms.Button();
			this.txtServiceType = new System.Windows.Forms.TextBox();
			this.txtResults = new System.Windows.Forms.TextBox();
			this.lbServices = new System.Windows.Forms.ListBox();
			this.lblServiceType = new System.Windows.Forms.Label();
			this.lblName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.lblLocation = new System.Windows.Forms.Label();
			this.txtLocation = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.pbThinking = new System.Windows.Forms.ProgressBar();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(13, 13);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 0;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnStop
			// 
			this.btnStop.Enabled = false;
			this.btnStop.Location = new System.Drawing.Point(95, 13);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 1;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnAnnounce
			// 
			this.btnAnnounce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAnnounce.Enabled = false;
			this.btnAnnounce.Location = new System.Drawing.Point(484, 367);
			this.btnAnnounce.Name = "btnAnnounce";
			this.btnAnnounce.Size = new System.Drawing.Size(75, 23);
			this.btnAnnounce.TabIndex = 2;
			this.btnAnnounce.Text = "Announce";
			this.btnAnnounce.UseVisualStyleBackColor = true;
			this.btnAnnounce.Click += new System.EventHandler(this.btnAnnounce_Click);
			// 
			// btnBye
			// 
			this.btnBye.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBye.Enabled = false;
			this.btnBye.Location = new System.Drawing.Point(592, 367);
			this.btnBye.Name = "btnBye";
			this.btnBye.Size = new System.Drawing.Size(75, 23);
			this.btnBye.TabIndex = 3;
			this.btnBye.Text = "Bye";
			this.btnBye.UseVisualStyleBackColor = true;
			this.btnBye.Click += new System.EventHandler(this.btnBye_Click);
			// 
			// txtServiceType
			// 
			this.txtServiceType.Location = new System.Drawing.Point(6, 36);
			this.txtServiceType.Name = "txtServiceType";
			this.txtServiceType.Size = new System.Drawing.Size(156, 20);
			this.txtServiceType.TabIndex = 4;
			// 
			// txtResults
			// 
			this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtResults.Location = new System.Drawing.Point(13, 40);
			this.txtResults.Multiline = true;
			this.txtResults.Name = "txtResults";
			this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtResults.Size = new System.Drawing.Size(465, 350);
			this.txtResults.TabIndex = 5;
			// 
			// lbServices
			// 
			this.lbServices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbServices.FormattingEnabled = true;
			this.lbServices.Location = new System.Drawing.Point(484, 210);
			this.lbServices.Name = "lbServices";
			this.lbServices.Size = new System.Drawing.Size(183, 147);
			this.lbServices.TabIndex = 6;
			this.lbServices.SelectedIndexChanged += new System.EventHandler(this.lbServices_SelectedIndexChanged);
			// 
			// lblServiceType
			// 
			this.lblServiceType.AutoSize = true;
			this.lblServiceType.Location = new System.Drawing.Point(6, 20);
			this.lblServiceType.Name = "lblServiceType";
			this.lblServiceType.Size = new System.Drawing.Size(70, 13);
			this.lblServiceType.TabIndex = 7;
			this.lblServiceType.Text = "Service Type";
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.Location = new System.Drawing.Point(9, 63);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(35, 13);
			this.lblName.TabIndex = 8;
			this.lblName.Text = "Name";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(6, 80);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(156, 20);
			this.txtName.TabIndex = 9;
			// 
			// lblLocation
			// 
			this.lblLocation.AutoSize = true;
			this.lblLocation.Location = new System.Drawing.Point(12, 107);
			this.lblLocation.Name = "lblLocation";
			this.lblLocation.Size = new System.Drawing.Size(48, 13);
			this.lblLocation.TabIndex = 10;
			this.lblLocation.Text = "Location";
			// 
			// txtLocation
			// 
			this.txtLocation.Location = new System.Drawing.Point(6, 124);
			this.txtLocation.Name = "txtLocation";
			this.txtLocation.Size = new System.Drawing.Size(156, 20);
			this.txtLocation.TabIndex = 11;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnAdd);
			this.groupBox1.Controls.Add(this.txtServiceType);
			this.groupBox1.Controls.Add(this.txtLocation);
			this.groupBox1.Controls.Add(this.lblServiceType);
			this.groupBox1.Controls.Add(this.lblLocation);
			this.groupBox1.Controls.Add(this.lblName);
			this.groupBox1.Controls.Add(this.txtName);
			this.groupBox1.Location = new System.Drawing.Point(484, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(183, 191);
			this.groupBox1.TabIndex = 12;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Services";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(87, 150);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(75, 23);
			this.btnAdd.TabIndex = 12;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// pbThinking
			// 
			this.pbThinking.Location = new System.Drawing.Point(177, 24);
			this.pbThinking.Name = "pbThinking";
			this.pbThinking.Size = new System.Drawing.Size(301, 10);
			this.pbThinking.TabIndex = 13;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(683, 406);
			this.Controls.Add(this.pbThinking);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lbServices);
			this.Controls.Add(this.txtResults);
			this.Controls.Add(this.btnBye);
			this.Controls.Add(this.btnAnnounce);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnStart);
			this.MinimumSize = new System.Drawing.Size(520, 367);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnAnnounce;
		private System.Windows.Forms.Button btnBye;
		private System.Windows.Forms.TextBox txtServiceType;
		private System.Windows.Forms.TextBox txtResults;
		private System.Windows.Forms.ListBox lbServices;
		private System.Windows.Forms.Label lblServiceType;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label lblLocation;
		private System.Windows.Forms.TextBox txtLocation;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.ProgressBar pbThinking;
	}
}
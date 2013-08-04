namespace ClientTest
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;


		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.txtServiceType = new System.Windows.Forms.TextBox();
			this.lblServiceType = new System.Windows.Forms.Label();
			this.btnDiscover = new System.Windows.Forms.Button();
			this.gbListen = new System.Windows.Forms.GroupBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.lblResults = new System.Windows.Forms.Label();
			this.txtResults = new System.Windows.Forms.TextBox();
			this.pbThinking = new System.Windows.Forms.ProgressBar();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.gbListen.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtServiceType
			// 
			this.txtServiceType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtServiceType.Location = new System.Drawing.Point(16, 29);
			this.txtServiceType.Name = "txtServiceType";
			this.txtServiceType.Size = new System.Drawing.Size(350, 20);
			this.txtServiceType.TabIndex = 0;
			// 
			// lblServiceType
			// 
			this.lblServiceType.AutoSize = true;
			this.lblServiceType.Location = new System.Drawing.Point(13, 13);
			this.lblServiceType.Name = "lblServiceType";
			this.lblServiceType.Size = new System.Drawing.Size(70, 13);
			this.lblServiceType.TabIndex = 1;
			this.lblServiceType.Text = "Service Type";
			// 
			// btnDiscover
			// 
			this.btnDiscover.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDiscover.Location = new System.Drawing.Point(372, 27);
			this.btnDiscover.Name = "btnDiscover";
			this.btnDiscover.Size = new System.Drawing.Size(75, 23);
			this.btnDiscover.TabIndex = 2;
			this.btnDiscover.Text = "Discover";
			this.btnDiscover.UseVisualStyleBackColor = true;
			this.btnDiscover.Click += new System.EventHandler(this.btnDiscover_Click);
			// 
			// gbListen
			// 
			this.gbListen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbListen.Controls.Add(this.btnStop);
			this.gbListen.Controls.Add(this.btnStart);
			this.gbListen.Location = new System.Drawing.Point(453, 12);
			this.gbListen.Name = "gbListen";
			this.gbListen.Size = new System.Drawing.Size(180, 57);
			this.gbListen.TabIndex = 3;
			this.gbListen.TabStop = false;
			this.gbListen.Text = "Listen";
			// 
			// btnStop
			// 
			this.btnStop.Enabled = false;
			this.btnStop.Location = new System.Drawing.Point(89, 19);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 1;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(7, 20);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 0;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// lblResults
			// 
			this.lblResults.AutoSize = true;
			this.lblResults.Location = new System.Drawing.Point(13, 75);
			this.lblResults.Name = "lblResults";
			this.lblResults.Size = new System.Drawing.Size(42, 13);
			this.lblResults.TabIndex = 4;
			this.lblResults.Text = "Results";
			// 
			// txtResults
			// 
			this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtResults.Location = new System.Drawing.Point(16, 92);
			this.txtResults.Multiline = true;
			this.txtResults.Name = "txtResults";
			this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtResults.Size = new System.Drawing.Size(617, 186);
			this.txtResults.TabIndex = 5;
			// 
			// pbThinking
			// 
			this.pbThinking.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pbThinking.Location = new System.Drawing.Point(16, 55);
			this.pbThinking.Name = "pbThinking";
			this.pbThinking.Size = new System.Drawing.Size(350, 13);
			this.pbThinking.Step = 1;
			this.pbThinking.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbThinking.TabIndex = 6;
			// 
			// timer
			// 
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(653, 290);
			this.Controls.Add(this.pbThinking);
			this.Controls.Add(this.txtResults);
			this.Controls.Add(this.lblResults);
			this.Controls.Add(this.gbListen);
			this.Controls.Add(this.btnDiscover);
			this.Controls.Add(this.lblServiceType);
			this.Controls.Add(this.txtServiceType);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.gbListen.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtServiceType;
		private System.Windows.Forms.Label lblServiceType;
		private System.Windows.Forms.Button btnDiscover;
		private System.Windows.Forms.GroupBox gbListen;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Label lblResults;
		private System.Windows.Forms.TextBox txtResults;
		private System.Windows.Forms.ProgressBar pbThinking;
		private System.Windows.Forms.Timer timer;
	}
}
// Copyright (C) 2003  Paul Welter
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace NetSpell.SpellChecker
{
	/// <summary>
	/// Summary description for OptionForm.
	/// </summary>
	public class OptionForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabPage aboutTab;
		private System.Windows.Forms.Button CancelBtn;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListView DictionaryList;
		private System.Windows.Forms.TabPage dictionaryTab;
		private System.Windows.Forms.TabPage generalTab;
		private System.Windows.Forms.CheckBox IgnoreDigitsCheck;
		private System.Windows.Forms.CheckBox IgnoreUpperCheck;
		private System.Windows.Forms.Label lbllabel1;
		private System.Windows.Forms.TextBox MaxSuggestions;
		private System.Windows.Forms.ColumnHeader nameColumn;
		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.TabControl optionsTabControl;

		private NetSpell.SpellChecker.Spelling SpellChecker;
		private System.Windows.Forms.TabPage versionsTab;
		internal System.Windows.Forms.Label lblCopyright;
		internal System.Windows.Forms.Label lblDescription;
		internal System.Windows.Forms.Label lblVersion;
		internal System.Windows.Forms.Label lblTitle;
		internal System.Windows.Forms.PictureBox pbIcon;
		private System.Windows.Forms.ListView assembliesListView;
		private System.Windows.Forms.ColumnHeader assemblyColumnHeader;
		private System.Windows.Forms.ColumnHeader versionColumnHeader;
		private System.Windows.Forms.ColumnHeader dateColumnHeader;
		private System.Windows.Forms.Label lblCompany;
		private System.Windows.Forms.ColumnHeader wordsColumn;

		/// <summary>
		///		Default Constroctor
		/// </summary>
		public OptionForm(ref Spelling spell)
		{
			this.SpellChecker = spell;
			InitializeComponent();
		}

		private void aboutTab_Click(object sender, System.EventArgs e)
		{
		
		}

		private void OptionForm_Load(object sender, System.EventArgs e)
		{
			this.IgnoreDigitsCheck.Checked = this.SpellChecker.IgnoreWordsWithDigits;
			this.IgnoreUpperCheck.Checked = this.SpellChecker.IgnoreAllCapsWords;
			this.MaxSuggestions.Text = this.SpellChecker.MaxSuggestions.ToString();

			// set dictionary info
			foreach (Dictionary dic in this.SpellChecker.Dictionaries)
			{
				ListViewItem item = new ListViewItem();
				item.Text = dic.FileName;
				item.SubItems.Add(dic.WordList.Count.ToString());
				DictionaryList.Items.Add(item);
			}

			// set about info
			this.pbIcon.Image = this.Owner.Icon.ToBitmap();

			AssemblyInfo aInfo = new AssemblyInfo();
			this.lblTitle.Text = aInfo.Title;
			this.lblVersion.Text = string.Format("Version {0}", aInfo.Version);
			this.lblCopyright.Text = aInfo.Copyright;
			this.lblDescription.Text = aInfo.Description;
			this.lblCompany.Text = aInfo.Company;

			// Get all modules
			ArrayList localItems = new ArrayList();
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
			{
				ListViewItem item = new ListViewItem();
				item.Text = module.ModuleName;

				// Get version info
				FileVersionInfo verInfo = module.FileVersionInfo;
				string versionStr = String.Format("{0}.{1}.{2}.{3}", 
					verInfo.FileMajorPart,
					verInfo.FileMinorPart,
					verInfo.FileBuildPart,
					verInfo.FilePrivatePart);
				item.SubItems.Add(versionStr);

				// Get file date info
				DateTime lastWriteDate = File.GetLastWriteTime(module.FileName);
				string dateStr = lastWriteDate.ToString("MMM dd, yyyy");
				item.SubItems.Add(dateStr);

				assembliesListView.Items.Add(item);

				// Stash assemply related list view items for later
				if (module.ModuleName.ToLower().StartsWith("netspell"))
				{
					localItems.Add(item);
				}
			}

			// Extract the assemply related modules and move them to the top
			for (int i = localItems.Count; i > 0; i--)
			{
				ListViewItem localItem = (ListViewItem)localItems[i-1];
				assembliesListView.Items.Remove(localItem);
				assembliesListView.Items.Insert(0, localItem);
			}
		}

		private void optionsTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.optionsTabControl = new System.Windows.Forms.TabControl();
			this.generalTab = new System.Windows.Forms.TabPage();
			this.lbllabel1 = new System.Windows.Forms.Label();
			this.MaxSuggestions = new System.Windows.Forms.TextBox();
			this.IgnoreUpperCheck = new System.Windows.Forms.CheckBox();
			this.IgnoreDigitsCheck = new System.Windows.Forms.CheckBox();
			this.dictionaryTab = new System.Windows.Forms.TabPage();
			this.DictionaryList = new System.Windows.Forms.ListView();
			this.nameColumn = new System.Windows.Forms.ColumnHeader();
			this.wordsColumn = new System.Windows.Forms.ColumnHeader();
			this.versionsTab = new System.Windows.Forms.TabPage();
			this.assembliesListView = new System.Windows.Forms.ListView();
			this.assemblyColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.versionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.dateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.aboutTab = new System.Windows.Forms.TabPage();
			this.lblCompany = new System.Windows.Forms.Label();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblTitle = new System.Windows.Forms.Label();
			this.pbIcon = new System.Windows.Forms.PictureBox();
			this.OkButton = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.optionsTabControl.SuspendLayout();
			this.generalTab.SuspendLayout();
			this.dictionaryTab.SuspendLayout();
			this.versionsTab.SuspendLayout();
			this.aboutTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// optionsTabControl
			// 
			this.optionsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.optionsTabControl.Controls.Add(this.generalTab);
			this.optionsTabControl.Controls.Add(this.dictionaryTab);
			this.optionsTabControl.Controls.Add(this.versionsTab);
			this.optionsTabControl.Controls.Add(this.aboutTab);
			this.optionsTabControl.Location = new System.Drawing.Point(8, 8);
			this.optionsTabControl.Name = "optionsTabControl";
			this.optionsTabControl.SelectedIndex = 0;
			this.optionsTabControl.Size = new System.Drawing.Size(386, 184);
			this.optionsTabControl.TabIndex = 0;
			this.optionsTabControl.SelectedIndexChanged += new System.EventHandler(this.optionsTabControl_SelectedIndexChanged);
			// 
			// generalTab
			// 
			this.generalTab.Controls.Add(this.lbllabel1);
			this.generalTab.Controls.Add(this.MaxSuggestions);
			this.generalTab.Controls.Add(this.IgnoreUpperCheck);
			this.generalTab.Controls.Add(this.IgnoreDigitsCheck);
			this.generalTab.Location = new System.Drawing.Point(4, 22);
			this.generalTab.Name = "generalTab";
			this.generalTab.Size = new System.Drawing.Size(378, 158);
			this.generalTab.TabIndex = 0;
			this.generalTab.Text = "General";
			// 
			// lbllabel1
			// 
			this.lbllabel1.Location = new System.Drawing.Point(48, 88);
			this.lbllabel1.Name = "lbllabel1";
			this.lbllabel1.Size = new System.Drawing.Size(264, 16);
			this.lbllabel1.TabIndex = 7;
			this.lbllabel1.Text = "Maximum &Suggestion Count";
			// 
			// MaxSuggestions
			// 
			this.MaxSuggestions.Location = new System.Drawing.Point(24, 86);
			this.MaxSuggestions.MaxLength = 2;
			this.MaxSuggestions.Name = "MaxSuggestions";
			this.MaxSuggestions.Size = new System.Drawing.Size(24, 20);
			this.MaxSuggestions.TabIndex = 3;
			this.MaxSuggestions.Text = "25";
			// 
			// IgnoreUpperCheck
			// 
			this.IgnoreUpperCheck.Checked = true;
			this.IgnoreUpperCheck.CheckState = System.Windows.Forms.CheckState.Checked;
			this.IgnoreUpperCheck.Location = new System.Drawing.Point(32, 48);
			this.IgnoreUpperCheck.Name = "IgnoreUpperCheck";
			this.IgnoreUpperCheck.Size = new System.Drawing.Size(296, 24);
			this.IgnoreUpperCheck.TabIndex = 2;
			this.IgnoreUpperCheck.Text = "Ignore Words in all &Upper Case";
			// 
			// IgnoreDigitsCheck
			// 
			this.IgnoreDigitsCheck.Location = new System.Drawing.Point(32, 24);
			this.IgnoreDigitsCheck.Name = "IgnoreDigitsCheck";
			this.IgnoreDigitsCheck.Size = new System.Drawing.Size(296, 24);
			this.IgnoreDigitsCheck.TabIndex = 1;
			this.IgnoreDigitsCheck.Text = "Ignore Words with &Digits";
			// 
			// dictionaryTab
			// 
			this.dictionaryTab.Controls.Add(this.DictionaryList);
			this.dictionaryTab.Location = new System.Drawing.Point(4, 22);
			this.dictionaryTab.Name = "dictionaryTab";
			this.dictionaryTab.Size = new System.Drawing.Size(378, 158);
			this.dictionaryTab.TabIndex = 2;
			this.dictionaryTab.Text = "Dictionaries";
			// 
			// DictionaryList
			// 
			this.DictionaryList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							 this.nameColumn,
																							 this.wordsColumn});
			this.DictionaryList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DictionaryList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.DictionaryList.LabelWrap = false;
			this.DictionaryList.Location = new System.Drawing.Point(0, 0);
			this.DictionaryList.MultiSelect = false;
			this.DictionaryList.Name = "DictionaryList";
			this.DictionaryList.Size = new System.Drawing.Size(378, 158);
			this.DictionaryList.TabIndex = 4;
			this.DictionaryList.View = System.Windows.Forms.View.Details;
			// 
			// nameColumn
			// 
			this.nameColumn.Text = "Name";
			this.nameColumn.Width = 281;
			// 
			// wordsColumn
			// 
			this.wordsColumn.Text = "Words";
			this.wordsColumn.Width = 80;
			// 
			// versionsTab
			// 
			this.versionsTab.Controls.Add(this.assembliesListView);
			this.versionsTab.Location = new System.Drawing.Point(4, 22);
			this.versionsTab.Name = "versionsTab";
			this.versionsTab.Size = new System.Drawing.Size(378, 158);
			this.versionsTab.TabIndex = 3;
			this.versionsTab.Text = "Versions";
			// 
			// assembliesListView
			// 
			this.assembliesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								 this.assemblyColumnHeader,
																								 this.versionColumnHeader,
																								 this.dateColumnHeader});
			this.assembliesListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assembliesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.assembliesListView.Location = new System.Drawing.Point(0, 0);
			this.assembliesListView.Name = "assembliesListView";
			this.assembliesListView.Size = new System.Drawing.Size(378, 158);
			this.assembliesListView.TabIndex = 10;
			this.assembliesListView.View = System.Windows.Forms.View.Details;
			// 
			// assemblyColumnHeader
			// 
			this.assemblyColumnHeader.Text = "Module";
			this.assemblyColumnHeader.Width = 176;
			// 
			// versionColumnHeader
			// 
			this.versionColumnHeader.Text = "Version";
			this.versionColumnHeader.Width = 92;
			// 
			// dateColumnHeader
			// 
			this.dateColumnHeader.Text = "Date";
			this.dateColumnHeader.Width = 87;
			// 
			// aboutTab
			// 
			this.aboutTab.Controls.Add(this.lblCompany);
			this.aboutTab.Controls.Add(this.lblCopyright);
			this.aboutTab.Controls.Add(this.lblDescription);
			this.aboutTab.Controls.Add(this.lblVersion);
			this.aboutTab.Controls.Add(this.lblTitle);
			this.aboutTab.Controls.Add(this.pbIcon);
			this.aboutTab.Location = new System.Drawing.Point(4, 22);
			this.aboutTab.Name = "aboutTab";
			this.aboutTab.Size = new System.Drawing.Size(378, 158);
			this.aboutTab.TabIndex = 4;
			this.aboutTab.Text = "About";
			// 
			// lblCompany
			// 
			this.lblCompany.Location = new System.Drawing.Point(64, 128);
			this.lblCompany.Name = "lblCompany";
			this.lblCompany.Size = new System.Drawing.Size(296, 23);
			this.lblCompany.TabIndex = 20;
			this.lblCompany.Text = "Application Company";
			// 
			// lblCopyright
			// 
			this.lblCopyright.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblCopyright.Location = new System.Drawing.Point(64, 64);
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Size = new System.Drawing.Size(296, 23);
			this.lblCopyright.TabIndex = 19;
			this.lblCopyright.Text = "Application Copyright";
			// 
			// lblDescription
			// 
			this.lblDescription.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblDescription.Location = new System.Drawing.Point(64, 88);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(296, 40);
			this.lblDescription.TabIndex = 18;
			this.lblDescription.Text = "Application Description";
			// 
			// lblVersion
			// 
			this.lblVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblVersion.Location = new System.Drawing.Point(64, 40);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(296, 23);
			this.lblVersion.TabIndex = 17;
			this.lblVersion.Text = "Application Version";
			// 
			// lblTitle
			// 
			this.lblTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblTitle.Location = new System.Drawing.Point(64, 16);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(296, 24);
			this.lblTitle.TabIndex = 16;
			this.lblTitle.Text = "Application Title";
			// 
			// pbIcon
			// 
			this.pbIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.pbIcon.Location = new System.Drawing.Point(16, 16);
			this.pbIcon.Name = "pbIcon";
			this.pbIcon.Size = new System.Drawing.Size(32, 32);
			this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pbIcon.TabIndex = 15;
			this.pbIcon.TabStop = false;
			// 
			// OkButton
			// 
			this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkButton.Location = new System.Drawing.Point(226, 200);
			this.OkButton.Name = "OkButton";
			this.OkButton.TabIndex = 5;
			this.OkButton.Text = "&OK";
			this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
			// 
			// CancelBtn
			// 
			this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(314, 200);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.TabIndex = 6;
			this.CancelBtn.Text = "&Cancel";
			// 
			// OptionForm
			// 
			this.AcceptButton = this.OkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(402, 232);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OkButton);
			this.Controls.Add(this.optionsTabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Options";
			this.Load += new System.EventHandler(this.OptionForm_Load);
			this.optionsTabControl.ResumeLayout(false);
			this.generalTab.ResumeLayout(false);
			this.dictionaryTab.ResumeLayout(false);
			this.versionsTab.ResumeLayout(false);
			this.aboutTab.ResumeLayout(false);
			this.ResumeLayout(false);

		}
#endregion

		private void OkButton_Click(object sender, System.EventArgs e)
		{
			this.SpellChecker.IgnoreWordsWithDigits = this.IgnoreDigitsCheck.Checked;
			this.SpellChecker.IgnoreAllCapsWords = this.IgnoreUpperCheck.Checked;
			this.SpellChecker.MaxSuggestions = int.Parse(this.MaxSuggestions.Text);
			this.Close();
		}


	}


	/// <summary>
	/// Summary description 
	/// </summary>
	public class AssemblyInfo
	{
		private Type myType = typeof(OptionForm);

		/// <summary>
		/// CodeBase of Assembly
		/// </summary>
		public string CodeBase
		{
			get {return myType.Assembly.CodeBase.ToString();}
		}

		/// <summary>
		/// Company of Assembly
		/// </summary>
		public string Company
		{
			get
			{
				Object[] r = myType.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				AssemblyCompanyAttribute ct = (AssemblyCompanyAttribute)r[0];
				return ct.Company;
			}
		}
	
		/// <summary>
		/// Copyright of Assembly
		/// </summary>
		public string Copyright
		{
			get
			{
				Object[] r = myType.Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				AssemblyCopyrightAttribute ct = (AssemblyCopyrightAttribute)r[0];
				return ct.Copyright;
			}
		}

		/// <summary>
		/// Description of Assembly
		/// </summary>
		public string Description
		{
			get
			{
				Object[] r = myType.Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				AssemblyDescriptionAttribute ct = (AssemblyDescriptionAttribute)r[0];
				return ct.Description;
			}
		}

		/// <summary>
		///		FullName of Assembly
		/// </summary>
		public string FullName
		{
			get {return myType.Assembly.GetName().FullName.ToString();}
		}

		/// <summary>
		/// Name of Assembly
		/// </summary>
		public string Name
		{
			get	{return myType.Assembly.GetName().Name.ToString();}
		}

		/// <summary>
		/// Product of Assembly
		/// </summary>
		public string Product
		{
			get
			{
				Object[] r = myType.Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				AssemblyProductAttribute ct = (AssemblyProductAttribute)r[0];
				return ct.Product;
			}
		}

		/// <summary>
		/// Title of Assembly
		/// </summary>
		public string Title
		{
			get
			{
				Object[] r = myType.Assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				AssemblyTitleAttribute ct = (AssemblyTitleAttribute)r[0];
				return ct.Title;
			}
		}

		/// <summary>
		/// Version of Assembly
		/// </summary>
		public string Version
		{
			get { return myType.Assembly.GetName().Version.ToString(); }
		}
	}
}

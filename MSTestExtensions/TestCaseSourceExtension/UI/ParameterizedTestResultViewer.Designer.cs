namespace TestCaseSourceExtension.UI
{
    partial class ParameterizedTestResultViewer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.resultsGrid = new System.Windows.Forms.DataGridView();
            this.CaseNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Input = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Outcome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResultContent = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // resultsGrid
            // 
            this.resultsGrid.AllowUserToAddRows = false;
            this.resultsGrid.AllowUserToDeleteRows = false;
            this.resultsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CaseNo,
            this.Input,
            this.Outcome});
            this.resultsGrid.Location = new System.Drawing.Point(52, 49);
            this.resultsGrid.Name = "resultsGrid";
            this.resultsGrid.ReadOnly = true;
            this.resultsGrid.RowTemplate.Height = 24;
            this.resultsGrid.Size = new System.Drawing.Size(400, 150);
            this.resultsGrid.TabIndex = 0;
            // 
            // CaseNo
            // 
            this.CaseNo.HeaderText = "#";
            this.CaseNo.Name = "CaseNo";
            this.CaseNo.ReadOnly = true;
            // 
            // Input
            // 
            this.Input.HeaderText = "Input";
            this.Input.Name = "Input";
            this.Input.ReadOnly = true;
            // 
            // Outcome
            // 
            this.Outcome.HeaderText = "Outcome";
            this.Outcome.Name = "Outcome";
            this.Outcome.ReadOnly = true;
            // 
            // ResultContent
            // 
            this.ResultContent.AutoSize = true;
            this.ResultContent.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ResultContent.Dock = System.Windows.Forms.DockStyle.Top;
            this.ResultContent.Location = new System.Drawing.Point(0, 0);
            this.ResultContent.Name = "ResultContent";
            this.ResultContent.Size = new System.Drawing.Size(122, 17);
            this.ResultContent.TabIndex = 1;
            this.ResultContent.Text = "What\'s the result?";
            // 
            // ParameterizedTestResultViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ResultContent);
            this.Controls.Add(this.resultsGrid);
            this.Name = "ParameterizedTestResultViewer";
            this.Size = new System.Drawing.Size(400, 150);
            ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView resultsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn CaseNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Input;
        private System.Windows.Forms.DataGridViewTextBoxColumn Outcome;
        private System.Windows.Forms.Label ResultContent;
    }
}

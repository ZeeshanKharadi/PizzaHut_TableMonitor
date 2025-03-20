
namespace TableMonitor.Forms
{
    partial class AddPrinter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddPrinter));
            this.label1 = new System.Windows.Forms.Label();
            this.TxtPrinterName = new System.Windows.Forms.TextBox();
            this.TxtPrintStatus = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtTransacionType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PrinterUpdatebtn = new System.Windows.Forms.Button();
            this.BackBtn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Add_Printer = new System.Windows.Forms.Button();
            this.Delete_Printer = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.printerId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TextPrinterFloor = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TextPosTerminal = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(23, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Printer Name";
            // 
            // TxtPrinterName
            // 
            this.TxtPrinterName.Location = new System.Drawing.Point(145, 68);
            this.TxtPrinterName.Name = "TxtPrinterName";
            this.TxtPrinterName.Size = new System.Drawing.Size(190, 20);
            this.TxtPrinterName.TabIndex = 2;
            // 
            // TxtPrintStatus
            // 
            this.TxtPrintStatus.Location = new System.Drawing.Point(145, 94);
            this.TxtPrintStatus.Name = "TxtPrintStatus";
            this.TxtPrintStatus.Size = new System.Drawing.Size(190, 20);
            this.TxtPrintStatus.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(23, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Status";
            // 
            // TxtTransacionType
            // 
            this.TxtTransacionType.Location = new System.Drawing.Point(145, 120);
            this.TxtTransacionType.Name = "TxtTransacionType";
            this.TxtTransacionType.Size = new System.Drawing.Size(190, 20);
            this.TxtTransacionType.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(23, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pool";
            // 
            // PrinterUpdatebtn
            // 
            this.PrinterUpdatebtn.BackColor = System.Drawing.Color.Black;
            this.PrinterUpdatebtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.PrinterUpdatebtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.PrinterUpdatebtn.Location = new System.Drawing.Point(453, 68);
            this.PrinterUpdatebtn.Name = "PrinterUpdatebtn";
            this.PrinterUpdatebtn.Size = new System.Drawing.Size(96, 33);
            this.PrinterUpdatebtn.TabIndex = 10;
            this.PrinterUpdatebtn.Text = "UPDATE";
            this.PrinterUpdatebtn.UseVisualStyleBackColor = false;
            this.PrinterUpdatebtn.Click += new System.EventHandler(this.PrinterUpdatebtn_Click);
            // 
            // BackBtn
            // 
            this.BackBtn.BackColor = System.Drawing.Color.Black;
            this.BackBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BackBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackBtn.Location = new System.Drawing.Point(453, 107);
            this.BackBtn.Name = "BackBtn";
            this.BackBtn.Size = new System.Drawing.Size(96, 33);
            this.BackBtn.TabIndex = 12;
            this.BackBtn.Text = "BACK";
            this.BackBtn.UseVisualStyleBackColor = false;
            this.BackBtn.Click += new System.EventHandler(this.BackBtn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(28, 197);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(523, 186);
            this.dataGridView1.TabIndex = 13;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Add_Printer
            // 
            this.Add_Printer.BackColor = System.Drawing.Color.Black;
            this.Add_Printer.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Add_Printer.FlatAppearance.BorderSize = 2;
            this.Add_Printer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Add_Printer.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Add_Printer.Location = new System.Drawing.Point(351, 68);
            this.Add_Printer.Name = "Add_Printer";
            this.Add_Printer.Size = new System.Drawing.Size(96, 33);
            this.Add_Printer.TabIndex = 14;
            this.Add_Printer.Text = "ADD";
            this.Add_Printer.UseVisualStyleBackColor = false;
            this.Add_Printer.Click += new System.EventHandler(this.Add_Printer_Click);
            // 
            // Delete_Printer
            // 
            this.Delete_Printer.BackColor = System.Drawing.Color.Black;
            this.Delete_Printer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Delete_Printer.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Delete_Printer.Location = new System.Drawing.Point(351, 107);
            this.Delete_Printer.Name = "Delete_Printer";
            this.Delete_Printer.Size = new System.Drawing.Size(96, 33);
            this.Delete_Printer.TabIndex = 15;
            this.Delete_Printer.Text = "DELETE";
            this.Delete_Printer.UseVisualStyleBackColor = false;
            this.Delete_Printer.Click += new System.EventHandler(this.Delete_Printer_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(180, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(284, 31);
            this.label4.TabIndex = 16;
            this.label4.Text = "Printer Configurations ";
            // 
            // printerId
            // 
            this.printerId.Location = new System.Drawing.Point(351, 147);
            this.printerId.Name = "printerId";
            this.printerId.Size = new System.Drawing.Size(190, 20);
            this.printerId.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(23, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 25);
            this.label5.TabIndex = 18;
            this.label5.Text = "Floor";
            // 
            // TextPrinterFloor
            // 
            this.TextPrinterFloor.Location = new System.Drawing.Point(145, 145);
            this.TextPrinterFloor.Name = "TextPrinterFloor";
            this.TextPrinterFloor.Size = new System.Drawing.Size(190, 20);
            this.TextPrinterFloor.TabIndex = 19;
            // 
            // AddPrinter
            // 

            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(23, 172);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 25);
            this.label6.TabIndex = 20;
            this.label6.Text = "Terminal";
            // 
            // TextPrinterFloor
            // 
            this.TextPosTerminal.Location = new System.Drawing.Point(145, 172);
            this.TextPosTerminal.Name = "TextPosTerminal";
            this.TextPosTerminal.Size = new System.Drawing.Size(190, 20);
            this.TextPosTerminal.TabIndex = 21;
            // 

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(585, 395);

            this.Controls.Add(this.TextPosTerminal);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TextPrinterFloor);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.printerId);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Delete_Printer);
            this.Controls.Add(this.Add_Printer);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.BackBtn);
            this.Controls.Add(this.PrinterUpdatebtn);
            this.Controls.Add(this.TxtTransacionType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TxtPrintStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TxtPrinterName);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddPrinter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Printer Configurations";
            this.Load += new System.EventHandler(this.AddPrinter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtPrinterName;
        private System.Windows.Forms.TextBox TxtPrintStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtTransacionType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button PrinterUpdatebtn;
        private System.Windows.Forms.Button BackBtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button Add_Printer;
        private System.Windows.Forms.Button Delete_Printer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox printerId;
        //private System.Windows.Forms.TextBox TxtPrinterFloor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TextPrinterFloor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TextPosTerminal;
    }
}
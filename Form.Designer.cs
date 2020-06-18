﻿namespace service_client
{
    partial class Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
            this.label_frame = new System.Windows.Forms.GroupBox();
            this.lbl_region = new System.Windows.Forms.Label();
            this.end_date = new System.Windows.Forms.Label();
            this.start_date = new System.Windows.Forms.Label();
            this.input_frame = new System.Windows.Forms.GroupBox();
            this.input_region = new System.Windows.Forms.ComboBox();
            this.input_end = new System.Windows.Forms.DateTimePicker();
            this.input_start = new System.Windows.Forms.DateTimePicker();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.btn_save = new System.Windows.Forms.Button();
            this.lbl_status = new System.Windows.Forms.Label();
            this.lbl_percent = new System.Windows.Forms.Label();
            this.btn_exit = new System.Windows.Forms.Button();
            this.label_frame.SuspendLayout();
            this.input_frame.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_frame
            // 
            this.label_frame.Controls.Add(this.lbl_region);
            this.label_frame.Controls.Add(this.end_date);
            this.label_frame.Controls.Add(this.start_date);
            this.label_frame.Location = new System.Drawing.Point(52, 41);
            this.label_frame.Name = "label_frame";
            this.label_frame.Size = new System.Drawing.Size(189, 191);
            this.label_frame.TabIndex = 0;
            this.label_frame.TabStop = false;
            // 
            // lbl_region
            // 
            this.lbl_region.AutoSize = true;
            this.lbl_region.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_region.Location = new System.Drawing.Point(25, 114);
            this.lbl_region.Name = "lbl_region";
            this.lbl_region.Size = new System.Drawing.Size(161, 20);
            this.lbl_region.TabIndex = 2;
            this.lbl_region.Text = "Аймагы/Территория";
            // 
            // end_date
            // 
            this.end_date.AutoSize = true;
            this.end_date.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.end_date.Location = new System.Drawing.Point(37, 68);
            this.end_date.Name = "end_date";
            this.end_date.Size = new System.Drawing.Size(149, 20);
            this.end_date.TabIndex = 1;
            this.end_date.Text = "Аяктоо күнү/Конец";
            // 
            // start_date
            // 
            this.start_date.AutoSize = true;
            this.start_date.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.start_date.Location = new System.Drawing.Point(20, 24);
            this.start_date.Name = "start_date";
            this.start_date.Size = new System.Drawing.Size(166, 20);
            this.start_date.TabIndex = 0;
            this.start_date.Text = "Баштоо күнү/Начало";
            // 
            // input_frame
            // 
            this.input_frame.Controls.Add(this.input_region);
            this.input_frame.Controls.Add(this.input_end);
            this.input_frame.Controls.Add(this.input_start);
            this.input_frame.Location = new System.Drawing.Point(268, 41);
            this.input_frame.Name = "input_frame";
            this.input_frame.Size = new System.Drawing.Size(211, 191);
            this.input_frame.TabIndex = 1;
            this.input_frame.TabStop = false;
            // 
            // input_region
            // 
            this.input_region.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.input_region.FormattingEnabled = true;
            this.input_region.Location = new System.Drawing.Point(0, 111);
            this.input_region.Name = "input_region";
            this.input_region.Size = new System.Drawing.Size(135, 28);
            this.input_region.TabIndex = 2;
            // 
            // input_end
            // 
            this.input_end.CustomFormat = "dd-MM-yy";
            this.input_end.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.input_end.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.input_end.Location = new System.Drawing.Point(0, 68);
            this.input_end.MaxDate = new System.DateTime(2029, 12, 31, 0, 0, 0, 0);
            this.input_end.MinDate = new System.DateTime(1999, 12, 31, 0, 0, 0, 0);
            this.input_end.Name = "input_end";
            this.input_end.Size = new System.Drawing.Size(135, 26);
            this.input_end.TabIndex = 1;
            // 
            // input_start
            // 
            this.input_start.CustomFormat = "dd-MM-yy";
            this.input_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.input_start.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.input_start.Location = new System.Drawing.Point(0, 24);
            this.input_start.MaxDate = new System.DateTime(2030, 1, 1, 0, 0, 0, 0);
            this.input_start.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.input_start.Name = "input_start";
            this.input_start.Size = new System.Drawing.Size(135, 26);
            this.input_start.TabIndex = 0;
            this.input_start.Value = new System.DateTime(2020, 6, 1, 0, 0, 0, 0);
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(52, 318);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(427, 31);
            this.progress.TabIndex = 2;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(52, 394);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(167, 45);
            this.btn_save.TabIndex = 3;
            this.btn_save.Text = "Сактоо/Сохранить";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // lbl_status
            // 
            this.lbl_status.AutoSize = true;
            this.lbl_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_status.Location = new System.Drawing.Point(49, 295);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(0, 20);
            this.lbl_status.TabIndex = 4;
            // 
            // lbl_percent
            // 
            this.lbl_percent.AutoSize = true;
            this.lbl_percent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_percent.Location = new System.Drawing.Point(427, 295);
            this.lbl_percent.Name = "lbl_percent";
            this.lbl_percent.Size = new System.Drawing.Size(0, 20);
            this.lbl_percent.TabIndex = 5;
            // 
            // btn_exit
            // 
            this.btn_exit.Location = new System.Drawing.Point(312, 394);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(167, 45);
            this.btn_exit.TabIndex = 6;
            this.btn_exit.Text = "Чыгуу/Выход";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 450);
            this.Controls.Add(this.btn_exit);
            this.Controls.Add(this.lbl_percent);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.input_frame);
            this.Controls.Add(this.label_frame);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form";
            this.Text = "1 Trud";
            this.label_frame.ResumeLayout(false);
            this.label_frame.PerformLayout();
            this.input_frame.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox label_frame;
        private System.Windows.Forms.GroupBox input_frame;
        private System.Windows.Forms.Label end_date;
        private System.Windows.Forms.Label start_date;
        private System.Windows.Forms.DateTimePicker input_start;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.DateTimePicker input_end;
        private System.Windows.Forms.Label lbl_region;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.Label lbl_percent;
        private System.Windows.Forms.ComboBox input_region;
        private System.Windows.Forms.Button btn_exit;
    }
}


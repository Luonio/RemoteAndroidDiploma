namespace WinFormTry_1
{
    partial class MenuItemControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.iconBox = new System.Windows.Forms.PictureBox();
            this.itemNameBox = new System.Windows.Forms.Label();
            this.funcNameTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.iconBox)).BeginInit();
            this.SuspendLayout();
            // 
            // iconBox
            // 
            this.iconBox.Location = new System.Drawing.Point(3, 3);
            this.iconBox.Name = "iconBox";
            this.iconBox.Size = new System.Drawing.Size(144, 144);
            this.iconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.iconBox.TabIndex = 0;
            this.iconBox.TabStop = false;
            this.iconBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MenuItemControl_MouseDown);
            this.iconBox.MouseEnter += new System.EventHandler(this.MenuItemControl_MouseEnter);
            this.iconBox.MouseLeave += new System.EventHandler(this.MenuItemControl_MouseLeave);
            this.iconBox.MouseHover += new System.EventHandler(this.iconBox_MouseHover);
            this.iconBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MenuItemControl_MouseUp);
            // 
            // itemNameBox
            // 
            this.itemNameBox.AutoSize = true;
            this.itemNameBox.Location = new System.Drawing.Point(153, 68);
            this.itemNameBox.Name = "itemNameBox";
            this.itemNameBox.Size = new System.Drawing.Size(0, 13);
            this.itemNameBox.TabIndex = 1;
            this.itemNameBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MenuItemControl_MouseDown);
            this.itemNameBox.MouseEnter += new System.EventHandler(this.MenuItemControl_MouseEnter);
            this.itemNameBox.MouseLeave += new System.EventHandler(this.MenuItemControl_MouseLeave);
            this.itemNameBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MenuItemControl_MouseUp);
            // 
            // funcNameTip
            // 
            this.funcNameTip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // MenuItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.itemNameBox);
            this.Controls.Add(this.iconBox);
            this.Name = "MenuItemControl";
            this.Size = new System.Drawing.Size(485, 150);
            this.Load += new System.EventHandler(this.MenuItemControl_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MenuItemControl_MouseDown);
            this.MouseEnter += new System.EventHandler(this.MenuItemControl_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.MenuItemControl_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MenuItemControl_MouseUp);
            this.Resize += new System.EventHandler(this.MenuItemControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.iconBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox iconBox;
        public System.Windows.Forms.Label itemNameBox;
        private System.Windows.Forms.ToolTip funcNameTip;
    }
}

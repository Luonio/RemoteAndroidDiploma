namespace WinFormTry_1
{
    partial class AltActionsControl
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
            ((System.ComponentModel.ISupportInitialize)(this.iconBox)).BeginInit();
            this.SuspendLayout();
            // 
            // iconBox
            // 
            this.iconBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AltActionsControl_MouseClick);
            // 
            // itemNameBox
            // 
            this.itemNameBox.Location = new System.Drawing.Point(153, 73);
            this.itemNameBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AltActionsControl_MouseClick);
            // 
            // AltActionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "AltActionsControl";
            this.Load += new System.EventHandler(this.AltActionsControl_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AltActionsControl_MouseClick);
            ((System.ComponentModel.ISupportInitialize)(this.iconBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

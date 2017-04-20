namespace WinFormTry_1
{
    partial class FormBorders
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
            this.SuspendLayout();
            // 
            // FormBorders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "FormBorders";
            this.Load += new System.EventHandler(this.FormHeader_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormBorder_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormBorder_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormBorder_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}

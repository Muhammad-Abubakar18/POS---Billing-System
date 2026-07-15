namespace DrMusa.Desktop;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.btnTestReceipt = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // btnTestReceipt
        // 
        this.btnTestReceipt.Location = new System.Drawing.Point(50, 50);
        this.btnTestReceipt.Name = "btnTestReceipt";
        this.btnTestReceipt.Size = new System.Drawing.Size(150, 40);
        this.btnTestReceipt.TabIndex = 0;
        this.btnTestReceipt.Text = "Test Receipt Preview";
        this.btnTestReceipt.UseVisualStyleBackColor = true;
        this.btnTestReceipt.Click += new System.EventHandler(this.btnTestReceipt_Click);
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.btnTestReceipt);
        this.Name = "Form1";
        this.Text = "POS Preview Test";
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Button btnTestReceipt;
}

namespace HATE
{
    partial class HATE
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HATE));
            this.button_Corrupt = new System.Windows.Forms.Button();
            this.checkBox_ShuffleText = new System.Windows.Forms.CheckBox();
            this.checkBox_ShuffleGFX = new System.Windows.Forms.CheckBox();
            this.checkBox_HitboxFix = new System.Windows.Forms.CheckBox();
            this.checkBox_ShuffleFont = new System.Windows.Forms.CheckBox();
            this.checkBox_ShuffleBG = new System.Windows.Forms.CheckBox();
            this.checkBox_ShuffleAudio = new System.Windows.Forms.CheckBox();
            this.checkBox_ShowSeed = new System.Windows.Forms.CheckBox();
            this.textBox_Seed = new System.Windows.Forms.TextBox();
            this.textBox_Power = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Launch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_Corrupt
            // 
            this.button_Corrupt.BackColor = System.Drawing.Color.Black;
            this.button_Corrupt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Corrupt.Font = new System.Drawing.Font("Papyrus", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Corrupt.ForeColor = System.Drawing.Color.Coral;
            this.button_Corrupt.Location = new System.Drawing.Point(20, 325);
            this.button_Corrupt.Name = "button_Corrupt";
            this.button_Corrupt.Size = new System.Drawing.Size(147, 25);
            this.button_Corrupt.TabIndex = 0;
            this.button_Corrupt.Text = "-CORRUPT-";
            this.button_Corrupt.UseVisualStyleBackColor = false;
            this.button_Corrupt.Click += new System.EventHandler(this.button_Corrupt_Clicked);
            // 
            // checkBox_ShuffleText
            // 
            this.checkBox_ShuffleText.AutoSize = true;
            this.checkBox_ShuffleText.BackColor = System.Drawing.Color.Black;
            this.checkBox_ShuffleText.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.checkBox_ShuffleText.FlatAppearance.BorderSize = 0;
            this.checkBox_ShuffleText.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBox_ShuffleText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox_ShuffleText.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_ShuffleText.ForeColor = System.Drawing.Color.White;
            this.checkBox_ShuffleText.Location = new System.Drawing.Point(14, 197);
            this.checkBox_ShuffleText.Name = "checkBox_ShuffleText";
            this.checkBox_ShuffleText.Size = new System.Drawing.Size(150, 31);
            this.checkBox_ShuffleText.TabIndex = 1;
            this.checkBox_ShuffleText.Text = "Shuffle Text";
            this.checkBox_ShuffleText.UseVisualStyleBackColor = false;
            this.checkBox_ShuffleText.CheckedChanged += new System.EventHandler(this.checkBox_ShuffleText_CheckedChanged);
            // 
            // checkBox_ShuffleGFX
            // 
            this.checkBox_ShuffleGFX.AutoSize = true;
            this.checkBox_ShuffleGFX.BackColor = System.Drawing.Color.Black;
            this.checkBox_ShuffleGFX.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.checkBox_ShuffleGFX.FlatAppearance.BorderSize = 0;
            this.checkBox_ShuffleGFX.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBox_ShuffleGFX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox_ShuffleGFX.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_ShuffleGFX.ForeColor = System.Drawing.Color.White;
            this.checkBox_ShuffleGFX.Location = new System.Drawing.Point(14, 160);
            this.checkBox_ShuffleGFX.Name = "checkBox_ShuffleGFX";
            this.checkBox_ShuffleGFX.Size = new System.Drawing.Size(172, 31);
            this.checkBox_ShuffleGFX.TabIndex = 2;
            this.checkBox_ShuffleGFX.Text = "Shuffle Sprites";
            this.checkBox_ShuffleGFX.UseVisualStyleBackColor = false;
            this.checkBox_ShuffleGFX.CheckedChanged += new System.EventHandler(this.checkBox_ShuffleGFX_CheckedChanged);
            // 
            // checkBox_HitboxFix
            // 
            this.checkBox_HitboxFix.AutoSize = true;
            this.checkBox_HitboxFix.BackColor = System.Drawing.Color.Black;
            this.checkBox_HitboxFix.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.checkBox_HitboxFix.FlatAppearance.BorderSize = 0;
            this.checkBox_HitboxFix.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBox_HitboxFix.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox_HitboxFix.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_HitboxFix.ForeColor = System.Drawing.Color.White;
            this.checkBox_HitboxFix.Location = new System.Drawing.Point(13, 123);
            this.checkBox_HitboxFix.Name = "checkBox_HitboxFix";
            this.checkBox_HitboxFix.Size = new System.Drawing.Size(125, 31);
            this.checkBox_HitboxFix.TabIndex = 3;
            this.checkBox_HitboxFix.Text = "Hitbox Fix";
            this.checkBox_HitboxFix.UseVisualStyleBackColor = false;
            this.checkBox_HitboxFix.CheckedChanged += new System.EventHandler(this.checkBox_HitboxFix_CheckedChanged);
            // 
            // checkBox_ShuffleFont
            // 
            this.checkBox_ShuffleFont.AutoSize = true;
            this.checkBox_ShuffleFont.BackColor = System.Drawing.Color.Black;
            this.checkBox_ShuffleFont.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.checkBox_ShuffleFont.FlatAppearance.BorderSize = 0;
            this.checkBox_ShuffleFont.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBox_ShuffleFont.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox_ShuffleFont.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_ShuffleFont.ForeColor = System.Drawing.Color.White;
            this.checkBox_ShuffleFont.Location = new System.Drawing.Point(13, 86);
            this.checkBox_ShuffleFont.Name = "checkBox_ShuffleFont";
            this.checkBox_ShuffleFont.Size = new System.Drawing.Size(156, 31);
            this.checkBox_ShuffleFont.TabIndex = 4;
            this.checkBox_ShuffleFont.Text = "Shuffle Fonts";
            this.checkBox_ShuffleFont.UseVisualStyleBackColor = false;
            this.checkBox_ShuffleFont.CheckedChanged += new System.EventHandler(this.checkBox_ShuffleFont_CheckedChanged);
            // 
            // checkBox_ShuffleBG
            // 
            this.checkBox_ShuffleBG.AutoSize = true;
            this.checkBox_ShuffleBG.BackColor = System.Drawing.Color.Black;
            this.checkBox_ShuffleBG.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.checkBox_ShuffleBG.FlatAppearance.BorderSize = 0;
            this.checkBox_ShuffleBG.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBox_ShuffleBG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox_ShuffleBG.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_ShuffleBG.ForeColor = System.Drawing.Color.White;
            this.checkBox_ShuffleBG.Location = new System.Drawing.Point(13, 49);
            this.checkBox_ShuffleBG.Name = "checkBox_ShuffleBG";
            this.checkBox_ShuffleBG.Size = new System.Drawing.Size(145, 31);
            this.checkBox_ShuffleBG.TabIndex = 5;
            this.checkBox_ShuffleBG.Text = "Shuffle GFX";
            this.checkBox_ShuffleBG.UseVisualStyleBackColor = false;
            this.checkBox_ShuffleBG.CheckedChanged += new System.EventHandler(this.checkBox_ShuffleBG_CheckedChanged);
            // 
            // checkBox_ShuffleAudio
            // 
            this.checkBox_ShuffleAudio.AutoSize = true;
            this.checkBox_ShuffleAudio.BackColor = System.Drawing.Color.Black;
            this.checkBox_ShuffleAudio.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.checkBox_ShuffleAudio.FlatAppearance.BorderSize = 0;
            this.checkBox_ShuffleAudio.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBox_ShuffleAudio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox_ShuffleAudio.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_ShuffleAudio.ForeColor = System.Drawing.Color.White;
            this.checkBox_ShuffleAudio.Location = new System.Drawing.Point(13, 12);
            this.checkBox_ShuffleAudio.Name = "checkBox_ShuffleAudio";
            this.checkBox_ShuffleAudio.Size = new System.Drawing.Size(156, 31);
            this.checkBox_ShuffleAudio.TabIndex = 6;
            this.checkBox_ShuffleAudio.Text = "Shuffle Audio";
            this.checkBox_ShuffleAudio.UseVisualStyleBackColor = false;
            this.checkBox_ShuffleAudio.CheckedChanged += new System.EventHandler(this.checkBox_ShuffleAudio_CheckedChanged);
            // 
            // checkBox_ShowSeed
            // 
            this.checkBox_ShowSeed.AutoSize = true;
            this.checkBox_ShowSeed.BackColor = System.Drawing.Color.Black;
            this.checkBox_ShowSeed.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.checkBox_ShowSeed.FlatAppearance.BorderSize = 0;
            this.checkBox_ShowSeed.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBox_ShowSeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox_ShowSeed.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_ShowSeed.ForeColor = System.Drawing.Color.White;
            this.checkBox_ShowSeed.Location = new System.Drawing.Point(14, 234);
            this.checkBox_ShowSeed.Name = "checkBox_ShowSeed";
            this.checkBox_ShowSeed.Size = new System.Drawing.Size(129, 31);
            this.checkBox_ShowSeed.TabIndex = 7;
            this.checkBox_ShowSeed.Text = "Show Seed";
            this.checkBox_ShowSeed.UseVisualStyleBackColor = false;
            this.checkBox_ShowSeed.CheckedChanged += new System.EventHandler(this.checkBox_ShowSeed_CheckedChanged);
            // 
            // textBox_Seed
            // 
            this.textBox_Seed.BackColor = System.Drawing.Color.White;
            this.textBox_Seed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_Seed.ForeColor = System.Drawing.Color.Black;
            this.textBox_Seed.Location = new System.Drawing.Point(67, 273);
            this.textBox_Seed.Name = "textBox_Seed";
            this.textBox_Seed.Size = new System.Drawing.Size(100, 20);
            this.textBox_Seed.TabIndex = 8;
            // 
            // textBox_Power
            // 
            this.textBox_Power.BackColor = System.Drawing.Color.White;
            this.textBox_Power.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_Power.ForeColor = System.Drawing.Color.Black;
            this.textBox_Power.Location = new System.Drawing.Point(67, 299);
            this.textBox_Power.Name = "textBox_Power";
            this.textBox_Power.Size = new System.Drawing.Size(100, 20);
            this.textBox_Power.TabIndex = 9;
            this.textBox_Power.Text = "0 - 255";
            this.textBox_Power.Enter += new System.EventHandler(this.textBox_Power_Enter);
            this.textBox_Power.Leave += new System.EventHandler(this.textBox_Power_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Papyrus", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(8, 273);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 22);
            this.label1.TabIndex = 10;
            this.label1.Text = "Seed:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Papyrus", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(8, 299);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 22);
            this.label2.TabIndex = 11;
            this.label2.Text = "Power:";
            // 
            // button_Launch
            // 
            this.button_Launch.BackColor = System.Drawing.Color.Black;
            this.button_Launch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Launch.Font = new System.Drawing.Font("Papyrus", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Launch.ForeColor = System.Drawing.Color.Fuchsia;
            this.button_Launch.Location = new System.Drawing.Point(20, 356);
            this.button_Launch.Name = "button_Launch";
            this.button_Launch.Size = new System.Drawing.Size(147, 25);
            this.button_Launch.TabIndex = 12;
            this.button_Launch.Text = "-LAUNCH UT-";
            this.button_Launch.UseVisualStyleBackColor = false;
            this.button_Launch.Click += new System.EventHandler(this.button_Launch_Clicked);
            // 
            // HATE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(184, 390);
            this.Controls.Add(this.button_Launch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Power);
            this.Controls.Add(this.textBox_Seed);
            this.Controls.Add(this.checkBox_ShowSeed);
            this.Controls.Add(this.checkBox_ShuffleAudio);
            this.Controls.Add(this.checkBox_ShuffleBG);
            this.Controls.Add(this.checkBox_ShuffleFont);
            this.Controls.Add(this.checkBox_HitboxFix);
            this.Controls.Add(this.checkBox_ShuffleGFX);
            this.Controls.Add(this.checkBox_ShuffleText);
            this.Controls.Add(this.button_Corrupt);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 310);
            this.Name = "HATE";
            this.Text = "HATE";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Corrupt;
        private System.Windows.Forms.CheckBox checkBox_ShuffleText;
        private System.Windows.Forms.CheckBox checkBox_ShuffleGFX;
        private System.Windows.Forms.CheckBox checkBox_HitboxFix;
        private System.Windows.Forms.CheckBox checkBox_ShuffleFont;
        private System.Windows.Forms.CheckBox checkBox_ShuffleBG;
        private System.Windows.Forms.CheckBox checkBox_ShuffleAudio;
        private System.Windows.Forms.CheckBox checkBox_ShowSeed;
        private System.Windows.Forms.TextBox textBox_Seed;
        private System.Windows.Forms.TextBox textBox_Power;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Launch;
    }
}


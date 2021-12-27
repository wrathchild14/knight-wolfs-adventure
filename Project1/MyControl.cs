using Microsoft.Xna.Framework;
using MonoGame.UI.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1
{
    class MyControl : ControlManager
    {
        public bool play;

        public MyControl(Game game) : base(game)
        {
        }

        public override void InitializeComponent()
        {
            var btn = new Button()
            {
                Text = "Start game",
                Size = new Vector2(200, 50),
                BackgroundColor = Color.DarkGreen,
                Location = new Vector2(300, 200)
            };

            btn.Clicked += btn_clicked;
            Controls.Add(btn);
        }

        private void btn_clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            this.play = true;
            this.Visible = false;
        }
    }
}

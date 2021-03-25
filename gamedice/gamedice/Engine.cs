using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace gamedice
{
    public class Engine
    {
        public Enemy[] enemes = new Enemy[6];
        public Character charh;
        public Dice dc = new Dice();
        public Label choose, spell;
        public Label[] status = new Label[2];
        public Font font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, (204));
        public int width = 1000, height = 600;

        public int state = 4; //0 = end_turn, 1 = attack, 2 = spell, 3 = shield, 4 = null
        public int dice = 6, en_count = 0, deads, st_roll;
        //Debug
        public bool debug = false;
        public Label _debug = new Label();
        //Dead
        Button retry, exit;
        Label dead;

        void _Create()
        {
            choose = new Label();
            choose.Location = new Point(width / 13 * 6, height - 230);
            choose.Font = font;
            choose.AutoSize = true;

            spell = new Label();
            spell.Location = new Point(width * 5 / 8, height - 230);
            spell.Font = font;
            spell.AutoSize = true;

            charh = new Character(new Hero());
            charh.attack.Click += b_click;
            charh.shield.Click += b_click;
            charh.spell.Click += b_click;

            for (int i = 0; i < enemes.Length; i++)
            {
                enemes[i] = new Enemy(i, new En_Factory());
                enemes[i].dead = true;
                enemes[i].en.Click += En_Click;
            }

            status[0] = new Label();
            status[0].Location = new Point(width * 5 / 8, enemes[0].en.Size.Height + enemes[0].en.Location.Y + 30);
            status[0].Font = font;
            status[0].AutoSize = true;

            status[1] = new Label();
            status[1].Location = new Point(enemes[0].en.Location.X, enemes[0].en.Size.Height + enemes[0].en.Location.Y + 30);
            status[1].Font = font;
            status[1].AutoSize = true;
        }
        public void _Spawn(bool start)
        {
            en_count = dc.diceroll(6);
            deads = 0;
            if (start)
            {
                Form.ActiveForm.Controls.Clear();
                _Create();
                int _hp = dc.diceroll(dice);
                int _attack = dc.diceroll(dice);
                charh.Add(charh);
                if (debug)
                    charh.Dispose(charh);
                Form.ActiveForm.Controls.Add(status[0]);
                Form.ActiveForm.Controls.Add(status[1]);
                Form.ActiveForm.Controls.Add(choose);
                Form.ActiveForm.Controls.Add(spell);
            }
            else
                charh._LSt();
            status[0].Text = null;
            status[1].Text = null;
            choose.Text = null;
            spell.Text = null;
            for (int i = 0; i < en_count; i++)
            {
                int roll = dc.diceroll(2);
                switch(roll)
                {
                    case 1:
                        enemes[i].f = new En_Factory();
                        break;
                    case 2:
                        enemes[i].f = new En_DMG();
                        break;
                }
                enemes[i].Add(charh);
            }
            st_roll = dc.diceroll(2);
            if (debug)
            {
                Form.ActiveForm.Controls.Add(_debug);
                _debug.AutoSize = true;
                en_count = 6;
            }
        }
        void Ch_Dead()
        {
            if (charh.hp <= 0)
            {
                //Form.ActiveForm.Controls.Clear();
                for(int i = 0; i < enemes.Length; i++)
                {
                    enemes[i]._EnRemove();
                }
                Form.ActiveForm.Controls.Remove(charh.attack);
                Form.ActiveForm.Controls.Remove(charh.shield);
                Form.ActiveForm.Controls.Remove(charh.spell);
                Form.ActiveForm.Controls.Remove(charh.l);
                Form.ActiveForm.Controls.Remove(charh.st);
                for (int i = 0; i < charh.states.Count; i++)
                    Form.ActiveForm.Controls.Remove(charh.states[i]);

                dead = new Label();
                dead.Text = "You dead";
                dead.Font = new Font("Impact", 17F, FontStyle.Regular, GraphicsUnit.Point, 204);
                dead.ForeColor = Color.Red;
                dead.AutoSize = true;
                dead.Location = new Point(width / 2 - 50, height / 2 - 20);

                retry = new Button();
                retry.Text = "RETRY";
                retry.Font = new Font("Impact", 17F, FontStyle.Regular, GraphicsUnit.Point, 204);
                retry.ForeColor = Color.DarkBlue;
                retry.AutoSize = true;
                retry.Click += b_click;
                retry.Location = new Point(width / 2 - 95, height / 2 + 20);

                exit = new Button();
                exit.Text = "EXIT";
                exit.Font = new Font("Impact", 17F, FontStyle.Regular, GraphicsUnit.Point, 204);
                retry.ForeColor = Color.Blue;
                exit.AutoSize = true;
                exit.Click += b_click;
                exit.Location = new Point(width / 2 + 20, height / 2 + 20);

                if (Form.ActiveForm != null)
                {
                    Form.ActiveForm.Controls.Add(retry);
                    Form.ActiveForm.Controls.Add(exit);
                    Form.ActiveForm.Controls.Add(dead);
                }
            }
        }
        private void En_Click(object sender, EventArgs e)
        {
            PictureBox en = (PictureBox)sender;
            switch (state)
            {
                case 1:
                    if (state != 0)
                    {
                        status[0].Text = null;
                        status[1].Text = null;
                        choose.Text = null;
                        spell.Text = null;
                        _Dmg(charh, (Actor)en.Tag);
                        EN_Attack();
                    }
                    break;
                case 2:
                    if (state != 0)
                    {
                        status[0].Text = null;
                        status[1].Text = null;
                        choose.Text = null;
                        spell.Text = null;
                        _Spl(charh, (Actor)en.Tag);
                        _StUpd();
                        EN_Attack();
                    }
                    break;
            }
        }
        private void EN_Attack()
        {
            //Enemy Turn
            for (int i = 0; i < enemes.Length; i++)
            {
                if (!enemes[i].dead)
                {
                    En_Roll(i);
                    for (int j = 0; j < enemes[i].state.Count; j++)
                    {
                        if (enemes[i].state[j].time > 0)
                        {
                            switch (enemes[i].state[j].effect)
                            {
                                case 0:
                                    status[1].Text += "Враг получил " + enemes[i].state[j].time + " урона Ядом\n";
                                    break;
                                case 1:
                                    status[1].Text += "Враг получил " + enemes[i].state[j].time + " урона Кровотечением\n";
                                    break;
                            }
                            enemes[i].Request(j);
                        }
                    }
                }
            }
            for (int i = 0; i < enemes.Length; i++)
            {
                if (!enemes[i].dead)
                {
                    if (enemes[i].hp <= 0)
                    {
                        enemes[i].dead = true;
                        enemes[i]._EnRemove();
                        charh.exp += 1;
                    }
                }
            }
            // HeroTurn
            for (int j = 0; j < charh.state.Count; j++)
            {
                if (charh.state[j].time > 0)
                {
                    switch (charh.state[j].effect)
                    {
                        case 0:
                            status[1].Text += "Герой получил " + charh.state[j].time + " урона Ядом\n";
                            break;
                        case 1:
                            status[1].Text += "Герой получил " + charh.state[j].time + " урона Кровотечением\n";
                            break;
                    }
                    if(!debug)
                        charh.Request(j);
                }
            }
            charh._LSt();
            //End
            _StUpd();
            _Lvl();
            st_roll = dc.diceroll(2);

            for (int i = 0; i < enemes.Length; i++)
                if (enemes[i].dead)
                    deads += 1;
            if (deads == enemes.Length)
                _Spawn(false);
            else
                deads = 0;

            Ch_Dead();

            //Debug
            if (debug)
            {
                for (int i = 0; i < enemes[0].states.Count; i++)
                    _debug.Text += " " + enemes[0].states[i] + " ";
            }
        }
        private void En_Roll(int i)
        {
            int roll = dc.diceroll(3);
            switch(roll)
            {
                case 1:
                    _Dmg(enemes[i], charh);
                    break;
                case 2:
                    int _roll = dc.diceroll(dice);
                    int def = _roll / 3 * (enemes[i].lvl + 1);
                    status[0].Text += "Враг получил " + def + " брони\n";
                    enemes[i].Defence(def, roll);
                    break;
                case 3:
                    st_roll = dc.diceroll(2);
                    _Spl(enemes[i], charh);
                    break;
            }
        }
        private void b_click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string b_name = btn.Text;
            switch (b_name)
            {
                case "ATTACK":
                    state = 1;
                    choose.Text = "Выберите врага";
                    break;
                case "SPELL":
                    state = 2;
                    switch(st_roll)
                    {
                        case 1:
                            spell.Text = "Яд";
                            break;
                        case 2:
                            spell.Text = "бафф Кровотечение";
                            break;
                    }
                    choose.Text = "Выберите врага";
                    //Debug
                    if(debug)
                        _debug.Text = "";
                    break;
                case "SHIELD":
                    status[0].Text = null;
                    status[1].Text = null;
                    choose.Text = null;
                    spell.Text = null;
                    int _roll = dc.diceroll(dice);
                    int def = _roll * 2 * charh.lvl; //_roll / 3 * (charh.lvl + 1);
                    status[0].Text = "Герой получил " + def + " брони\n";
                    charh.Defence(def, _roll);
                    charh._LSt();
                    EN_Attack();
                    break;

                case "RETRY":
                    _Spawn(true);
                    charh.exp = 0;
                    charh.dead = false;
                    break;
                case "EXIT":
                    Application.Exit();
                    break;
            }
        }
        void _Lvl()
        {
            int roll;
            if (charh.exp == 6)
            {
                for (int i = 0; i < 3; i++)
                {
                    roll = dc.diceroll(2);
                    switch (roll)
                    {
                        case 1:
                            charh.max_hp += 1;
                            break;
                        case 2:
                            charh.dmg += 1;
                            break;
                    }
                }
                charh.lvl += 1;
                charh.hp = charh.max_hp;
                charh.exp = 0;
            }
        }
        void _Dmg(Actor c, Actor a)
        {
            int dmg = 0;
            int roll = dc.diceroll(dice);
            int r_st = 0;

            if (roll == 1)
                r_st = 1;
            if (roll == dice)
                r_st = 2;
            if (roll > 1 && roll < dice)
                r_st = 3;

            switch(r_st)
            {
                case 1:
                    if (!debug)
                        dmg = -1;
                    status[0].Text += "Критический промах" + "\n";
                    break;
                case 2:
                    dmg = c.dmg * 2;
                    status[0].Text += c.name + " критически атаковал " + a.name + " на " + dmg + "\n";
                    break;
                case 3:
                    float _dmg = ((float)roll + (float)c.lvl) / (float)dice * (float)c.dmg;
                    dmg = (int)_dmg;
                    status[0].Text += c.name + " атаковал " + a.name + " на " + dmg.ToString() + "\n";
                    break;
            }
            if (a.name == "Герой" && debug)
                dmg = 0;
            c.DMG(a, dmg);
            if (a.hp < 0)
                a.hp = 0;
            a._LSt();
            c._LSt();
        }
        void _Spl(Actor sender, Actor receiver)
        {
            int roll = dc.diceroll(dice) / 3 * sender.lvl;
            switch (st_roll)
            {
                case 1:
                    status[1].Text += sender.name + " наложил на " + receiver.name + " " + roll.ToString() + " Яд\n";
                    receiver._ChngSt(new Poison(), roll);
                    break;
                case 2:
                    status[1].Text += sender.name + " наложил на себя " + roll.ToString() + " бафф Кровотечения\n";
                    sender._ChngSt(new Bleed_Act(sender), roll);
                    break;
            }
            sender._LSt();
            receiver._LSt();
        }
        private void _StUpd()
        {
            if (Form.ActiveForm != null)
            {
                if(!charh.dead)
                {
                    for (int j = 0; j < charh.state.Count; j++)
                    {
                        if (charh.state[j].time != 0)
                        {
                            Form.ActiveForm.Controls.Add(charh.states[j]);
                            charh.states[j].Text = charh.state[j].time.ToString();
                        }
                        else
                            Form.ActiveForm.Controls.Remove(charh.states[j]);
                    }
                }
                for (int i = 0; i < en_count; i++)
                    for (int j = 0; j < enemes[i].state.Count; j++)
                        if (!enemes[i].dead)
                        {
                            if (enemes[i].state[j].time != 0)
                            {
                                Form.ActiveForm.Controls.Add(enemes[i].states[j]);
                                enemes[i].states[j].Text = enemes[i].state[j].time.ToString();
                            }
                            else
                                Form.ActiveForm.Controls.Remove(enemes[i].states[j]);
                        }
            }
        }
    }

    public class Dice
    {
        Random r = new Random();
        public int diceroll(int n)
        {
            int j = r.Next(1, n + 1);
            return j;
        }
    }
}
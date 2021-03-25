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
    public abstract class DMG
    {
        public abstract void Hit(Actor sender, Actor receiver, int _dmg);
    }
    public abstract class DEF
    {
        public abstract void Defence(Actor sender, int _def, int roll);
    }
    public class Norm_DEF : DEF
    {
        public override void Defence(Actor sender, int _def, int roll)
        {
            sender.def += _def;
            if(roll == 6)
            {
                for(int i = 0; i < sender.state.Count; i++)
                {
                    if (sender.state[i].effect == 0 || sender.state[i].effect == 1)
                        sender.state[i].time = 0;
                }
            }
            if (roll == 5)
            {
                for (int i = 0; i < sender.state.Count; i++)
                {
                    if (sender.state[i].effect == 0 || sender.state[i].effect == 1)
                        sender.state[i].time = 1;
                }
            }
        }
    }
    public class Norm_DMG : DMG
    {
        public override void Hit(Actor sender, Actor receiver, int _dmg)
        {
            if (_dmg == -1)
            {
                sender.hp = sender.hp - sender.dmg + sender.lvl;
            }
            else
            {
                int t_def = receiver.def - _dmg;
                if (t_def <= 0)
                {
                    receiver.def = 0;
                    receiver.hp += t_def;
                }
                else
                    receiver.def -= _dmg;
            }
        }
    }
    public class Bleed_DMG : DMG
    {
        public override void Hit(Actor sender, Actor receiver, int _dmg)
        {
            int t_def = 0;
            if (_dmg == -1)
            {
                sender.hp = sender.hp - sender.dmg + sender.lvl;
            }
            else
            {
                t_def = receiver.def - _dmg;
                if (t_def <= 0)
                {
                    receiver.def = 0;
                    receiver.hp += t_def;
                }
                else
                    receiver.def -= _dmg;
            }
            if (t_def < 0)
            {
                int count = (0 - t_def) / 3;
                receiver._ChngSt(new Bleed(), count);
            }
        }
    }
    public class Enemy : Actor
    {
        Font font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
        public Enemy(int i, Factory _f)
        {
            width = x / 6;
            int space = width / 5;

            f = _f;

            name = "Враг";

            en = new PictureBox();
            en.Size = new Size(width - space, y / 6);
            int result = (i == 0) ? space / 2 : width * i;
            en.Location = new Point(result, 130);
            en.BackColor = Color.DarkRed;
            en.Tag = this;

            l.Font = font;
            l.Location = new Point(result, 40);

            st.Font = font;
            st.Location = new Point(result + 100, 40);
        }
        public override void Add(Actor h)
        {
            dead = false;
            Dispose(h);
            Form.ActiveForm.Controls.Add(en);
            Form.ActiveForm.Controls.Add(l);
            Form.ActiveForm.Controls.Add(st);
            _LSt();
        }
        public override void _ChngSt(State st, int count)
        {
            bool add = true;
            for (int i = 0; i < state.Count; i++)
            {
                if (state[i].effect == st.effect)
                {
                    state[i].time += count;
                    add = false;
                }
            }
            if (add)
            {
                state.Add(st);
                state[state.Count - 1].time += count;
                states.Add(st.state);
                states[state.Count - 1].Font = font;
            }
            for(int i = 0; i < state.Count; i++)
                states[i].Location = new Point(St_Loc(i), en.Location.Y + en.Size.Height + 5);

        }
        private int St_Loc(int i)
        {
            switch (i)
            {
                case 0:
                    return en.Location.X;
                case 1:
                    return en.Location.X + states[0].Size.Width + 3;
                case 2:
                    return states[1].Location.X + states[0].Size.Width + 3;
            }
            return 0;
        }
        public void _EnRemove()
        {
            dead = true;
            if (Form.ActiveForm != null)
            {
                Form.ActiveForm.Controls.Remove(en);
                Form.ActiveForm.Controls.Remove(l);
                Form.ActiveForm.Controls.Remove(st);
                for (int i = 0; i < states.Count; i++)
                    Form.ActiveForm.Controls.Remove(states[i]);
                states.Clear();
            }
        }
    }
    public class Character : Actor
    {
        public Button attack, shield, spell;
        public int exp = 0;
        Font font = new Font("Consolas", 18F, FontStyle.Regular, GraphicsUnit.Point, 204);
        public Character(Factory _f)
        {
            width = x / 4;
            f = _f;
            lvl = 1;
            name = "Герой";

            shield = new Button();
            shield.Text = "SHIELD";
            shield.Font = font;
            shield.BackColor = Color.DarkGray;
            shield.Size = new Size(width, 150);
            shield.Location = new Point(0, y - 150 - 39);

            attack = new Button();
            attack.Text = "ATTACK";
            attack.Font = font;
            attack.BackColor = Color.Red;
            attack.Size = new Size(width, 150);
            attack.Location = new Point(shield.Location.X + width + 20, y - 150 - 39);

            spell = new Button();
            spell.Text = "SPELL";
            spell.Font = font;
            spell.BackColor = Color.Blue;
            spell.Size = new Size(width, 150);
            spell.Location = new Point(attack.Location.X + width + 20, y - 150 - 39);

            l.Font = font;
            l.Location = new Point(spell.Location.X + width, spell.Location.Y);

            st.Font = font;
            st.Location = new Point(l.Location.X + 120, spell.Location.Y);
        }
        public override void Add(Actor h)
        {
            dead = false;
            Dispose(h);
            Form.ActiveForm.Controls.Add(attack);
            Form.ActiveForm.Controls.Add(shield);
            Form.ActiveForm.Controls.Add(spell);
            Form.ActiveForm.Controls.Add(l);
            Form.ActiveForm.Controls.Add(st);
            _LSt();
        }
        public override void _ChngSt(State st, int count)
        {
            bool add = true;
            for (int i = 0; i < state.Count; i++)
            {
                if (state[i].effect == st.effect)
                {
                    state[i].time += count;
                    add = false;
                }
            }
            if (add)
            {
                state.Add(st);
                state[state.Count - 1].time += count;
                states.Add(st.state);
                states[state.Count - 1].Font = font;
                for (int i = 0; i < state.Count; i++)
                    states[i].Location = new Point(St_Loc(i), shield.Location.Y - 35);
            }

        }
        private int St_Loc(int i)
        {
            switch (i)
            {
                case 0:
                    return shield.Location.X;
                case 1:
                    return shield.Location.X + states[0].Size.Width + 3;
                case 2:
                    return states[1].Location.X + states[0].Size.Width + 3;
            }
            return 0;
        }
    }
    public abstract class Actor
    {
        public bool dead = false;
        public List<State> state;
        public List<Label> states;
        public int x = 1000, y = 600, width;
        public int max_hp, hp, def, dmg, lvl;
        public string name;
        public Label l, st;
        public PictureBox en;
        public DMG f_dmg = new Norm_DMG();
        public DEF f_def = new Norm_DEF();
        public Factory f;
        public Actor()
        {
            l = new Label();
            l.AutoSize = true;
            l.Text = "Здоровье\nБроня\nУрон\nУровень";

            st = new Label();
            st.AutoSize = true;
            st.Text = hp + "\n" + def + "\n" + dmg + "\n" + lvl;

            state = new List<State>();
            states = new List<Label>();
        }
        public void _LSt()
        {
            st.Text = hp + "\n" + def + "\n" + dmg + "\n" + lvl;
        }
        public void Dispose(Actor h)
        {
            f.Dispose(this, h);
            state.Clear();
        }
        public abstract void _ChngSt(State st, int count);
        public void Request(int j)
        {
            state[j].Handle(this);
            if (state[j].time == 0)
            {
                Form.ActiveForm.Controls.Remove(states[j]);
                states.RemoveAt(j);
                state.RemoveAt(j);
            }
            
        }
        public void DMG(Actor a, int _dmg)
        {
            f_dmg.Hit(this, a, _dmg);
        }
        public void Defence(int _def, int roll)
        {
            f_def.Defence(this, _def, roll);
        }
        public abstract void Add(Actor h);
    }
    public abstract class State
    {
        public int effect, time;
        public Label state;
        public abstract void Handle(Actor a);
    }
    public class Poison : State
    {
        public Poison()
        {
            effect = 0;
            state = new Label();
            state.BackColor = Color.Green;
            state.AutoSize = true;
            state.Text = "" + time;
        }
        public override void Handle(Actor a)
        {
            a.hp -= time;
            time = time / 2;
        }
    }
    class Bleed_Act : State
    {
        public Bleed_Act(Actor a)
        {
            effect = 2;
            state = new Label();
            state.BackColor = Color.OrangeRed;
            state.AutoSize = true;
            state.Text = "" + time;
            a.f_dmg = new Bleed_DMG();
        }
        public override void Handle(Actor a)
        {
            time -= 1;
            if(time <= 0)
                a.f_dmg = new Norm_DMG();
        }
    }
    public class Bleed : State
    {
        public Bleed()
        {
            effect = 1;
            state = new Label();
            state.BackColor = Color.Red;
            state.AutoSize = true;
            state.Text = "" + time;
        }
        public override void Handle(Actor a)
        {
            a.hp -= time;
            time -= 1;
        }
    }
}
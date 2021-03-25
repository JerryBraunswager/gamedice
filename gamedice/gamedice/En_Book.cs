using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gamedice
{
    public abstract class Factory
    {
        public abstract void Dispose(Actor a, Actor h);
    }
    class Hero : Factory
    {
        public override void Dispose(Actor a, Actor h)
        {
            a.max_hp = 50;
            a.hp = a.max_hp;
            a.def = 0;
            a.dmg = 6;
        }
    }
    class En_Factory : Factory
    {
        public override void Dispose(Actor a, Actor h)
        {
            a.lvl = h.lvl;
            a.max_hp = 15 + a.lvl - 1;
            a.hp = a.max_hp;
            a.def = 0;
            a.dmg = 4 + a.lvl - 1;
        }
    }
    class En_DMG : Factory
    {
        public override void Dispose(Actor a, Actor h)
        {
            a.lvl = h.lvl;
            a.max_hp = 10 + a.lvl - 1;
            a.hp = a.max_hp;
            a.def = 0;
            a.dmg = 6 + a.lvl - 1;
        }
    }
}

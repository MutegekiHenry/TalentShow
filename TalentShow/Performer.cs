﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentShow
{
    public enum Division { Alpha, Gamma, Omega }

    public class Performer
    {
        public Division Division { get; private set; }
        public PersonName Name { get; private set; }
        public string Affiliation { get; private set; }

        public Performer(Division division, PersonName name, string affiliation)
        {
            Division = division;
            Name = name;
            Affiliation = affiliation;
        }
    }
}

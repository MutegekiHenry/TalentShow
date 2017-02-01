﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace TalentShow.Tests
{
    [TestClass]
    public class ContestantTests
    {
        [TestMethod]
        public void CreateContestantWithOnePerformer()
        {
            Contest contest = new Contest(name: "Dance");

            Division division = new Division("Alpha");
            PersonName name = new PersonName(firstName: "John", lastName: "Smith");
            Organization affiliation = new Organization("ABC");

            Performance performance = new Performance(description: "Dancing an abc to xyz", duration: new TimeSpan(hours: 0, minutes: 2, seconds: 0));

            Contestant contestant = new Contestant(performance);

            Assert.AreEqual(performance, contestant.Performance);
        }

        [TestMethod]
        public void CreateContestantWithMultiplePerformers()
        {
            Contest contest = new Contest(name: "Dance");

            Division division = new Division("Alpha");
            PersonName name = new PersonName(firstName: "John", lastName: "Smith");
            Organization affiliation = new Organization("ABC");

            Division division2 = new Division("Alpha");
            PersonName name2 = new PersonName(firstName: "Bob", lastName: "Beach");
            Organization affiliation2 = new Organization("XYZ");

            Performance performance = new Performance(description: "Dancing an abc to xyz", duration: new TimeSpan(hours: 0, minutes: 2, seconds: 0));

            Contestant contestant = new Contestant(performance);

            Assert.AreEqual(performance, contestant.Performance);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CreateContestantWithZeroPerformers()
        {
            Contest contest = new Contest(name: "Dance");

            Performance performance = new Performance(description: "Dancing an abc to xyz", duration: new TimeSpan(hours: 0, minutes: 2, seconds: 0));

            Contestant contestant = new Contestant(performance);
        }
    }
}

using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Divergence.DataAccess.Helpers
{
    public static class RandomGenerator
    {
        private static DataContext db = new DataContext();

        /// <summary>
        /// Get a new random story name
        /// </summary>
        /// <returns>the new story name</returns>
        public static string StoryName()
        {
            var returnValue = "New Story";
            var rand = new Random();

            var availableFirstNames = db.StoryNames.Where(name => name.Part == DataClasses.NamePart.First);
            var availableMiddleNames = db.StoryNames.Where(name => name.Part == DataClasses.NamePart.Middle);
            var availableLastNames = db.StoryNames.Where(name => name.Part == DataClasses.NamePart.Last);

            if (availableFirstNames.Count() > 1)
                returnValue = availableFirstNames.OrderBy(stz => rand.Next(availableFirstNames.Count() - 1)).FirstOrDefault().Name;
            else if (availableFirstNames.Count() == 1)
                returnValue = availableFirstNames.First().Name;

            if (availableMiddleNames.Count() > 0)
            {
                var numOfNames = Math.Min(availableMiddleNames.Count(), rand.Next(1, 3));

                while (numOfNames > 0)
                {
                    returnValue = " " + availableMiddleNames.OrderBy(stz => rand.Next(availableMiddleNames.Count())).FirstOrDefault().Name;

                    numOfNames--;
                }
            }

            if (availableLastNames.Count() > 1)
                returnValue = " " + availableLastNames.OrderBy(stz => rand.Next(availableLastNames.Count() - 1)).FirstOrDefault().Name;
            else if (availableLastNames.Count() == 1)
                returnValue = " " + availableLastNames.First().Name;

            return returnValue.Trim();
        }

        /// <summary>
        /// Get a new random protagonist name
        /// </summary>
        /// <returns>the person's name</returns>
        public static string ProtagonistName()
        {
            var returnValue = "New Thing";
            var rand = new Random();

            var availableFirstNames = db.ProtagonistNames.Where(name => name.Part == DataClasses.NamePart.First);
            var availableMiddleNames = db.ProtagonistNames.Where(name => name.Part == DataClasses.NamePart.Middle);
            var availableLastNames = db.ProtagonistNames.Where(name => name.Part == DataClasses.NamePart.Last);

            if (availableFirstNames.Count() > 1)
                returnValue = availableFirstNames.OrderBy(stz => rand.Next(availableFirstNames.Count() - 1)).FirstOrDefault().Name;
            else if (availableFirstNames.Count() == 1)
                returnValue = availableFirstNames.First().Name;

            if (availableMiddleNames.Count() > 0)
            {
                var numOfNames = Math.Min(availableMiddleNames.Count(), rand.Next(1, 3));

                while (numOfNames > 0)
                {
                    returnValue = " " + availableMiddleNames.OrderBy(stz => rand.Next(availableMiddleNames.Count())).FirstOrDefault().Name;

                    numOfNames--;
                }
            }

            if (availableLastNames.Count() > 1)
                returnValue = " " + availableLastNames.OrderBy(stz => rand.Next(availableLastNames.Count() - 1)).FirstOrDefault().Name;
            else if (availableLastNames.Count() == 1)
                returnValue = " " + availableLastNames.First().Name;

            return returnValue.Trim();
        }

        /// <summary>
        /// Get a new random protagonist gender
        /// </summary>
        /// <returns>the gender</returns>
        public static string ProtagonistGender()
        {
            var returnValue = "Eunich";
            var rand = new Random();

            var availableGenders = db.ProtagonistGenders;

            if (availableGenders.Count() > 1)
                returnValue = availableGenders.OrderBy(stz => rand.Next(availableGenders.Count() - 1)).FirstOrDefault().Name;
            else if (availableGenders.Count() == 1)
                returnValue = availableGenders.First().Name;

            return returnValue;
        }
    }
}
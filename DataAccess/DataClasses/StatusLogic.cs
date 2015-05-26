using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Divergence.DataAccess.DataClasses
{
    /// <summary>
    /// The current status logic determining if a stanza can be randomly chosen for a pathway
    /// </summary>
    [Serializable]
    public class StatusLogic : IComparable, IEquatable<StatusLogic>
    {
        /// <summary>
        /// List of status logics
        /// </summary>
        public IEnumerable<StatusComparison> StatusComparisons { get; set; }

        /// <summary>
        /// Validate StatusLogic against a single status value
        /// </summary>
        /// <param name="statusValue">the value to be validated</param>
        /// <returns>true or false</returns>
        public bool Validate(KeyValuePair<StatusValue, int> statusValue)
        {
            foreach (var comparison in StatusComparisons)
            {
                if (!comparison.Qualifies(statusValue.Key, statusValue.Value))
                    return false;
            }

            return StatusComparisons.Count() == 0;
        }

        /// <summary>
        /// Simple compare of equality
        /// </summary>
        /// <param name="obj">the other object</param>
        /// <returns>
        /// -1: Null or invalid type comparison
        /// 0: Equal (by db id)
        /// 1: Not the same
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != typeof(StatusLogic))
                return -1;

            var other = (StatusLogic)obj;
            return other.Equals(this) ? 0 : 1;
        }

        /// <summary>
        /// Equate 2 status logic blobs
        /// </summary>
        /// <param name="other">compare-to</param>
        /// <returns>Boolean on equality</returns>
        public bool Equals(StatusLogic other)
        {
            return StatusComparisons.Any(sc => !other.StatusComparisons.Contains(sc));
        }
    }

    /// <summary>
    /// Types of logical comparison methods
    /// </summary>
    public enum LogicComparison : short
    {
        LessThan = 0,
        LessThanOrEqualTo = 1,
        Equals = 2,
        GreaterThan = 3,
        GreaterThanOrEqualTo = 4,
        DoesNotEqual = 5,
        DoesNotExist = 6,
        Exists = 7
    }

    /// <summary>
    /// The components of a logical comparison
    /// </summary>
    [Serializable]
    public class StatusComparison : IComparable, IEquatable<StatusComparison>
    {
        /// <summary>
        /// The string value Status Type
        /// </summary>
        public StatusValue StatusType { get; set; }

        /// <summary>
        /// The logical type to compare it by
        /// </summary>
        public LogicComparison Logic { get; set; }

        /// <summary>
        /// The numerical value to compare against
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Check if the input [logic]s the stored value
        /// </summary>
        /// <param name="type">the status type</param>
        /// <param name="input">the input value</param>
        /// <returns>True if input [logic]s stored value. (ie GreaterThan: input > stored value</returns>
        public bool Qualifies(StatusValue type, int input)
        {
            //Dummy check the incoming type
            if (type != null)
                switch (Logic)
                {
                    case LogicComparison.Exists:
                        return type.Equals(StatusType);
                    case LogicComparison.DoesNotExist:
                        return !type.Equals(StatusType);
                    case LogicComparison.Equals:
                        return type.Equals(StatusType) && input == Value;
                    case LogicComparison.DoesNotEqual:
                        return type.Equals(StatusType) && input != Value;
                    case LogicComparison.GreaterThan:
                        return type.Equals(StatusType) && input > Value;
                    case LogicComparison.GreaterThanOrEqualTo:
                        return type.Equals(StatusType) && input >= Value;
                    case LogicComparison.LessThan:
                        return type.Equals(StatusType) && input < Value;
                    case LogicComparison.LessThanOrEqualTo:
                        return type.Equals(StatusType) && input <= Value;
                }

            //Some weird outlier
            return false;
        }

        /// <summary>
        /// Simple compare of equality
        /// </summary>
        /// <param name="obj">the other object</param>
        /// <returns>
        /// -1: Null or invalid type comparison
        /// 0: Equal (by db id)
        /// 1: Not the same
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != typeof(StatusComparison))
                return -1;

            var other = (StatusComparison)obj;
            return other.Equals(this) ? 0 : 1;
        }

        /// <summary>
        /// Equate 2 status logic blobs
        /// </summary>
        /// <param name="other">compare-to</param>
        /// <returns>Boolean on equality</returns>
        public bool Equals(StatusComparison other)
        {
            return other.Value.Equals(Value) && other.StatusType.Equals(StatusType) && other.Logic.Equals(Logic);
        }
    }
}
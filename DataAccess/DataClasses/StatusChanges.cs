using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Divergence.DataAccess.DataClasses
{
    /// <summary>
    /// The current status logic for modifying plot status after a choice has been made
    /// </summary>
    [Serializable]
    public class StatusChanges : IComparable, IEquatable<StatusChanges>
    {
        /// <summary>
        /// Status logics
        /// </summary>
        public StatusModification StatusModifier { get; set; }

        /// <summary>
        /// The status type to modify
        /// </summary>
        public StatusValue StatusType { get; set; }

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
            if (obj == null || obj.GetType() != typeof(StatusChanges))
                return -1;

            var other = (StatusChanges)obj;
            return other.Equals(this) ? 0 : 1;
        }

        /// <summary>
        /// Equate 2 status logic blobs
        /// </summary>
        /// <param name="other">compare-to</param>
        /// <returns>Boolean on equality</returns>
        public bool Equals(StatusChanges other)
        {
            return other.StatusModifier.Equals(StatusModifier) && other.StatusType.Equals(StatusType);
        }
    }

    /// <summary>
    /// Types of logical comparison methods
    /// </summary>
    public enum ModifierType : short
    {
        Add = 0,
        Subtract = 1,
        Set = 2,
        Remove = 3
    }

    /// <summary>
    /// The components of a logical comparison
    /// </summary>
    [Serializable]
    public class StatusModification : IComparable, IEquatable<StatusModification>
    {
        /// <summary>
        /// The type of modification being done
        /// </summary>
        public ModifierType Type { get; set; }

        /// <summary>
        /// The numerical value to change with
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Check if the input [logic]s the stored value
        /// </summary>
        /// <param name="type">the status type</param>
        /// <param name="input">the input value</param>
        /// <returns>True if input [logic]s stored value. (ie GreaterThan: input > stored value</returns>
        public int? Modify(int? input)
        {
            //Dummy check the incoming type
            switch (Type)
            {
                case ModifierType.Add:
                    input = input.HasValue ? input.Value + Value : Value;
                    break;
                case ModifierType.Subtract:
                    input = input.HasValue ? input.Value - Value : 0 - Value;
                    break;
                case ModifierType.Set:
                    input = Value;
                    break;
                case ModifierType.Remove:
                    input = null;
                    break;
            }

            //Fluent design
            return input;
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
            if (obj == null || obj.GetType() != typeof(StatusModification))
                return -1;

            var other = (StatusModification)obj;
            return other.Equals(this) ? 0 : 1;
        }

        /// <summary>
        /// Equate 2 status logic blobs
        /// </summary>
        /// <param name="other">compare-to</param>
        /// <returns>Boolean on equality</returns>
        public bool Equals(StatusModification other)
        {
            return other.Value.Equals(Value) && other.Type.Equals(Type);
        }
    }
}
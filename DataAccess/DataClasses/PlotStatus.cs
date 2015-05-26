using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Divergence.DataAccess.DataClasses
{
    /// <summary>
    /// The current status value collection of a single arc at a given pathway point
    /// </summary>
    [Serializable]
    public class PlotStatus : IComparable, IEquatable<PlotStatus>
    {
        /// <summary>
        /// Table of status values keyed by strings with integer values
        /// </summary>
        public Dictionary<StatusValue, int> StatusValues { get; set; }

        /// <summary>
        /// Default constructor to init the hashtable
        /// </summary>
        public PlotStatus()
        {
            StatusValues = new Dictionary<StatusValue, int>();
        }

        /// <summary>
        /// Validate PlotStatus against status logic statements
        /// </summary>
        /// <param name="logics">The logic block</param>
        /// <returns>true or false</returns>
        public bool Validate(IEnumerable<StatusLogic> logics)
        {
            foreach(var value in StatusValues)
            {
                if (!logics.All(l => l.Validate(value)))
                    return false;
            }

            return logics.Count() == 0;
        }

        /// <summary>
        /// Compare two PlotStatus blobs
        /// </summary>
        /// <param name="obj">Must be a Plot Status</param>
        /// <returns>
        /// -2: null or inequatable type
        /// -1: input is older than current
        /// 0: Equal
        /// 1: input is newer than current
        /// 2: Equatable type but completely foreign statuses
        /// </returns>
        public int CompareTo(object obj)
        {
            if(obj == null || obj.GetType() != typeof(PlotStatus))
                return -2;

            var other = (PlotStatus)obj;

            if (this.Equals(other))
                return 0;

            if (StatusValues.Any(kvp => !other.StatusValues.ContainsKey(kvp.Key) || other.StatusValues[kvp.Key] < kvp.Value))
                return -1;
            if (other.StatusValues.Any(kvp => !StatusValues.ContainsKey(kvp.Key) || StatusValues[kvp.Key] < kvp.Value))
                return 1;

            return 2;
        }

        /// <summary>
        /// Equate 2 plot status blobs
        /// </summary>
        /// <param name="other">compare-to</param>
        /// <returns>Boolean on equality</returns>
        public bool Equals(PlotStatus other)
        {
            return StatusValues.Any(kvp => !other.StatusValues.ContainsKey(kvp.Key) || !other.StatusValues[kvp.Key].Equals(kvp.Value));
        }
    }
}
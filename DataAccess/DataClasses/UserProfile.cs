using DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergence.DataAccess.DataClasses
{
    [Table("UserProfile")]
    [Serializable]
    public class UserProfile : IComparable, IEquatable<UserProfile>
    {
        private static DataContext db = new DataContext();

        /// <summary>
        /// The db ID
        /// </summary>
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        /// <summary>
        /// The username for logging in
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Compare two users
        /// </summary>
        /// <param name="obj">Must be an UserProfile</param>
        /// <returns>
        /// -2: null or inequatable type
        /// 0: Equal
        /// 2: Equatable type but completely foreign contexts
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != typeof(UserProfile))
                return -2;

            var other = (UserProfile)obj;

            if (other.Equals(this))
                return 0;

            return 2;
        }

        /// <summary>
        /// Equating by db id
        /// </summary>
        /// <param name="other">the other object</param>
        /// <returns>if they are the same db object</returns>
        public bool Equals(UserProfile other)
        {
            return other != null && other.UserId.Equals(this.UserId);
        }
    }
}

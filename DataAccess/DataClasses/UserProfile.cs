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
        /// Arcs this user has participated in
        /// </summary>
        public ICollection<Arc> Arcs { get; set; }

        /// <summary>
        /// Choices this user has created
        /// </summary>
        public ICollection<Choice> Choices { get; set; }

        /// <summary>
        /// Stanzas this user has created
        /// </summary>
        public ICollection<Stanza> Stanzas { get; set; }

        /// <summary>
        /// Tags this user has created
        /// </summary>
        public ICollection<Tag> Tags { get; set; }

        /// <summary>
        /// Tags this user has taken
        /// </summary>
        public ICollection<Pathway> Pathways { get; set; }

        /// <summary>
        /// Genders this user has created
        /// </summary>
        public ICollection<ProtagonistGender> ProtagonistGenders { get; set; }

        /// <summary>
        /// Protagonist Names this user has created
        /// </summary>
        public ICollection<ProtagonistName> ProtagonistNames { get; set; }

        /// <summary>
        /// Status Values this user has created
        /// </summary>
        public ICollection<StatusValue> StatusValues { get; set; }

        /// <summary>
        /// Story Names this user has created
        /// </summary>
        public ICollection<StoryName> StoryNames { get; set; }

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

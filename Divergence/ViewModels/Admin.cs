using Divergence.DataAccess.DataClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Divergence.ViewModels
{
    #region Stanzas
    public class StanzaList
    {
        public IEnumerable<Stanza> Stanzas { get; set; }
    }

    public class AddStanza
    {
        public IEnumerable<Choice> AvailableChoices { get; set; }

        [Required]
        [Range(0, 100)]
        public int MinimumAge { get; set; }

        [Required]
        [Range(0, 100)]
        public int MaximumAge { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Text { get; set; }
    }

    public class RemoveStanza
    {
        public long StanzaId { get; set; }
    }

    public class EditStanza
    {
        public IEnumerable<Choice> AvailableChoices { get; set; }

        [Required]
        [Range(0, 100)]
        public int MinimumAge { get; set; }

        [Required]
        [Range(0, 100)]
        public int MaximumAge { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Text { get; set; }
    }
    #endregion

    #region Choice
    public class ChoiceList
    {
        public IEnumerable<Choice> Choices { get; set; }
    }

    public class AddChoice
    {
        [Required]
        [MaxLength(2000)]
        public string Text { get; set; }
    }
    #endregion

    #region status Objects
    public class AddStatusChange
    {
        [Required]
        public ModifierType Type { get; set; }

        [Required]
        public StatusValue StatusType { get; set; }

        [Required]
        [Range(1,100)]
        public int Value { get; set; }
    }

    public class AddStatusLogic
    {
        [Required]
        public StatusValue StatusType { get; set; }

        [Required]
        public LogicComparison Logic { get; set; }

        [Required]
        [Range(1, 100)]
        public int Value { get; set; }
    }
    #endregion
}
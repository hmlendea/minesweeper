using System;
using System.ComponentModel.DataAnnotations;

namespace Minesweeper.Models
{
    public abstract class ModelBase : IEquatable<ModelBase>
    {
        [Key]
        [StringLength(40, ErrorMessage = "The {0} must be between {1} and {2} characters long", MinimumLength = 1)]
        public abstract string Id { get; }

        public virtual bool Equals(ModelBase other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((ModelBase)obj);
        }

        public override int GetHashCode() => Id?.GetHashCode() ?? 0;

        public override string ToString() => Id;
    }
}

namespace Masasamjant.Repositories
{
    public class Character : IEquatable<Character>
    {
        public Character(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public bool Equals(Character? other)
        {
            return other != null &&
                FirstName == other.FirstName &&
                LastName == other.LastName;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Character);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LastName);
        }
    }
}

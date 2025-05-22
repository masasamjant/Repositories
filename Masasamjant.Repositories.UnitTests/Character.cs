namespace Masasamjant.Repositories
{
    public class Character : IEquatable<Character>
    {
        public Character(Guid identifier, string firstName, string lastName)
        {
            Identifier = identifier;
            FirstName = firstName;
            LastName = lastName;
        }

        public Character()
        { }

        public Guid Identifier { get; internal set; } = Guid.NewGuid();

        public string FirstName { get; internal set; } = string.Empty;

        public string LastName { get; internal set; } = string.Empty;

        public byte[] Version { get; internal set; } = Array.Empty<byte>();

        public bool Equals(Character? other)
        {
            return other != null && Identifier.Equals(other.Identifier);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Character);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Identifier);
        }
    }
}

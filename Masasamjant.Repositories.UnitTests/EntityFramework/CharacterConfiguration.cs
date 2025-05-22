using Masasamjant.Repositories.EntityFramework.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masasamjant.Repositories.EntityFramework
{
    public class CharacterConfiguration : EntityTableConfiguration<Character>
    {
        public CharacterConfiguration() 
            : base("Characters", "Character")
        { }

        protected override void ConfigureTableSchema(EntityTypeBuilder<Character> builder)
        {
            builder.Property(x => x.Identifier).IsRequired();
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(64);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(64);
            builder.Property(x => x.Version).IsRowVersion();
            builder.HasKey(x => x.Identifier);
        }
    }
}

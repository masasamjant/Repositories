using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masasamjant.Repositories.EntityFramework
{
    [TestClass]
    public class EntityRepositoryUnitTest : UnitTest
    {
        [TestMethod]
        public async Task Test_AddAsync()
        {
            var mickey = new Character("Mickey", "Mouse");
            var donald = new Character("Donald", "Duck");  
            EntityRepository repository = new TestRepository();
            await repository.AddAsync(mickey);
            await repository.AddAsync(donald);
            var result = await repository.GetAsync<Character>();
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(mickey));
            Assert.IsTrue(result.Contains(donald));
        }

        [TestMethod]
        public async Task Test_ExistsAsync()
        {
            var mickey = new Character("Mickey", "Mouse");
            var donald = new Character("Donald", "Duck");
            EntityRepository repository = new TestRepository();
            await repository.AddAsync(mickey);
            await repository.AddAsync(donald);
            var specification = new QuerySpecification<Character>(x => x.FirstName == "Minnie");
            var result = await repository.ExistsAsync(specification);
            Assert.IsFalse(result);
            specification = new QuerySpecification<Character>(x => x.FirstName == "Mickey");
            result = await repository.ExistsAsync(specification);
            Assert.IsTrue(result);
        }
    }
}

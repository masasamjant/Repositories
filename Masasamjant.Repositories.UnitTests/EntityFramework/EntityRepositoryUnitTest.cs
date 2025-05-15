namespace Masasamjant.Repositories.EntityFramework
{
    [TestClass]
    public class EntityRepositoryUnitTest : UnitTest
    {
        [TestMethod]
        public async Task Test_AddAsync()
        {
            bool saveChanges = false;
            Action? saveChangesCallback = () => saveChanges = true;
            var mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
            var donald = new Character(Guid.NewGuid(), "Donald", "Duck");
            EntityRepository repository = GetTestRepository(saveChangesCallback);
            await repository.AddAsync(mickey);
            Assert.IsFalse(saveChanges);
            await repository.AddAsync(donald, true);
            Assert.IsTrue(saveChanges);
            var result = await repository.GetAsync<Character>();
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(mickey));
            Assert.IsTrue(result.Contains(donald));
        }

        [TestMethod]
        public async Task Test_ExistsAsync()
        {
            var mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
            var donald = new Character(Guid.NewGuid(), "Donald", "Duck");
            EntityRepository repository = GetTestRepository();
            await repository.AddAsync(mickey);
            await repository.AddAsync(donald);
            var specification = new QuerySpecification<Character>(x => x.FirstName == "Minnie");
            var result = await repository.ExistsAsync(specification);
            Assert.IsFalse(result);
            specification = new QuerySpecification<Character>(x => x.FirstName == "Mickey");
            result = await repository.ExistsAsync(specification);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Test_GetAsync()
        {
            var mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
            var donald = new Character(Guid.NewGuid(), "Donald", "Duck");
            var minnie = new Character(Guid.NewGuid(), "Minnie", "Mouse");
            EntityRepository repository = GetTestRepository();
            await repository.AddAsync(mickey);
            await repository.AddAsync(donald);
            await repository.AddAsync(minnie);
            var result = await repository.GetAsync<Character>();
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains(mickey));
            Assert.IsTrue(result.Contains(donald));
            Assert.IsTrue(result.Contains(minnie));
            var specification = new QuerySpecification<Character>(x => x.LastName == "Mouse");
            result = await repository.GetAsync(specification);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(mickey));
            Assert.IsTrue(result.Contains(minnie));
            Assert.IsFalse(result.Contains(donald));
        }

        [TestMethod]
        public async Task Test_Query()
        {
            var mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
            var donald = new Character(Guid.NewGuid(), "Donald", "Duck");
            var minnie = new Character(Guid.NewGuid(), "Minnie", "Mouse");
            EntityRepository repository = GetTestRepository();
            await repository.AddAsync(mickey);
            await repository.AddAsync(donald);
            await repository.AddAsync(minnie);
            var query = repository.Query<Character>();
            Assert.IsTrue(query.Count() == 3);
            Assert.IsTrue(query.Contains(mickey));
            Assert.IsTrue(query.Contains(donald));
            Assert.IsTrue(query.Contains(minnie));
            var specification = new QuerySpecification<Character>(x => x.LastName == "Mouse");
            query = repository.Query(specification);
            Assert.IsTrue(query.Count() == 2);
            Assert.IsTrue(query.Contains(mickey));
            Assert.IsFalse(query.Contains(donald));
            Assert.IsTrue(query.Contains(minnie));
        }

        [TestMethod]
        public async Task Test_RemoveAsync()
        {
            bool saveChanges = false;
            Action? saveChangesCallback = () => saveChanges = true;
            var mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
            EntityRepository repository = GetTestRepository(saveChangesCallback);
            await repository.AddAsync(mickey);
            var result = await repository.RemoveAsync(mickey);
            Assert.IsTrue(Equals(mickey, result));
            Assert.IsFalse(saveChanges);
            result = (await repository.GetAsync(new QuerySpecification<Character>(x => x.FirstName == "Mickey"))).FirstOrDefault();
            Assert.IsNull(result);

            await repository.AddAsync(mickey);
            result = await repository.RemoveAsync(mickey, true);
            Assert.IsTrue(Equals(mickey, result));
            Assert.IsTrue(saveChanges);
            result = (await repository.GetAsync(new QuerySpecification<Character>(x => x.FirstName == "Mickey"))).FirstOrDefault();
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Test_SaveChangesAsync()
        {
            bool saveChanges = false;
            Action? saveChangesCallback = () => saveChanges = true;
            EntityRepository repository = GetTestRepository(saveChangesCallback);
            var result = await repository.SaveChangesAsync();
            Assert.IsTrue(saveChanges);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Test_UpdateAsync()
        {
            bool saveChanges = false;
            Action? saveChangesCallback = () => saveChanges = true;
            var mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
            EntityRepository repository = GetTestRepository(saveChangesCallback);
            await repository.AddAsync(mickey);
            mickey.FirstName = "Minnie";
            var result = await repository.UpdateAsync(mickey);
            Assert.IsTrue(Equals(mickey, result));
            Assert.AreEqual("Minnie", result.FirstName);
            Assert.IsFalse(saveChanges);
            
            result = (await repository.GetAsync(new QuerySpecification<Character>(x => x.FirstName == "Mickey"))).FirstOrDefault();
            Assert.IsNull(result);

            result = (await repository.GetAsync(new QuerySpecification<Character>(x => x.FirstName == "Minnie"))).FirstOrDefault();
            Assert.IsTrue(Equals(mickey, result));
            
            mickey.FirstName = "Mickey";
            result = await repository.UpdateAsync(mickey, true);
            Assert.IsTrue(Equals(mickey, result));
            Assert.AreEqual("Mickey", result.FirstName);
            Assert.IsTrue(saveChanges);
        }

        private static EntityRepository GetTestRepository(Action? saveChangesCallback = null)
        {
            return new TestRepository("TestRepository", true, saveChangesCallback);
        }
    }
}

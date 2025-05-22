using Masasamjant.Repositories.Abstractions;

namespace Masasamjant.Repositories.EntityFramework
{
    [TestClass]
    public class EntityRepositoryIntegrationTest : IntegrationTest
    {
        private static readonly string connectionString = @"Server=localhost\SQLExpress;Database=Characters;Trusted_Connection=True;TrustServerCertificate=true;";
        private static readonly AutoResetEvent testResetEvent = new AutoResetEvent(true);

        [TestMethod]
        public async Task Test_When_Connected()
        {
            try
            {
                testResetEvent.WaitOne();

                Character? mickey;
                Character? donald;

                using (var repository = GetTestRepository())
                {
                    // Test add
                    mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
                    donald = new Character(Guid.NewGuid(), "Donald", "Duck");
                    await repository.AddAsync(mickey);
                    await repository.AddAsync(donald);
                    var count = await repository.SaveChangesAsync();
                    Assert.AreEqual(2, count);

                    // Test get all
                    var result = await repository.GetAsync<Character>();
                    Assert.AreEqual(2, result.Count);

                    mickey = null;
                    donald = null;

                    // Test get single
                    var specification = new QuerySpecification<Character>(x => x.FirstName == "Mickey" && x.LastName == "Mouse");
                    mickey = (await repository.GetAsync<Character>(specification)).FirstOrDefault();
                    Assert.IsNotNull(mickey);

                    // Test remove
                    mickey = await repository.RemoveAsync(mickey!);
                    count = await repository.SaveChangesAsync();
                    Assert.IsNotNull(mickey);
                    Assert.AreEqual(1, count);

                    // Test exists
                    specification = new QuerySpecification<Character>(x => x.FirstName == "Mickey");
                    var exists = await repository.ExistsAsync(specification);
                    Assert.IsFalse(exists);
                    specification = new QuerySpecification<Character>(x => x.FirstName == "Donald");
                    exists = await repository.ExistsAsync(specification);
                    Assert.IsTrue(exists);

                    // Test query
                    specification = new QuerySpecification<Character>(x => x.LastName == "Duck" || x.LastName == "Mouse");
                    var query = repository.Query(specification);
                    donald = query.Single();
                    Assert.IsNotNull(donald);
                    Assert.AreEqual("Donald", donald.FirstName);
                    Assert.AreEqual("Duck", donald.LastName);

                    // Test update
                    donald.LastName = "Deck";
                    donald = await repository.UpdateAsync(donald);
                    count = await repository.SaveChangesAsync();
                    Assert.AreEqual(1, count);
                    Assert.AreEqual("Deck", donald.LastName);

                    // Test remove 
                    donald = await repository.RemoveAsync(donald);
                    count = await repository.SaveChangesAsync();
                    Assert.IsNotNull(donald);
                    Assert.AreEqual(1, count);

                    // Test get all
                    result = await repository.GetAsync<Character>();
                    Assert.AreEqual(0, result.Count);
                }
            }
            finally
            {
                testResetEvent.Set();
            }
        }

        [TestMethod]
        public async Task Test_When_Disconnected()
        {
            try
            {
                testResetEvent.WaitOne();

                Character? mickey;
                Character? donald;
                using (var repository = GetTestRepository())
                {
                    mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
                    donald = new Character(Guid.NewGuid(), "Donald", "Duck");
                    await repository.AddAsync(mickey);
                    await repository.AddAsync(donald);
                    var actual = await repository.SaveChangesAsync();
                    Assert.AreEqual(2, actual);
                }

                using (var repository = GetTestRepository())
                {
                    var result = await repository.GetAsync<Character>();
                    Assert.AreEqual(2, result.Count);
                }

                mickey = null;
                donald = null;

                using (var repository = GetTestRepository())
                {
                    var specification = new QuerySpecification<Character>(x => x.FirstName == "Mickey" && x.LastName == "Mouse");
                    mickey = (await repository.GetAsync<Character>(specification)).FirstOrDefault();
                    Assert.IsNotNull(mickey);
                }

                // Test remove
                using (var repository = GetTestRepository())
                {
                    mickey = await repository.RemoveAsync(mickey!);
                    var result = await repository.SaveChangesAsync();
                    Assert.IsNotNull(mickey);
                    Assert.AreEqual(1, result);
                }

                // Test exists
                using (var repository = GetTestRepository())
                {
                    var specification = new QuerySpecification<Character>(x => x.FirstName == "Mickey");
                    var exists = await repository.ExistsAsync(specification);
                    Assert.IsFalse(exists);
                    specification = new QuerySpecification<Character>(x => x.FirstName == "Donald");
                    exists = await repository.ExistsAsync(specification);
                    Assert.IsTrue(exists);
                }

                // Test query
                using (var repository = GetTestRepository())
                {
                    var specification = new QuerySpecification<Character>(x => x.LastName == "Duck" || x.LastName == "Mouse");
                    var query = repository.Query(specification);
                    donald = query.Single();
                    Assert.IsNotNull(donald);
                    Assert.AreEqual("Donald", donald.FirstName);
                    Assert.AreEqual("Duck", donald.LastName);
                }

                // Test update
                using (var repository = GetTestRepository())
                {
                    donald.LastName = "Deck";
                    donald = await repository.UpdateAsync(donald);
                    var count = await repository.SaveChangesAsync();
                    Assert.AreEqual(1, count);
                    Assert.AreEqual("Deck", donald.LastName);
                }

                // Test remove
                using (var repository = GetTestRepository())
                {
                    donald = await repository.RemoveAsync(donald);
                    var count = await repository.SaveChangesAsync();
                    Assert.IsNotNull(donald);
                    Assert.AreEqual(1, count);
                }

                // Test get all
                using (var repository = GetTestRepository())
                {
                    var result = await repository.GetAsync<Character>();
                    Assert.AreEqual(0, result.Count);
                }
            }
            finally
            {
                testResetEvent.Set();
            }
        }

        [TestMethod]
        public async Task Test_Transaction_Rollback()
        {
            try
            {
                testResetEvent.WaitOne();

                Character? mickey;
                Character? donald;

                using (var repository = GetTestRepository())
                {
                    var transaction = await repository.BeginTransactionAsync();

                    // Test add
                    mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
                    donald = new Character(Guid.NewGuid(), "Donald", "Duck");
                    await repository.AddAsync(mickey);
                    await repository.AddAsync(donald);
                    var count = await repository.SaveChangesAsync();
                    Assert.AreEqual(2, count);

                    await transaction.RollbackAsync();
                    transaction.Dispose();

                    var result = await repository.GetAsync<Character>();
                    Assert.AreEqual(0, result.Count);
                }
            }
            finally
            {
                testResetEvent.Set();
            }
        }

        [TestMethod]
        public async Task Test_Transaction_Commit()
        {
            try
            {
                testResetEvent.WaitOne();

                Character? mickey;
                Character? donald;

                using (var repository = GetTestRepository())
                {
                    var transaction = await repository.BeginTransactionAsync();

                    // Test add
                    mickey = new Character(Guid.NewGuid(), "Mickey", "Mouse");
                    donald = new Character(Guid.NewGuid(), "Donald", "Duck");
                    await repository.AddAsync(mickey);
                    await repository.AddAsync(donald);
                    var count = await repository.SaveChangesAsync();
                    Assert.AreEqual(2, count);

                    await transaction.CommitAsync();
                    transaction.Dispose();

                    var result = await repository.GetAsync<Character>();
                    Assert.AreEqual(2, result.Count);

                    transaction = await repository.BeginTransactionAsync();

                    foreach (var item in result)
                    {
                        await repository.RemoveAsync(item);
                    }

                    await repository.SaveChangesAsync();
                    await transaction.CommitAsync();
                    transaction.Dispose();

                    result = await repository.GetAsync<Character>();
                    Assert.AreEqual(0, result.Count);
                }
            }
            finally
            {
                testResetEvent.Set();
            }
        }

        private static IRepository GetTestRepository(Action? saveChangesCallback = null)
        {
            return new TestRepository(connectionString, false, saveChangesCallback);
        }
    }
}

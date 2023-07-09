using Billing.Model;
using NUnit.Framework;

namespace BillingTests
{
    [TestFixture]
    public class EmissionTest
    {
        private Emission emission;

        private void Distribute(List<User> users,long amountOfCoins)
        {
            emission = new Emission(users, amountOfCoins);
            emission.Distribute();
        }

        [Test]
        public void DistributeWhenUsersEmptyOrNull()
        {
            Distribute(new List<User>(), 10);
            Assert.IsFalse(emission.Success);
            Distribute(null, 10);
            Assert.IsFalse(emission.Success);
        }

        [TestCase(-10)]
        [TestCase(0)]
        public void DistributeWhenIncorrectAmountOfCoins(long amountOfCoins)
        {
            var users = new List<User>() { new User("oleg", 100) };
            Distribute(users, amountOfCoins);
            Assert.IsFalse(emission.Success);
        }

        [TestCase(1)]
        [TestCase(1000)]
        public void DistributeWhenOneUser(long amountOfCoins)
        {
            var users = new List<User>() { new User("oleg", 100) };
            Distribute(users, amountOfCoins);
            DistributionValidation(users, amountOfCoins);
        }

        [TestCase(1000)]
        [TestCase(5000)]
        [TestCase(25000)]
        public void DistributeWhenManyUsers(long amountOfCoins)
        {
            var users = new List<User>();
            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                users.Add(new User("user", rnd.Next()));
            }
            Distribute(users, amountOfCoins);
            DistributionValidation(users, amountOfCoins);
        }

        [TestCase(3)]
        [TestCase(10)]
        [TestCase(150)]
        [TestCase(15000)]
        [TestCase(100000)]
        public void DistributeWhenUsersWithSameRating(long amountOfCoins)
        {
            var users = new List<User>() { new User("oleg", 500), new User("sofia",500),new User("boris",500) };
            Distribute(users, amountOfCoins);
            DistributionValidation(users, amountOfCoins);
        }

        [TestCase(3)]
        [TestCase(10)]
        [TestCase(150)]
        [TestCase(15000)]
        [TestCase(100000)]
        public void DistributeWhenTotalRatingIsZero(long amountOfCoins)
        {
            var users = new List<User>() { new User("oleg", 0), new User("sofia", 0), new User("boris", 0) };
            Distribute(users, amountOfCoins);
            DistributionValidation(users, amountOfCoins);
        }

        [Test]
        public void DistributeSeveralTimes()
        {
            var users = new List<User>() { new User("oleg", 200), new User("sofia", 1200), new User("boris", 450) };
            emission = new Emission(users, 200);
            emission.Distribute();
            emission.Distribute();
            emission = new Emission(users, 600);
            emission.Distribute();
            Assert.That(users.Sum(u => u.Amount), Is.EqualTo(1000));
        }


        public void DistributionValidation(List<User> users, long expectedAmountOfCoins)
        {
            var totalAmountOfCoins = users.Sum(user => user.Amount);
            Assert.IsTrue(emission.Success);
            Assert.IsTrue(users.All(user => user.Amount > 0));
            Assert.That(totalAmountOfCoins, Is.EqualTo(expectedAmountOfCoins));
            users = users.DistinctBy(u=>u.Rating).OrderBy(user => user.Rating).ToList();
            var previous = users[0].Amount;
            for (int i = 1; i < users.Count; i++)
            {
                Assert.LessOrEqual(previous, users[i].Amount);
            }
        }
    }
}
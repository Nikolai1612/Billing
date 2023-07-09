
namespace Billing.Model
{
    public class Coin
    {
        private static long counterId;

        static Coin()
        {
            counterId = 0;
        }

        public long Id { get; private set; }
        public int NumberOfHolders => holders.Count;

        private List<User> holders;

        public Coin()
        {
            Id = ++counterId;
            holders = new List<User>();
        }

        public void AddNewHolder(User holder)
        {
            holders.Add(holder);
        }

        public string GetHistory()
        {
            return string.Join("-", holders.Select(user=>user.Name));
        }
    }
}
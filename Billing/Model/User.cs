namespace Billing.Model
{
    public class User
    {
        private List<Coin> coins;
        public string Name { get; private set; }
        public int Rating { get; private set; }
        public long Amount => coins.Count;

        public User(string name, int rating)
        {
            Name = name;    
            Rating = rating;
            coins = new List<Coin>();
        }

        public void AddCoin(Coin coin)
        {
            coin.AddNewHolder(this);
            coins.Add(coin);
        }

        public void SendCoinsToUser(User user, long amount)
        {
            for (long i = 0; i < amount; i++)
            {
                user.AddCoin(coins[coins.Count - 1]);
                coins.RemoveAt(coins.Count - 1);
            }
        }

        public List<Coin> GetCoins()
        {
            return coins.ToList();
        }
    }
}
using System;

namespace Billing.Model
{
    public class Emission
    {
        private List<User> users;
        private long amountOfNewCoins;
        private long remainingCoins;
        private long totalRating;

        public bool Success { get; private set; }
        public string Comment { get; private set; }

        public Emission(List<User> users, long amountOfNewCoins)
        {
            this.users = users;
            this.amountOfNewCoins = amountOfNewCoins;
        } 


        public void Distribute()
        {
            if (DataIsNotCorrect()) return;
            remainingCoins = amountOfNewCoins;
            totalRating = users.Sum(user => (long)user.Rating);
            if (totalRating == 0) DistributeRemainingCoins(users);
            else DistributeCoinsAmongUsersByRating();
            Success = true;
            Comment = $"Emission completed successfully.Created and distributed {amountOfNewCoins} coins";
        }

        private bool DataIsNotCorrect()
        {
            if (users == null || users.Count == 0)
            {
                Comment = "Emission is not possible. User list is empty";
                Success = false;
                return true;
            }
            if (amountOfNewCoins < users.Count)
            {
                Comment = "Emission is not possible. Amount of coins is less than the number of users.";
                Success = false;
                return true;
            }
            return false;
        }

        private void DistributeCoinsAmongUsersByRating()
        {
            users = users
                .OrderBy(user => user.Rating)
                .ToList();
            var coinPerRating = (double)remainingCoins / totalRating;
            var remaindersOfCoins = new List<(User User, double Amount)>();
            foreach (var user in users)
            {
                var coinsForCurrentRating = coinPerRating * user.Rating;
                if ((long)coinsForCurrentRating < 1)
                {
                    coinsForCurrentRating = 1;
                    totalRating -= user.Rating;
                    coinPerRating = (double)(remainingCoins-coinsForCurrentRating) / totalRating;
                }
                else remaindersOfCoins.Add((user, coinsForCurrentRating - (long)coinsForCurrentRating));
                remainingCoins -= (long)coinsForCurrentRating;
                AddCoins(user, (long)coinsForCurrentRating);
            }
            if (remainingCoins == 0) return;
            var usersWithRemainders = remaindersOfCoins
                .OrderByDescending(pair => pair.Amount)
                .Select(pair=>pair.User)
                .ToList();
            DistributeRemainingCoins(usersWithRemainders);
        }

        private void DistributeRemainingCoins(List<User> users)
        {
            int i = 0;
            while (remainingCoins > 0)
            {
                AddCoins(users[i], 1);
                i = (i + 1) % users.Count;
                remainingCoins--;
            }
        }

        private void AddCoins(User user, long amount)
        {
            for (long i = 0; i < amount; i++)
            {
                user.AddCoin(new Coin());
            }
        }
    }
}
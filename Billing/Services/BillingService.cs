using Billing;
using Billing.Model;
using Grpc.Core;
using System.Xml.Linq;

namespace Billing.Services
{
    public class BillingService : Billing.BillingBase
    {
        static List<User> users = new List<User>()
        {
            new User("boris",5000),
            new User("maria",1000),
            new User("oleg",800)
        };

        private readonly ILogger<BillingService> _logger;
        public BillingService(ILogger<BillingService> logger)
        {
            _logger = logger;
        }

        public override async Task ListUsers(None request,
            IServerStreamWriter<UserProfile> responseStream,
            ServerCallContext context)
        {
            foreach (var user in users)
            {
                await responseStream.WriteAsync(new UserProfile { Name = user.Name, Amount = user.Amount });
            }
        }

        public override Task<Response> CoinsEmission(EmissionAmount request, ServerCallContext context)
        {
            var emission = new Emission(users, request.Amount);
            emission.Distribute();
            return Task.FromResult(new Response()
            {
                Status = emission.Success ? Response.Types.Status.Ok : Response.Types.Status.Failed,
                Comment =emission.Comment
            });
        }

        public override Task<Response> MoveCoins(MoveCoinsTransaction request, ServerCallContext context)
        {
            var srcUser = users
                .FirstOrDefault(user => user.Name == request.SrcUser);
            var dstUser = users
                .FirstOrDefault(user => user.Name == request.DstUser);

            if (srcUser == null || dstUser == null)
            {
                return Task.FromResult(new Response()
                {
                    Status = Response.Types.Status.Failed,
                    Comment = srcUser == null ? $" user {request.SrcUser} not found\n" : "" +
                    dstUser == null ? $" user {request.DstUser} not found\n" : ""
                });
            }
            if (srcUser.Amount < request.Amount)
            {
                return Task.FromResult(new Response()
                {
                    Status = Response.Types.Status.Failed,
                    Comment = $"Impossible to move {request.Amount} coins from {request.SrcUser} to {request.DstUser} because " +
                    $"{request.SrcUser} has less than {request.Amount} coins"
                });
            }
            srcUser.SendCoinsToUser(dstUser, request.Amount);
            return Task.FromResult(new Response() 
            { 
                Status = Response.Types.Status.Ok, 
                Comment = $"{request.Amount} coins moved from {srcUser.Name} to {dstUser.Name}"
            });
        }

        public override Task<Coin> LongestHistoryCoin(None request, ServerCallContext context)
        {
            var coins = users
                .SelectMany(user => user.GetCoins());
            var coinWithLongestHistory = coins
                .FirstOrDefault(coin => coin.NumberOfHolders == coins.Max(c => c.NumberOfHolders));
            if (coinWithLongestHistory == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Coin not found"));
            return Task.FromResult(new Coin() { Id = coinWithLongestHistory.Id, History = coinWithLongestHistory.GetHistory() });
        }
    }
}
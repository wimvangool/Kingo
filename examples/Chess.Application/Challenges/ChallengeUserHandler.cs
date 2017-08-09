using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Users;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class ChallengeUserHandler : IMessageHandler<ChallengeUserCommand>
    {
        private readonly IUserRepository _users;
        private readonly IChallengeRepository _challenges;

        public ChallengeUserHandler(IUserRepository users, IChallengeRepository challenges)
        {            
            _users = users;
            _challenges = challenges;
        }

        public async Task HandleAsync(ChallengeUserCommand message, IMicroProcessorContext context)
        {            
            var playerOne = await _users.GetByIdAsync(Session.Current.UserId);
            var playerTwo = await _users.GetByIdAsync(message.UserId);

            await _challenges.AddAsync(playerOne.Challenge(message.ChallengeId, playerTwo));
        }
    }
}

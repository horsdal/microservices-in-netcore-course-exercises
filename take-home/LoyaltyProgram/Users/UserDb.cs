using System.Collections.Generic;

namespace LoyaltyProgram.Users
{
    public class UserDb
    {
        private static readonly IDictionary<int, LoyaltyProgramUser> RegisteredUsers = new Dictionary<int, LoyaltyProgramUser>();
    
        public LoyaltyProgramUser GetUserById(int userId) => 
            RegisteredUsers.ContainsKey(userId) 
                ? RegisteredUsers[userId]
                : null;
    
        public LoyaltyProgramUser UpdateUser(
            int userId,
            LoyaltyProgramUser user)
            => RegisteredUsers[userId] = user;

        private LoyaltyProgramUser RegisterUser(LoyaltyProgramUser user)
        {
            var userId = RegisteredUsers.Count;
            user.Id = userId;
            return RegisteredUsers[userId] = user;
        }
    }
}
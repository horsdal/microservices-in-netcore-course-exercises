using System;
using System.Collections.Generic;
using System.Linq;

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

        public LoyaltyProgramUser RegisterUser(LoyaltyProgramUser user)
        {
            var userId = RegisteredUsers.Count;
            var newUser = user with { Id = userId };
            return RegisteredUsers[userId] = user;
        }

        public IEnumerable<LoyaltyProgramUser> LookUpByTag(string tag) =>
            RegisteredUsers
                .Values
                .Where(u => u.Settings.Interests.Contains(tag));
    }
}
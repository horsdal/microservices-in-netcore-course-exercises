using System;

namespace LoyaltyProgram.Users
{
  public record LoyaltyProgramSettings()
  {
    public LoyaltyProgramSettings(string[] interests) : this()
    {
      this.Interests = interests;
    }

    public string[] Interests { get; init; } = Array.Empty<string>();
  }
  public record LoyaltyProgramUser(int Id, string Name, int LoyaltyPoints, LoyaltyProgramSettings Settings);
}
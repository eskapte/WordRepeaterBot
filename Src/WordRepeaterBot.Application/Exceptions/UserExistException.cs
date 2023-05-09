namespace WordRepeaterBot.Application.Exceptions;

public class UserExistException : Exception
{
    public UserExistException(long userId) : base($"User with id {userId} is already exist") { }
}

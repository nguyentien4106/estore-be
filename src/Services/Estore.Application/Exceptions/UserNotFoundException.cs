using BuildingBlocks.Exceptions;

namespace EStore.Application.Exceptions;

public class UserNotFoundException(string name, string email) : NotFoundException(name, email)
{
    
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Exceptions;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Domain.Entities;

namespace SmartLoan.Application.Users.Commands.RegisterUser;

public class RegisterUserHandler(IApplicationDbContext context, IIdentityService identityService)
    : IRequestHandler<RegisterUserCommand, int>
{
    public async Task<int> Handle(RegisterUserCommand request, CancellationToken ct)
    {
       
        var exists = await context.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (exists) throw new BusinessRuleException("A user with this email already exists.");


        var hashedPassword = identityService.HashPassword(request.Password);

        
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = hashedPassword,
            Role = request.Role
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(ct);

        return user.Id;
    }
}
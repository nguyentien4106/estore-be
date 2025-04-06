﻿namespace EStore.Application.Files.Queries.GetFilesByUserName;

public record GetFilesByUserNameQuery(string UserName) : IQuery<AppResponse<List<FileEntityResponse>>>;

public class GetFilesByUserNameQueryValidator : AbstractValidator<GetFilesByUserNameQuery>
{
    public GetFilesByUserNameQueryValidator()
    {
            RuleFor(x => x.UserName)
                    .NotNull().WithMessage("UserName is required.");
        }
}

using FluentValidation;
using JWTAuthServer.Core.DTOs;

namespace JWTAuthServer.API.Validations.UserValidations
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Lütfen Email giriniz.").EmailAddress().WithMessage("Lütfen geçerli bir Email giriniz.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Lütfen Şifre giriniz");

            RuleFor(x => x.UserName).NotEmpty().WithMessage("Lütfen UserName giriniz");
        }
    }
}

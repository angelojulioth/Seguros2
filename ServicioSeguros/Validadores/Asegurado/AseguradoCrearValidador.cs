using FluentValidation;
using Seguros.Intermediarios.Mensajes.Asegurado;

namespace ServicioSeguros.Validadores.Asegurado
{
    public class AseguradoCrearValidador : AbstractValidator<AseguradoCrearDto>, IValidator<AseguradoCrearDto>
    {
        public AseguradoCrearValidador()
        {
            RuleFor(x => x.Cedula)
                .NotEmpty()
                .WithMessage("La cédula es obligatoria");

            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("{PropertyName} es obligatorio y tiene un máxima longitud de 100 caracteres");

            RuleFor(x => x.Telefono)
                .MaximumLength(15)
                .WithMessage("{PropertyName} tiene un máxima longitud de 15 caracteres");

            RuleFor(x => x.FechaNacimiento)
                .NotEmpty()
                .LessThan(DateTime.Today)
                .WithMessage("{PropertyName} es obligatoria y debe ser una fecha pasada");
        }
    }
}
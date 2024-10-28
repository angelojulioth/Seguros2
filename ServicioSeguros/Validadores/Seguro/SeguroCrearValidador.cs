using FluentValidation;
using Seguros.Intermediarios.Mensajes.Seguro;

namespace ServicioSeguros.Validadores.Seguro
{
    public class SeguroCrearValidador : AbstractValidator<SeguroCrearDto>, IValidator<SeguroCrearDto>
    {
        public SeguroCrearValidador()
        {
            RuleFor(x => x.Codigo).NotEmpty().MaximumLength(10).WithMessage("El código no puede estar vacío ni tener más de 10 caracteres");
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100).WithMessage("{PropertyName} es obligatorio y tiene un máxima longitud de 100 caracteres");
            RuleFor(x => x.SumaAsegurada).NotEmpty().GreaterThan(1000).WithMessage("{PropertyName} es obligatorio y tiene que ser mayor a 1000");
            RuleFor(x => x.Prima).NotEmpty().GreaterThan(0).WithMessage("{PropertyName} es obligatoria y tiene que ser mayor a 0");
            RuleFor(x => x.EdadMinima).GreaterThan(0).WithMessage("La edad tiene que ser mayor a 0");
            RuleFor(x => x.EdadMaxima).GreaterThan(0).LessThan(100).WithMessage("La edad tiene que ser menor a 100");
        }
    }
}

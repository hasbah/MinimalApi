using FluentValidation;
using MinimalApi.Core;
using MinimalApi.Services.Interfaces;
using System.Net;

namespace MinimalApi.Filters
{
    public class BasicValidator<T> : IEndpointFilter where T : class
    {
        private IValidator<T> _validator; 
        public BasicValidator(IValidator<T> validator )
        {
            _validator = validator; 
        }

        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            var contextObj = context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T));

            if (contextObj == null)
            {
                return Results.BadRequest(response);
            }
            var result = await _validator.ValidateAsync((T)contextObj);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(p => p.ErrorMessage).ToList();
                response.ErrorMessages.AddRange(errors);
                return Results.BadRequest(response);
            }
            return await next(context);
        }
    }
}

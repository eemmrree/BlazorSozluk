using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Extensions
{
    public static class Registration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
        {
            var assem = Assembly.GetExecutingAssembly();
            services.AddMediatR(assem);
            services.AddAutoMapper(assem);
            services.AddValidatorsFromAssembly(assem);
            return services;
        }
    }
}

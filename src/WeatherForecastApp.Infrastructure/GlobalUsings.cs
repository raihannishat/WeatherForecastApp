// System namespaces
global using System.IdentityModel.Tokens.Jwt;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text;

// Microsoft libraries
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;

// Application layer
global using WeatherForecastApp.Application.Common;
global using WeatherForecastApp.Application.Contracts.Repositories;
global using WeatherForecastApp.Application.Contracts.Repositories.Base;
global using WeatherForecastApp.Application.Contracts.Services;
global using WeatherForecastApp.Application.Contracts.Services.Auth;
global using WeatherForecastApp.Application.Specifications;
global using WeatherForecastApp.Application.Specifications.Contracts;

// Domain layer
global using WeatherForecastApp.Domain.Base;
global using WeatherForecastApp.Domain.Entities;
global using WeatherForecastApp.Domain.ValueObjects;

// Infrastructure layer
global using WeatherForecastApp.Infrastructure.Extensions;
global using WeatherForecastApp.Infrastructure.Persistence;
global using WeatherForecastApp.Infrastructure.Persistence.Configurations;
global using WeatherForecastApp.Infrastructure.Repositories;
global using WeatherForecastApp.Infrastructure.Repositories.Implementations;
global using WeatherForecastApp.Infrastructure.Services;


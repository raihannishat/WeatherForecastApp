// System namespaces
global using System.Linq.Expressions;
global using System.Reflection;

// Third-party libraries
global using FluentValidation;
global using Mapster;
global using MediatR;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;

// Application layer
global using WeatherForecastApp.Application.Behaviors;
global using WeatherForecastApp.Application.Common;
global using WeatherForecastApp.Application.Common.Mapping;
global using WeatherForecastApp.Application.Contracts.Repositories;
global using WeatherForecastApp.Application.Contracts.Repositories.Base;
global using WeatherForecastApp.Application.Contracts.Services.Auth;
global using WeatherForecastApp.Application.Contracts.Services.WeatherForecast;
global using WeatherForecastApp.Application.DTOs;
global using WeatherForecastApp.Application.DTOs.Auth;
global using WeatherForecastApp.Application.DTOs.WeatherForecasts;
global using WeatherForecastApp.Application.Specifications;
global using WeatherForecastApp.Application.Specifications.Contracts;
global using WeatherForecastApp.Application.Extensions;
global using WeatherForecastApp.Application.Services;

// Domain layer
global using WeatherForecastApp.Domain.Base;
global using WeatherForecastApp.Domain.Entities;
global using WeatherForecastApp.Domain.Exceptions;
global using WeatherForecastApp.Domain.ValueObjects;


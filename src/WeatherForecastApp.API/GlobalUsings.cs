// System namespaces
global using System.Net;
global using System.Text;
global using System.Text.Json;
global using System.Threading.RateLimiting;

// Third-party libraries
global using FluentValidation;
global using MediatR;
global using Scalar.AspNetCore;
global using Serilog;
global using Serilog.Events;
global using Serilog.Sinks.File;

// ASP.NET Core
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.RateLimiting;
global using Microsoft.IdentityModel.Tokens;

// Application layer
global using WeatherForecastApp.Application.Common;
global using WeatherForecastApp.Application.Contracts.Services;
global using WeatherForecastApp.Application.DTOs;
global using WeatherForecastApp.Application.DTOs.Auth;
global using WeatherForecastApp.Application.DTOs.WeatherForecasts;
global using WeatherForecastApp.Application.Features.Auth.Commands.Login;
global using WeatherForecastApp.Application.Features.Auth.Commands.Register;
global using WeatherForecastApp.Application.Features.WeatherForecasts.Commands.CreateWeatherForecast;
global using WeatherForecastApp.Application.Features.WeatherForecasts.Queries.GetAllWeatherForecasts;
global using WeatherForecastApp.Application.Features.WeatherForecasts.Queries.GetForecastsByTemperatureRange;

// Extensions (API only references Infrastructure.Extensions for DI registration)
global using WeatherForecastApp.API.Constants;
global using WeatherForecastApp.API.Extensions;
global using WeatherForecastApp.Application.Extensions;
global using WeatherForecastApp.Infrastructure.Extensions;

// Project namespaces
global using WeatherForecastApp.API.Middleware;
global using WeatherForecastApp.API.Models;


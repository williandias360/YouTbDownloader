// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using YouTbDownloader.application;
using YouTbDownloader.Config;

var serviceProvider = Ioc.RegistrarDependencias();

var app = serviceProvider.GetRequiredService<Application>();
app.Run();
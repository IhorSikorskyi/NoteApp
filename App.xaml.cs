using Microsoft.Extensions.DependencyInjection;
using NoteApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NoteAppWPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static IServiceProvider _serviceProvider;

    public static IServiceProvider ServiceProvider => _serviceProvider ??= ConfigureServices();

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddScoped<Context>();
        services.AddScoped<Cache>();
        
        return services.BuildServiceProvider();
    }
}
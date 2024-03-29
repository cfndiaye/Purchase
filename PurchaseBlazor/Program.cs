﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PurchaseShared.Models;
using MudBlazor.Services;
using PurchaseBlazor.Pages.Authentication;
using Blazored.LocalStorage;
using Radzen;


namespace PurchaseBlazor
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("#app");
      if (builder.HostEnvironment.IsProduction())
      {
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://dakar-hightech.com:11443") });
      }
      else
      {
        //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped(c => new HttpClient { BaseAddress = new Uri(SettingApp.ServerUrl) });
        

      }


      builder.Services.AddBlazoredLocalStorage();
      builder.Services.AddAuthorizationCore();
      builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
      builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
      builder.Services.AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Information));
           

      //Include Radzen
      builder.Services.AddScoped<DialogService>();
      builder.Services.AddScoped<NotificationService>();
      builder.Services.AddScoped<TooltipService>();
      builder.Services.AddScoped<ContextMenuService>();

      //Include MudBlazor
      builder.Services.AddMudServices();

      //await builder.Build().RunAsync();

      var app = builder.Build();
      

      await app.RunAsync();
    }
  }
}

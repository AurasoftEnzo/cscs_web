//using CSCS_Web_Enzo_1;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//}
//app.UseRouting();

//app.UseAuthorization();

//app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();


//app.Run();



//// In Startup.cs 
//public void ConfigureServices(IServiceCollection services)
//{
//    services.AddControllersWithViews();
//    services.AddSingleton<TemplateService>();
//    services.AddSingleton<FunctionDispatcher>();

//    // Register basic SSR functions
//    var dispatcher = new FunctionDispatcher();
//    dispatcher.RegisterFunction("LoadTemplate", new LoadTemplateFunction());
//    dispatcher.RegisterFunction("RenderTemplate", new RenderTemplateFunction());
//    services.AddSingleton(dispatcher);
//}

//------------------------------------------------------------

//using CSCS_Web_Enzo_1;
//using CSCS_Web_Enzo_1.CSCS.Template;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddSingleton<TemplateService>();
//builder.Services.AddSingleton<FunctionDispatcher>(provider =>
//{
//    var dispatcher = new FunctionDispatcher();
//    var templateService = provider.GetRequiredService<TemplateService>();

//    // Register your CSCS functions here
//    dispatcher.RegisterFunction("LoadTemplate", new LoadTemplateFunction(templateService));
//    dispatcher.RegisterFunction("RenderTemplate", new RenderTemplateFunction(templateService));

//    return dispatcher;
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();


//-----------------------------------------------

//using CSCS;
//using CSCS_Web_Enzo_1;
//using CSCS_Web_Enzo_1.CSCS;
//using CSCS_Web_Enzo_1.CSCS.Template;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using SplitAndMerge;

//var builder = WebApplication.CreateBuilder(args);

// Add services
//builder.Services.AddControllersWithViews();
////builder.Services.AddSingleton<TemplateService>();


//builder.Services.AddSingleton<FunctionDispatcher>(provider =>
//{
//    var dispatcher = new FunctionDispatcher();
//    var templateService = provider.GetRequiredService<TemplateService>();

//    // Register core functions
//    dispatcher.RegisterFunction("LoadTemplate", new LoadTemplateFunction(templateService));
//    dispatcher.RegisterFunction("Render", new RenderTemplateFunction(templateService));

//    // Register math functions if needed
//    //dispatcher.RegisterFunction("Sin", new SinFunction());

//    return dispatcher;
//});



////// Configure CSCS functions
////builder.Services.AddSingleton<FunctionDispatcher>(provider =>
////{
////    var dispatcher = new FunctionDispatcher();
////    var templateService = provider.GetRequiredService<TemplateService>();

////    dispatcher.RegisterFunction("LoadTemplate", new LoadTemplateFunction(templateService));
////    dispatcher.RegisterFunction("Render", new RenderTemplateFunction(templateService));

////    return dispatcher;
////});



//var app = builder.Build();


// // Middleware pipeline
//app.UseStaticFiles();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Render}/{action=Index}");


//CSCS_Web.App.Run();


//----------------------------------------------------


using CSCS_Web_Enzo_1;
using Microsoft.Extensions.FileProviders;
using System;
using System.Runtime;

var builder = WebApplication.CreateBuilder(args);

//--CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
//--CORS


var cscsConfig = builder.Configuration.GetSection("CSCSConfig").Get<CSCSConfig>();



var serverApp = builder.Build();

serverApp.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(cscsConfig.StaticFilesDirectory)
});
//serverApp.UseStaticFiles();

//--CORS
serverApp.UseCors();
//--CORS

//// global cors policy
//serverApp.UseCors(x => x
//    .AllowAnyMethod()
//    .AllowAnyHeader()
//    .SetIsOriginAllowed(origin => true) // allow any origin
//                                        //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
//    .AllowCredentials()); // allow credentials


CSCSWebApplication.Initialize(serverApp, cscsConfig);


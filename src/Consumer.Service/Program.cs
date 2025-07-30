using ConsumerService;
using ConsumerService.Consumers;
using KitchenService.Data;
using MenuService.Data;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;

var builder = Host.CreateApplicationBuilder(args);

// Configuração dos DbContexts
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb") ?? 
        throw new InvalidOperationException("Connection string 'OrderDb' not found.")));

builder.Services.AddDbContext<KitchenDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KitchenDb") ?? 
        throw new InvalidOperationException("Connection string 'KitchenDb' not found.")));

builder.Services.AddDbContext<MenuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MenuDb") ?? 
        throw new InvalidOperationException("Connection string 'MenuDb' not found.")));

// Registrar os consumers como hosted services
builder.Services.AddHostedService<OrderConsumer>();
builder.Services.AddHostedService<KitchenConsumer>();
builder.Services.AddHostedService<MenuConsumer>();

// Opcional: manter o Worker se necessário para outras tarefas
// builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// Garantir que os bancos de dados sejam criados
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    try
    {
        var orderContext = services.GetRequiredService<OrderDbContext>();
        var kitchenContext = services.GetRequiredService<KitchenDbContext>();
        var menuContext = services.GetRequiredService<MenuDbContext>();
        
        await orderContext.Database.EnsureCreatedAsync();
        await kitchenContext.Database.EnsureCreatedAsync();
        await menuContext.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao criar os bancos de dados");
    }
}

host.Run();

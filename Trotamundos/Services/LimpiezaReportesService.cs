using Negocio.Interfaces;

namespace Trotamundos.Services;

/// <summary>
/// Elimina periódicamente los reportes resueltos y las alertas leídas
/// una vez transcurridas 24 horas desde que se resolvieron/leyeron.
/// </summary>
public class LimpiezaReportesService : BackgroundService
{
    private static readonly TimeSpan Retencion = TimeSpan.FromHours(24);
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(15);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LimpiezaReportesService> _logger;

    public LimpiezaReportesService(IServiceScopeFactory scopeFactory, ILogger<LimpiezaReportesService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Intervalo);
        do
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IReporteRepository>();
                int eliminados = await repo.EliminarResueltosAnterioresAAsync(DateTime.UtcNow - Retencion);
                if (eliminados > 0)
                    _logger.LogInformation("Limpieza: {Cantidad} reportes/alertas resueltos eliminados.", eliminados);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error al limpiar reportes y alertas resueltos.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}

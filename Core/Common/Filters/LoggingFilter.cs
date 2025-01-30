using System.Diagnostics;

using MassTransit;
using MassTransit.Configuration;

using Serilog;


namespace Core.Common.Filters;

public class LoggingSpecification<T> : IPipeSpecification<T>
where T : class, PipeContext
{
    public void Apply(IPipeBuilder<T> builder) => builder.AddFilter(new LoggingFilter<T>());

    public IEnumerable<ValidationResult> Validate() => [];
}


public class LoggingFilter<T> : IFilter<T>
where T : class, PipeContext
{
    public async Task Send(T context, IPipe<T> next)
    {
        var sw = Stopwatch.StartNew();

        Log.Information("processing message {@Context}", context);

        try
        {
            await next.Send(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "failed to process message: {ExceptionMessage}", ex.Message);

            throw;
        }
        finally
        {
            Log.Verbose("message processed in {Elapsed}ms", sw.Elapsed);
        }
    }

    public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(LoggingFilter<T>));
}
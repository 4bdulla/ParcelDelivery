using Core.Common.Monitoring;

using MassTransit;
using MassTransit.Configuration;

using Prometheus;

using ITimer = Prometheus.ITimer;


namespace Core.Common.Filters;

public class MonitoringSpecification<T>(MetricReporter reporter) : IPipeSpecification<T>
where T : class, PipeContext
{
    public void Apply(IPipeBuilder<T> builder) => builder.AddFilter(new MonitoringFilter<T>(reporter));

    public IEnumerable<ValidationResult> Validate() => [];
}


public class MonitoringFilter<T>(MetricReporter reporter) : IFilter<T>
where T : class, PipeContext
{
    public async Task Send(T context, IPipe<T> next)
    {
        if (!context.TryGetPayload(out T payload))
        {
            await next.Send(context);

            return;
        }

        string requestName = payload.GetType().Name;

        using ITimer timer = reporter.RequestProcessingDuration.WithLabels(requestName).NewTimer();
        using IDisposable progress = reporter.RequestInProcess.WithLabels(requestName).TrackInProgress();

        reporter.Requests.WithLabels(requestName).Inc();

        try
        {
            await next.Send(context);
        }
        catch (Exception ex)
        {
            reporter.Exceptions.WithLabels(requestName, ex.GetType().Name).Inc();

            throw;
        }
    }

    public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(MonitoringFilter<T>));
}
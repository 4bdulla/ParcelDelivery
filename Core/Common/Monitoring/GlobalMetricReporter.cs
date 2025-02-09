using Prometheus;


namespace Core.Common.Monitoring;

public class MetricReporter
{
    private readonly Gauge _serviceStatusGauge = Metrics.CreateGauge(
        "service_status",
        "Service up/down status",
        new GaugeConfiguration { LabelNames = ["service"] });

    private readonly Gauge _serviceUptimeGauge = Metrics.CreateGauge(
        "service_uptime",
        "Uptime of the service",
        new GaugeConfiguration { LabelNames = ["service"] });

    public Histogram RequestProcessingDuration { get; } = Metrics.CreateHistogram(
        "request_processing_duration",
        "Request processing duration",
        new HistogramConfiguration
        {
            Buckets = Histogram.ExponentialBuckets(1, 2, 16),
            LabelNames = ["request"],
        });

    public Gauge RequestInProcess { get; } = Metrics.CreateGauge(
        "request_in_process",
        "Count of currently processing requests",
        new GaugeConfiguration { LabelNames = ["request"] });

    public Counter Requests { get; } = Metrics.CreateCounter(
        "request_count",
        "Count of requests",
        labelNames: ["request"]);

    public Counter Exceptions { get; } = Metrics.CreateCounter(
        "exception_count",
        "Number of exceptions",
        labelNames: ["request", "error_code"]);

    public void ServiceUp(string service)
    {
        _serviceStatusGauge.WithLabels(service).Inc();
        _serviceUptimeGauge.WithLabels(service).SetToCurrentTimeUtc();
    }

    public void ServiceDown(string service)
    {
        _serviceStatusGauge.WithLabels(service).Dec();
        _serviceUptimeGauge.WithLabels(service).SetToTimeUtc(DateTimeOffset.UnixEpoch);
    }
}
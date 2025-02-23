using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin;

namespace Recombobulator;

[PluginName("Pen Recombobulator")]
public class PenRecombobulator : IPositionedPipelineElement<IDeviceReport>
{
    private int _initialReportCount;
    private const int MaxInitialReports = 3;
    private const uint PressureDifferenceThreshold = 2300;
    private uint _lastPressure;
    private bool _penDown;

    [Property("Invert Pressure")]
    [DefaultPropertyValue(true)]
    public bool InvertPressure { get; set; } = true;

    [Property("Max Pressure")]
    [DefaultPropertyValue(8191)]
    public uint MaxPressure { get; set; } = 8191;

    [BooleanProperty("Invert X Tilt", "")]
    [DefaultPropertyValue(false)]
    public bool InvertXTilt { get; set; } = false;

    [BooleanProperty("Invert Y Tilt", "")]
    [DefaultPropertyValue(false)]
    public bool InvertYTilt { get; set; } = false;

    public PipelinePosition Position => PipelinePosition.PreTransform;

    public event Action<IDeviceReport> Emit;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint ProcessPressure(uint rawPressure)
    {
        if (_initialReportCount < MaxInitialReports)
        {
            uint pressureDifference = (uint)Math.Abs((int)rawPressure - (int)_lastPressure);
            if (pressureDifference > PressureDifferenceThreshold)
            {
                rawPressure = InvertPressure ? MaxPressure : 0;
            }
            _initialReportCount++;
        }
        _lastPressure = rawPressure;
        return InvertPressure ? MaxPressure - rawPressure : rawPressure;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector2 ProcessTilt(Vector2 tilt)
    {
        // branchless inversion
        return new Vector2(
            InvertXTilt ? -tilt.X : tilt.X,
            InvertYTilt ? -tilt.Y : tilt.Y
        );
    }

    public void Consume(IDeviceReport deviceReport)
    {
        // switch statement for jump table?
        switch (deviceReport)
        {
            case ITabletReport report:
                bool penCurrentlyDown = report.Pressure > 0;
                if (penCurrentlyDown != _penDown)
                {
                    _penDown = penCurrentlyDown;
                    _initialReportCount = 0;
                    _lastPressure = InvertPressure ? MaxPressure : 0;
                }
                report.Pressure = ProcessPressure(report.Pressure);
                Emit?.Invoke(report);
                break;

            case ITiltReport tiltReport:
                tiltReport.Tilt = ProcessTilt(tiltReport.Tilt);
                Emit?.Invoke(tiltReport);
                break;

            default:
                Emit?.Invoke(deviceReport); // pass thru other report types
                break;
        }
    }
}
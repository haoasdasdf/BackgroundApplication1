using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Devices.Sensors;
using Windows.Foundation;
using System.Diagnostics;

namespace BackgroundApplication1
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private Accelerometer accelerometer;
        private int count;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            accelerometer = Accelerometer.GetDefault();
            if (accelerometer != null)
            {
                uint minReportInterval = accelerometer.MinimumReportInterval;
                Debug.WriteLine($"MinimumReportInterval={minReportInterval}");
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                accelerometer.ReportInterval = reportInterval;
                accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }
        }

        private void ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            AccelerometerReading reading = args.Reading;
            Debug.WriteLine($"x={reading.AccelerationX,5:0.00}, y={reading.AccelerationY,5:0.00}, z={reading.AccelerationZ,5:0.00}");
            count++;
            if (count == 20)
            {
                // Stop reporting
                accelerometer.ReportInterval = 0;
                accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }
        }
    }
}

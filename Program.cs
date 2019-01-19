using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace VolumeLock
{
    public static class Program
    {
        public const string DATE_TIME_FORMAT_STRING = "yyyy-MM-dd HH:mm:ss.fff";

        public static async Task Main()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.OSArchitecture != Architecture.X64)
            {
                Console.WriteLine("This program only supports Windows operating systems on the 64-bit architecture.");
                Environment.Exit(1);
            }

            using (CancellationTokenSource cTokenSource = new CancellationTokenSource())
            {
                // Wait for CTRL+C so we can abort.
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => { cTokenSource.Cancel(); };
                Console.WriteLine("[{0}] Press Ctrl+C to exit application.", DateTime.Now.ToString(Program.DATE_TIME_FORMAT_STRING));

                // Create a new controller and get the default devices.
                CoreAudioController ctrl = new CoreAudioController();
                CoreAudioDevice playbackDevice = ctrl.DefaultPlaybackDevice;
                CoreAudioDevice communicationsDevice = ctrl.DefaultCaptureCommunicationsDevice;

                // Observe the playback device.
                VolumeLockObserver playbackObs = new VolumeLockObserver(playbackDevice);
                playbackObs.Subscribe(playbackDevice.VolumeChanged);

                // Observe the capture device.
                VolumeLockObserver captureObs = new VolumeLockObserver(communicationsDevice);
                captureObs.Subscribe(communicationsDevice.VolumeChanged);

                // Wait indefinitely for CTRL+C.
                try { await Task.Delay(-1, cTokenSource.Token); }
                catch { Environment.Exit(0); }
            }
        }
    }
}

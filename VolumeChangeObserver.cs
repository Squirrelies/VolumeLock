using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Diagnostics;

namespace VolumeLock
{
    public class VolumeLockObserver : IObserver<DeviceVolumeChangedArgs>
    {
        private IDisposable unsubscriber;
        private CoreAudioDevice device;
        private readonly double lockValue;

        public VolumeLockObserver(CoreAudioDevice device)
        {
            this.device = device;
            this.lockValue = device.Volume;

            Console.WriteLine("[{0}] Locking [{1}] volume to {2}%.", DateTime.Now.ToString(Program.DATE_TIME_FORMAT_STRING), this.device.FullName, lockValue);
        }

        public virtual void Subscribe(IObservable<DeviceVolumeChangedArgs> provider)
        {
            this.unsubscriber = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            this.unsubscriber.Dispose();
        }

        public virtual void OnCompleted()
        {
            Debug.WriteLine("[{0}] Additional volume changes will not be monitored or handled.", DateTime.Now.ToString(Program.DATE_TIME_FORMAT_STRING));
        }

        public virtual void OnError(Exception error)
        {
            Debug.WriteLine("[{0}] Exception {1}:\r\n{2}\r\n{3}", DateTime.Now.ToString(Program.DATE_TIME_FORMAT_STRING), error.GetType().ToString(), error.Message, error.StackTrace);
        }

        public virtual void OnNext(DeviceVolumeChangedArgs current)
        {
            if (this.lockValue != current.Volume)
            {
                Console.WriteLine("[{0}] Volume change on [{1}] to {2}%. Setting back to lock value of {3}%.", DateTime.Now.ToString(Program.DATE_TIME_FORMAT_STRING), this.device.FullName, current.Volume, this.lockValue);
                this.device.Volume = this.lockValue;
            }
        }
    }
}

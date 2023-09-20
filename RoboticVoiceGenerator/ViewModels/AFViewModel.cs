using System;
using System.Windows.Input;

using Microsoft.Win32;

using NAudio.Wave;

using Xlfdll;
using Xlfdll.Windows.Presentation;

namespace RoboticVoiceGenerator
{
    public class AFViewModel : ObservableObject, IDisposable
    {
        public AFViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;
            this.VoiceGenerator = new AFVoiceGenerator(this.MainViewModel.ResourceManager);
            this.WaveOut = new WaveOut();

            this.Pitch = 1.8;
            this.TrimStart = 0.1;
            this.TrimEnd = 0.85;

            this.WaveOut.PlaybackStopped += WaveOut_PlaybackStopped;
        }

        public MainViewModel MainViewModel { get; }

        private String _inputText;
        private String _outputText;
        private Boolean _isPreviewing;

        public String InputText
        {
            get => _inputText;
            set => SetField(ref _inputText, value);
        }
        public String OutputText
        {
            get => _outputText;
            set => SetField(ref _outputText, value);
        }
        public Boolean IsPreviewing
        {
            get => _isPreviewing;
            set => SetField(ref _isPreviewing, value);
        }

        public Double Pitch
        {
            get
            {
                return this.VoiceGenerator.Pitch;
            }
            set
            {
                this.VoiceGenerator.Pitch = value;

                OnPropertyChanged(nameof(this.Pitch));
            }
        }
        public Double TrimStart
        {
            get
            {
                return this.VoiceGenerator.TrimStart;
            }
            set
            {
                this.VoiceGenerator.TrimStart = value;

                OnPropertyChanged(nameof(this.TrimStart));
            }
        }
        public Double TrimEnd
        {
            get
            {
                return this.VoiceGenerator.TrimEnd;
            }
            set
            {
                this.VoiceGenerator.TrimEnd = value;

                OnPropertyChanged(nameof(this.TrimEnd));
            }
        }

        public ICommand PreviewCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    if (!this.IsPreviewing)
                    {
                        this.IsPreviewing = true;

                        currentSampleProvider = this.VoiceGenerator.GetVoice(this.OutputText.Split(' '));

                        if (currentSampleProvider != null)
                        {
                            this.WaveOut.Init(currentSampleProvider);
                            this.WaveOut.Play();
                        }
                    }
                    else
                    {
                        this.WaveOut.Stop();
                    }
                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.OutputText);
                }
            );

        public ICommand SaveCommand
            => new RelayCommand<Object>
            (
                async delegate
                {
                    SaveFileDialog dlg = new SaveFileDialog()
                    {
                        Filter = "Wave Audio Files (*.wav)|*.wav|All Files (*.*)|*.*",
                        RestoreDirectory = true
                    };

                    if (dlg.ShowDialog() == true)
                    {
                        currentSampleProvider = this.VoiceGenerator.GetVoice(this.OutputText.Split(' '));

                        WaveFileWriter.CreateWaveFile(dlg.FileName, currentSampleProvider.ToWaveProvider());

                        this.DisposeResources();

                        await this.MainViewModel.DialogCoordinator.ShowMessageAsync
                            (this, "Operation completed", $"Wave has been saved to\n\n{dlg.FileName}");
                    }
                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.OutputText);
                }
            );

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            this.IsPreviewing = false;

            this.DisposeResources();
        }

        private void DisposeResources()
        {
            (currentSampleProvider as IDisposable)?.Dispose();
        }

        private ISampleProvider currentSampleProvider;


        public AFVoiceGenerator VoiceGenerator { get; }
        public WaveOut WaveOut { get; }

        #region IDisposable Members

        public void Dispose()
        {
            this.WaveOut?.Dispose();
        }

        #endregion
    }
}
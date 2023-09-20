using System;
using System.Windows.Input;

using AFVoiceGenerator.Helpers;

using MahApps.Metro.Controls.Dialogs;

using Microsoft.Win32;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using Xlfdll;
using Xlfdll.Windows.Presentation;

namespace AFVoiceGenerator
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.DialogCoordinator = dialogCoordinator;

            this.InputText = String.Empty;
            this.OutputText = String.Empty;
            this.Pitch = 1.8;
            this.TrimStart = 0.1;
            this.TrimEnd = 0.85;
            this.IsPreviewing = false;

            PronounceAudioHelper.WaveOut.PlaybackStopped += WaveOut_PlaybackStopped;
        }

        public IDialogCoordinator DialogCoordinator { get; }

        private String _inputText;
        private String _outputText;
        private Double _pitch;
        private Double _trimStart;
        private Double _trimEnd;
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
        public Double Pitch
        {
            get => _pitch;
            set => SetField(ref _pitch, value);
        }
        public Double TrimStart
        {
            get => _trimStart;
            set => SetField(ref _trimStart, value);
        }
        public Double TrimEnd
        {
            get => _trimEnd;
            set => SetField(ref _trimEnd, value);
        }
        public Boolean IsPreviewing
        {
            get => _isPreviewing;
            set => SetField(ref _isPreviewing, value);
        }

        public ICommand PreviewCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    if (!this.IsPreviewing)
                    {
                        this.IsPreviewing = true;

                        currentSampleProviders = PronounceAudioHelper.GetProcessedObjects
                            (this.OutputText.Split(' '),
                            Properties.Resources.ResourceManager,
                            this.Pitch,
                            this.TrimStart,
                            this.TrimEnd);

                        ISampleProvider resultSampleProvider = currentSampleProviders[currentSampleProviders.Length - 1] as ISampleProvider;

                        if (resultSampleProvider != null)
                        {
                            PronounceAudioHelper.WaveOut.Init(resultSampleProvider);
                            PronounceAudioHelper.WaveOut.Play();
                        }
                    }
                    else
                    {
                        PronounceAudioHelper.WaveOut.Stop();
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
                        currentSampleProviders = PronounceAudioHelper.GetProcessedObjects
                                        (this.OutputText.Split(' '),
                                        Properties.Resources.ResourceManager,
                                        this.Pitch,
                                        this.TrimStart,
                                        this.TrimEnd);

                        ISampleProvider provider = currentSampleProviders[currentSampleProviders.Length - 1] as ISampleProvider;

                        WaveFileWriter.CreateWaveFile(dlg.FileName, provider.ToWaveProvider());

                        this.DisposeResources();

                        await this.DialogCoordinator.ShowMessageAsync
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
            foreach (ISampleProvider sampleProvider in currentSampleProviders)
            {
                (sampleProvider as IDisposable)?.Dispose();
            }

            currentSampleProviders = null;
        }

        private ISampleProvider[] currentSampleProviders;
    }
}
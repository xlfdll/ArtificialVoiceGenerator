using System;
using System.Windows.Input;

using Microsoft.Win32;

using NAudio.Wave;

using Xlfdll;
using Xlfdll.Windows.Presentation;

namespace ArtificialVoiceGenerator
{
    public class AFViewModel : ObservableObject
    {
        public AFViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.VoiceGenerator = new AFVoiceGenerator(this.MainViewModel.ResourceManager);
            this.WaveOut = new WaveOutEvent();
            this.WaveOut.PlaybackStopped += WaveOut_PlaybackStopped;
        }

        public MainViewModel MainViewModel { get; }

        private String _inputText;
        private String _outputText;
        private Boolean _isPreviewing;

        public String InputText
        {
            get
            {
                return _inputText;
            }
            set
            {
                SetField(ref _inputText, value);

                this.OutputText = AFVoiceGenerator.DecomposePinyin(value);
            }
        }
        public String OutputText
        {
            get => _outputText;
            private set => SetField(ref _outputText, value);
        }
        public Boolean IsPreviewing
        {
            get => _isPreviewing;
            set => SetField(ref _isPreviewing, value);
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

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            this.IsPreviewing = false;
        }

        public ICommand PreviewCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    if (!this.IsPreviewing)
                    {
                        this.IsPreviewing = true;

                        IWaveProvider wave = this.VoiceGenerator.GetVoice<IWaveProvider>
                            (this.OutputText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                        this.WaveOut.Init(wave);
                        this.WaveOut.Play();
                    }
                    else
                    {
                        if (this.WaveOut.PlaybackState == PlaybackState.Playing)
                        {
                            this.WaveOut.Stop();
                        }

                        this.IsPreviewing = false;
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


                        await this.MainViewModel.DialogCoordinator.ShowMessageAsync
                            (this, "Operation completed", $"Wave has been saved to\n\n{dlg.FileName}");
                    }
                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.OutputText);
                }
            );


        private AFVoiceGenerator VoiceGenerator { get; }
        private WaveOutEvent WaveOut { get; }
    }
}
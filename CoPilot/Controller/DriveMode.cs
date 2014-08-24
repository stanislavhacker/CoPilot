using CoPilot.Core.Utils;
using CoPilot.Data;
using CoPilot.Resources;
using CoPilot.Utils;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;
using View = CoPilot.CoPilot.View;

namespace CoPilot.CoPilot.Controller
{
    /// <summary>
    /// Speech grammars
    /// </summary>
    public static class SpeechGrammars
    {
        public static String MAIN = "main";
        public static String NUMBERS = "numbers";
        public static String FILL = "fill";
    }

    /// <summary>
    /// Speech context
    /// </summary>
    public enum SpeechContext
    {
        Normal,
        Listening,
        VideoRecording
    }

    /// <summary>
    /// Predefinned speak
    /// </summary>
    public enum PredefinnedSpeak
    {
        Speed,
        Writing
    }

    public class DriveMode : Base
    {

        #region PRIVATE

        private SpeechRecognizer recognizer;
        private SpeechSynthesizer synthetizer;
        private Sentences sentences;
        private Boolean speaking = false;
        private Popup window;

        #endregion

        #region PROPERTY

        /// <summary>
        /// Commands
        /// </summary>
        private Commands commands;
        public Commands Commands
        {
            get
            {
                return commands;
            }
            set
            {
                commands = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Content
        /// </summary>
        private object content = null;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Page
        /// </summary>
        private String page = null;
        public String Page
        {
            get
            {
                return page;
            }
            set
            {
                page = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is drive mode allowed
        /// </summary>
        private bool isDriveModeAllowed = false;
        public bool IsDriveModeAllowed
        {
            get
            {
                return isDriveModeAllowed;
            }
            set
            {
                isDriveModeAllowed = value;
                RaisePropertyChanged();

                if (value && IsDeviceConnected && IsSupported)
                {
                    this.StartDriveMode();
                }
            }
        }

        /// <summary>
        /// Is device connected
        /// </summary>
        private bool isDeviceConnected = false;
        public bool IsDeviceConnected
        {
            get
            {
                return isDeviceConnected;
            }
            set
            {
                isDeviceConnected = value;
                RaisePropertyChanged();

                if (value && isDriveModeAllowed && IsSupported)
                {
                    this.StartDriveMode();
                }
            }
        }

        /// <summary>
        /// Is supported
        /// </summary>
        private bool isSupported = true;
        public bool IsSupported
        {
            get
            {
                return isSupported;
            }
            set
            {
                isSupported = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Orientation
        /// </summary>
        public PageOrientation Orientation
        {
            get
            {
                return angle == -90 ? PageOrientation.LandscapeLeft : PageOrientation.LandscapeRight;
            }
            set
            {
                angle = value == PageOrientation.LandscapeLeft ? -90 : 90;
                RaisePropertyChanged();
                OnPropertyChanged("Angle");
            }
        }

        /// <summary>
        /// Angle
        /// </summary>
        private Double angle = 90;
        public Double Angle
        {
            get
            {
                return angle;
            }
        }

        /// <summary>
        /// SpeechContext
        /// </summary>
        private SpeechContext speechContext = SpeechContext.Normal;
        public SpeechContext SpeechContext
        {
            get
            {
                return speechContext;
            }
            set
            {
                if (value == speechContext)
                {
                    return;
                }
                speechContext = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is open
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return window != null && window.IsOpen;
            }
        }

        /// <summary>
        /// CoPilot
        /// </summary>
        private View.CoPilot copilot;
        public View.CoPilot CoPilot
        {
            get
            {
                return copilot;
            }
            set
            {
                copilot = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// Tap Command
        /// </summary>
        public ICommand LiseningCommand
        {
            get
            {
                return new RelayCommand(async (param) =>
                {
                    await makeLiseningCommand();
                }, param => true);
            }
        }

        #endregion

        /// <summary>
        /// Drive mode
        /// </summary>
        public DriveMode()
        {
            LoadSpeechFile();

            InitializeSpeechRecognition();
            InitializeSpeechSynthetizer();
        }

        /// <summary>
        /// Start driver mode
        /// </summary>
        public async void StartDriveMode()
        {
            if (window != null)
            {
                return;
            }

            if (recognizer == null)
            {
                await createSpeechRecognizer();
            }

            speechContext = SpeechContext.Normal;

            View.Popup.DriveMode win = new View.Popup.DriveMode();
            win.Width = App.Current.Host.Content.ActualWidth;
            win.Height = App.Current.Host.Content.ActualHeight;
            win.DataContext = this;

            window = new Popup();
            window.VerticalAlignment = VerticalAlignment.Center;
            window.HorizontalAlignment = HorizontalAlignment.Center;
            window.Child = win;
            window.IsOpen = true;

            OnPropertyChanged("IsOpen");

            Speak(sentences.Start);
        }

        /// <summary>
        /// Stop driver mode
        /// </summary>
        public void StopDriveMode()
        {
            if (window == null)
            {
                return;
            }

            recognizer.Dispose();
            InitializeSpeechRecognition();

            speechContext = SpeechContext.Normal;

            window.IsOpen = false;
            window = null;

            OnPropertyChanged("IsOpen");

            Speak(sentences.Stop);
        }

        /// <summary>
        /// Speak
        /// </summary>
        /// <param name="what"></param>
        /// <returns></returns>
        public async Task Speak(Sentence sentences)
        {
            //cancel all
            synthetizer.CancelAll();

            try
            {
                //get sentance
                String sentance = this.getSentanceFromMorePossibilities(sentences);
                //say
                await Speak(sentance);
            }
            catch
            {
                Debug.WriteLine("Speech canceled.");
            }
        }

        /// <summary>
        /// Speak text
        /// </summary>
        /// <param name="sentance"></param>
        /// <returns></returns>
        private async Task Speak(String sentance)
        {
            String ssmlText = "";
            ssmlText = "<speak version=\"1.0\" ";
            ssmlText += "xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-GB\">";
            ssmlText += sentance;
            ssmlText += "<mark name=\"END\"/>";
            ssmlText += "</speak>";
            await synthetizer.SpeakSsmlAsync(ssmlText);
        }

        /// <summary>
        /// Speek for predefinned type
        /// </summary>
        /// <param name="predefinnedSpeak"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task Speak(PredefinnedSpeak predefinnedSpeak, double value)
        {
            //if speaking, cancel request
            if (speaking)
            {
                return;
            }

            //say something
            switch (predefinnedSpeak)
            {
                case PredefinnedSpeak.Speed:
                    if (value > 50)
                    {
                        await Speak(sentences.MoreThenFifty);
                    }
                    break;
                case PredefinnedSpeak.Writing:
                    await Speak(sentences.Writing);
                    await Speak(value.ToString());
                    break;
            }
        }

        /// <summary>
        /// Listen
        /// </summary>
        public async Task Listen(int tries = 2)
        {
            //return if not allowed context
            if (SpeechContext == SpeechContext.VideoRecording || window == null)
            {
                return;
            }

            SpeechContext = SpeechContext.Listening;

            //speak
            if (tries == 2)
            {
                await Speak(sentences.Say);
            }

            //listen
            SpeechRecognitionResult result = await ListenImmediately();

            //return if not allowed context
            if (window == null)
            {
                return;
            }

            //results
            if (result != null && IsTextConfidence(result.TextConfidence))
            {
                SpeechContext = SpeechContext.Normal;

                //confident
                this.processMainGrammar(result);
                this.processFillGrammar(result);
            }
            else
            {
                //not confident
                await Speak(sentences.NotUndestand);

                //tries
                if (tries > 0)
                {
                    tries--;
                    await Speak(sentences.TryItAgain);
                    await Listen(tries);
                }
                else
                {
                    await Speak(sentences.HaveTimeToThink);
                    SpeechContext = SpeechContext.Normal;
                }
            }
        }

        #region LISTEN

        /// <summary>
        /// Listen now
        /// </summary>
        /// <returns></returns>
        private async Task<SpeechRecognitionResult> ListenImmediately()
        {
            SpeechRecognitionResult result;
            try
            {
                result = await this.recognizer.RecognizeAsync();
            }
            catch
            {
                result = null;
            }
            return result;
        }

        #endregion

        #region GRAMMARS

        /// <summary>
        /// Process main grammar
        /// </summary>
        /// <param name="result"></param>
        private async void processMainGrammar(SpeechRecognitionResult result)
        {
            //not main rule
            if (result.RuleName != SpeechGrammars.MAIN)
            {
                return;
            }

            var command = result.Semantics["Command"].Value as String;

            //cancel
            if (command == "Cancel" || command == "Back")
            {
                CoPilot.BackCommand.Execute(null);
                await Speak(sentences.Cancel);
                return;
            }

            //make-photo
            if (command == "MakePhoto")
            {
                CoPilot.CameraController.TakeShot();
                await Speak(sentences.TakeShot);
                return;
            }

            //start-recording
            if (command == "StartRecording")
            {
                await Speak(sentences.StartRecordingInfo);
                await Speak(sentences.StartRecordingNow);
                SpeechContext = SpeechContext.VideoRecording;
                CoPilot.CameraController.RecordStart();
                return;
            }

            //add fill
            if (command == "AddFill")
            {
                CoPilot.AddFuelCommand.Execute(null);
                await Speak(sentences.AddFill);
                return;
            }
        }

        /// <summary>
        /// Process fill grammar
        /// </summary>
        /// <param name="result"></param>
        private async void processFillGrammar(SpeechRecognitionResult result)
        {
            if (result.RuleName != SpeechGrammars.FILL)
            {
                return;
            }

            var page = this.Page;
            var command = result.Semantics["Fill"].Value as String;

            //input
            if (page == "Fuel.xaml")
            {
                if (command == "Save")
                {
                    var isSaveEnable = this.getContent<View.Fuel>().IsSaveEnable;
                    if (isSaveEnable)
                    {
                        this.getContent<View.Fuel>().OkCommand.Execute(null);
                        await this.Speak(sentences.SaveOk);
                    }
                    else
                    {
                        await this.Speak(sentences.CannotSave);
                    }
                }
                else if (command == "FullTank")
                {
                    this.getContent<View.Fuel>().ToggleCheckBox(command);
                }
                else
                {
                    SpeechContext = SpeechContext.Listening;

                    await this.Speak(sentences.SayValue);

                    var number = await ListenImmediately();
                    var stringValue = "";
                    try
                    {
                        stringValue = number.Semantics["Amount"].Value as String;
                    }
                    catch
                    {
                        stringValue = null;
                    }

                    Double amount;
                    if (Double.TryParse(stringValue, out amount))
                    {
                        this.getContent<View.Fuel>().EnterValue(command, amount.ToString());
                        await this.Speak(PredefinnedSpeak.Writing, amount);
                    }
                    else
                    {
                        await this.Speak(sentences.NotUndestand);
                    }

                    SpeechContext = SpeechContext.Normal;
                }
            }
        }

        /// <summary>
        /// Get content
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T getContent<T>()
        {
            return (T)this.Content;
        }

        #endregion

        #region PRIVATE

        /// <summary>
        /// Load speech file
        /// </summary>
        private void LoadSpeechFile()
        {
            sentences = Sentences.Load();
            this.Commands = Commands.Load();
        }

        /// <summary>
        /// Initialize speech synthetizer
        /// </summary>
        private void InitializeSpeechSynthetizer()
        {
            var voice = InstalledVoices.All.FirstOrDefault((e) => e.Language.Contains("en") && e.Gender == VoiceGender.Male);

            if (voice == null)
            {
                this.IsSupported = false;
                return;
            }

            synthetizer = new SpeechSynthesizer();
            synthetizer.SetVoice(voice);
            synthetizer.SpeechStarted += (SpeechSynthesizer sender, SpeechStartedEventArgs args) =>
            {
                this.speaking = true;
            };
            synthetizer.BookmarkReached += (SpeechSynthesizer sender, SpeechBookmarkReachedEventArgs args) =>
            {
                this.speaking = false;
            };
        }

        /// <summary>
        /// Initialize speech
        /// </summary>
        private void InitializeSpeechRecognition()
        {
            var langauge = InstalledSpeechRecognizers.All.FirstOrDefault(e => e.Language.ToLowerInvariant() == "en-gb");
            if (langauge == null)
            {
                this.IsSupported = false;
                return;
            }
        }

        /// <summary>
        /// Create
        /// </summary>
        private async Task createSpeechRecognizer()
        {
            var langauge = InstalledSpeechRecognizers.All.FirstOrDefault(e => e.Language.ToLowerInvariant() == "en-gb");
            if (langauge == null)
            {
                return;
            }

            recognizer = new SpeechRecognizer();
            recognizer.SetRecognizer(langauge);

            //grammars
            recognizer.Grammars.AddGrammarFromUri(SpeechGrammars.MAIN, new Uri("ms-appx:///Resources/Speech/Grammars/main.en-gb.xml", UriKind.Absolute));
            recognizer.Grammars.AddGrammarFromUri(SpeechGrammars.NUMBERS, new Uri("ms-appx:///Resources/Speech/Grammars/number.en-gb.xml", UriKind.Absolute));
            recognizer.Grammars.AddGrammarFromUri(SpeechGrammars.FILL, new Uri("ms-appx:///Resources/Speech/Grammars/fill.en-gb.xml", UriKind.Absolute));

            //preload grammars
            await recognizer.PreloadGrammarsAsync();
        }

        /// <summary>
        /// Make lisening command
        /// </summary>
        /// <returns></returns>
        private async Task makeLiseningCommand()
        {
            switch (this.SpeechContext)
            {
                case SpeechContext.VideoRecording:
                    CoPilot.CameraController.RecordStop();
                    this.SpeechContext = SpeechContext.Normal;
                    await Speak(sentences.StopRecording);
                    break;
                default:
                    await this.Listen();
                    break;
            }
        }

        /// <summary>
        /// Is tesxt confident?
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private Boolean IsTextConfidence(SpeechRecognitionConfidence result)
        {
            switch (result)
            {
                case SpeechRecognitionConfidence.Rejected:
                case SpeechRecognitionConfidence.Low:
                    return false;
                case SpeechRecognitionConfidence.High:
                case SpeechRecognitionConfidence.Medium:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get sentance from more possibilities
        /// </summary>
        /// <param name="sentences"></param>
        /// <returns></returns>
        private string getSentanceFromMorePossibilities(Sentence sentences)
        {
            Random rnd = new Random();
            int current = rnd.Next(sentences.Sentences.Count());
            return sentences.Sentences.ElementAt(current);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoPilot.Data
{
    [XmlRoot("speech")]
    public class Sentences
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <returns></returns>
        public static Sentences Load()
        {
            Uri uri = new Uri("Resources/Speech/Say/en-gb.xml", UriKind.Relative);
            var resource = App.GetResourceStream(uri);

            XmlSerializer xml = new XmlSerializer(typeof(Sentences));
            Sentences tmpSentences = xml.Deserialize(resource.Stream) as Sentences;

            resource.Stream.Close();
            resource.Stream.Dispose();

            return tmpSentences;
        }


        [XmlElement("start")]
        public Sentence Start { get; set; }

        [XmlElement("stop")]
        public Sentence Stop { get; set; }

        [XmlElement("say")]
        public Sentence Say { get; set; }

        [XmlElement("not-undestand")]
        public Sentence NotUndestand { get; set; }

        [XmlElement("take-shot")]
        public Sentence TakeShot { get; set; }

        [XmlElement("start-recording-now")]
        public Sentence StartRecordingNow { get; set; }

        [XmlElement("start-recording-info")]
        public Sentence StartRecordingInfo { get; set; }

        [XmlElement("stop-recording")]
        public Sentence StopRecording { get; set; }

        [XmlElement("add-fill")]
        public Sentence AddFill { get; set; }

        [XmlElement("add-repair")]
        public Sentence AddRepair { get; set; }

        [XmlElement("cancel")]
        public Sentence Cancel { get; set; }

        [XmlElement("try-it-again")]
        public Sentence TryItAgain { get; set; }

        [XmlElement("have-time-to-think")]
        public Sentence HaveTimeToThink { get; set; }

        [XmlElement("say-value")]
        public Sentence SayValue { get; set; }

        [XmlElement("more-then-fifty")]
        public Sentence MoreThenFifty { get; set; }

        [XmlElement("cannot-save")]
        public Sentence CannotSave { get; set; }

        [XmlElement("save-ok")]
        public Sentence SaveOk { get; set; }

        [XmlElement("writing")]
        public Sentence Writing { get; set; }
    }

    public class Sentence
    {
        [XmlElement("sentence")]
        public List<String> Sentences { get; set; }
    }
}

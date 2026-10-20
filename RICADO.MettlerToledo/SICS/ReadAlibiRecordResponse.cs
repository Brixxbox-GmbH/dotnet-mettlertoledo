using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RICADO.MettlerToledo.SICS
{
    // Multi-Line Response: "AMR A" + one Record Line + "AMR B" (or a single "AMR I" / "AMR L" Line)
    internal class ReadAlibiRecordResponse : Response
    {
        #region Constants

        private const string SuccessMessageRegex = "^AMR A\r\n(.+)\r\nAMR B$";
        private const string FailureMessageRegex = "^AMR [IL]";

        // Default Export Mode 0, e.g. 000000001;;DTM;"20/10/2026 18:41:49";Gross;"6.02 kg";Net;"6.02 kg";Tare;"PT 0.25 kg";SNO;"1";...
        private const string RecordRegex = "^([0-9]+);[^;]*;DTM;\"([^\"]*)\";Gross;\"([+\\-]?[0-9.]+) ([^\"]*)\";Net;\"([+\\-]?[0-9.]+) [^\"]*\";Tare;\"(PT )?([+\\-]?[0-9.]+) [^\"]*\"";
        private const string RecordIdRegex = "^([0-9]+)";

        #endregion


        #region Private Fields

        private long _recordId;
        private DateTime? _timestamp;
        private double? _grossWeight;
        private double? _netWeight;
        private double? _tareWeight;
        private bool _presetTare;
        private string _units;
        private string _record;

        #endregion


        #region Public Properties

        public long RecordId => _recordId;
        public DateTime? Timestamp => _timestamp;
        public double? GrossWeight => _grossWeight;
        public double? NetWeight => _netWeight;
        public double? TareWeight => _tareWeight;
        public bool PresetTare => _presetTare;
        public string Units => _units;
        public string Record => _record;

        #endregion


        #region Constructor

#if NETSTANDARD
        protected ReadAlibiRecordResponse(Request request, byte[] responseMessage) : base(request, responseMessage)
        {
        }
#else
        protected ReadAlibiRecordResponse(Request request, Memory<byte> responseMessage) : base(request, responseMessage)
        {
        }
#endif

        #endregion


        #region Public Methods

#if NETSTANDARD
        public static ReadAlibiRecordResponse UnpackResponseMessage(ReadAlibiRecordRequest request, byte[] responseMessage)
        {
            return new ReadAlibiRecordResponse(request, responseMessage);
        }
#else
        public static ReadAlibiRecordResponse UnpackResponseMessage(ReadAlibiRecordRequest request, Memory<byte> responseMessage)
        {
            return new ReadAlibiRecordResponse(request, responseMessage);
        }
#endif

        #endregion


        #region Protected Methods

        protected override void UnpackMessageDetail(Request request, string messageDetail)
        {
            Match successMatch = Regex.Match(messageDetail, SuccessMessageRegex, RegexOptions.Singleline);

            if (successMatch.Success == false)
            {
                if (Regex.IsMatch(messageDetail, FailureMessageRegex))
                {
                    throw new SICSException("The Read Alibi Record Request cannot be Executed at this Time or no Alibi Record exists");
                }

                throw new SICSException("The Read Alibi Record Response Message Format was Invalid");
            }

            _record = successMatch.Groups[1].Value;

            Match idMatch = Regex.Match(_record, RecordIdRegex);

            if (idMatch.Success == false || long.TryParse(idMatch.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out _recordId) == false)
            {
                throw new SICSException("Failed to Extract the Record ID from the Read Alibi Record Response");
            }

            // The Record Values are only available in the default Export Mode - other Modes / Separators still provide the Record ID and the raw Record
            Match recordMatch = Regex.Match(_record, RecordRegex);

            if (recordMatch.Success == false)
            {
                return;
            }

            if (DateTime.TryParseExact(recordMatch.Groups[2].Value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime timestamp))
            {
                _timestamp = timestamp;
            }

            _grossWeight = parseWeight(recordMatch.Groups[3].Value);
            _units = recordMatch.Groups[4].Value;
            _netWeight = parseWeight(recordMatch.Groups[5].Value);
            _presetTare = recordMatch.Groups[6].Success;
            _tareWeight = parseWeight(recordMatch.Groups[7].Value);
        }

        #endregion


        #region Private Methods

        private static double? parseWeight(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double weight))
            {
                return weight;
            }

            return null;
        }

        #endregion
    }
}

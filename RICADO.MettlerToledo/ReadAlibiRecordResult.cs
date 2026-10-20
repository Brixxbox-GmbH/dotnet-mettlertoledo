using System;

namespace RICADO.MettlerToledo
{
    public class ReadAlibiRecordResult : RequestResult
    {
        #region Private Fields

        private readonly long _recordId;
        private readonly DateTime? _timestamp;
        private readonly double? _grossWeight;
        private readonly double? _netWeight;
        private readonly double? _tareWeight;
        private readonly bool _presetTare;
        private readonly string _units;
        private readonly string _record;

        #endregion


        #region Public Properties

        /// <summary>
        /// The sequential Number of the Alibi Record (e.g. 1 for "000000001")
        /// </summary>
        public long RecordId => _recordId;

        /// <summary>
        /// The Values below are only available when the Indicator uses the default Alibi Export Mode (AMR MODE 0) and Separator (;)
        /// </summary>
        public DateTime? Timestamp => _timestamp;
        public double? GrossWeight => _grossWeight;
        public double? NetWeight => _netWeight;
        public double? TareWeight => _tareWeight;
        public bool PresetTare => _presetTare;
        public string Units => _units;

        /// <summary>
        /// The raw Alibi Record Line as sent by the Indicator
        /// </summary>
        public string Record => _record;

        #endregion


        #region Constructor

        internal ReadAlibiRecordResult(Channels.ProcessMessageResult result, long recordId, DateTime? timestamp, double? grossWeight, double? netWeight, double? tareWeight, bool presetTare, string units, string record) : base(result)
        {
            _recordId = recordId;
            _timestamp = timestamp;
            _grossWeight = grossWeight;
            _netWeight = netWeight;
            _tareWeight = tareWeight;
            _presetTare = presetTare;
            _units = units;
            _record = record;
        }

        #endregion
    }
}

using System;
using System.Text;

namespace RICADO.MettlerToledo.SICS
{
    internal class ReadAlibiRecordRequest : Request
    {
        #region Constructor

        protected ReadAlibiRecordRequest(string commandCode) : base(commandCode)
        {
        }

        #endregion


        #region Public Methods

#if NETSTANDARD
        public ReadAlibiRecordResponse UnpackResponseMessage(byte[] responseMessage)
        {
            return ReadAlibiRecordResponse.UnpackResponseMessage(this, responseMessage);
        }
#else
        public ReadAlibiRecordResponse UnpackResponseMessage(Memory<byte> responseMessage)
        {
            return ReadAlibiRecordResponse.UnpackResponseMessage(this, responseMessage);
        }
#endif

        public static ReadAlibiRecordRequest CreateNew(MettlerToledoDevice device)
        {
            return new ReadAlibiRecordRequest(Commands.ReadAlibiRecord);
        }

        #endregion


        #region Protected Methods

        protected override void BuildMessageDetail(ref StringBuilder messageBuilder)
        {
            messageBuilder.Append(" LAST");
        }

        #endregion
    }
}

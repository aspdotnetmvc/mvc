using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class TlvId
    {
        public static short TP_pid = 0x0001;
        public static short TP_udhi = 0x0002;
        public static short LinkID = 0x0003;
        public static short ChargeUserType = 0x0004;
        public static short ChargeTermType = 0x0005;
        public static short ChargeTermPseudo = 0x0006;
        public static short DestTermType = 0x0007;
        public static short DestTermPseudo = 0x0008;
        public static short PkTotal = 0x0009;
        public static short PkNumber = 0x000A;
        public static short SubmitMsgType = 0x000B;
        public static short SPDealReslt = 0x000C;
        public static short SrcTermType = 0x000D;
        public static short SrcTermPseudo = 0x000E;
        public static short NodesCount = 0x000F;
        public static short MsgSrc = 0x0010;
        public static short SrcType = 0x0011;
        public static short Mserviceid = 0x0012;
    }
}

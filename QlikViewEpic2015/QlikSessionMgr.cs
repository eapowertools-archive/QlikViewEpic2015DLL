using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QlikSenseEpic2015;
using System.Runtime.InteropServices;
using EBIFoundation82;
using ByteEncryptor;
using System.Security;
using System.Reflection;
using System.IO;
using System.Web;

namespace QlikSenseEpic2015
{
	[ComVisible(true)]
	[Guid("242746BA-B253-4D3F-B6DD-471EFFEDD33C")]
	[ClassInterface(ClassInterfaceType.None)]
	[ProgId("QlikViewEpic2015.QlikSessionMgr")]
	public class QlikSessionMgr : IBISessionManager
	{
		private static string KEY_FILENAME = Path.Combine(getDirectory, "qlikviewepic2015.config");
		private AESEncrypter Encryptor;
        private String encryptedString;
        private EBIFoundation82.IBIConfiguration BIConfiguration;
		string QlikServerUrl = "";
		string BiUserId = "";
		string handshake = "";
		string key = "";
        string iv = "";


		public QlikSessionMgr()
		{
			using (var reader = new StreamReader(KEY_FILENAME))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("key="))
					{
						key = line.Substring(4);
					}

                    if(line.StartsWith("iv="))
                    {
                        iv = line.Substring(3);
                    }
				}
			}

			if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(iv))
			{
				throw new FileNotFoundException(string.Format("Could not read key value pair 'key' or 'iv' from '{0}'.", KEY_FILENAME));
			}
			Encryptor = new AESEncrypter(key);

		}

		public static string getDirectory
		{
			get
			{
				string codeBase = AppDomain.CurrentDomain.BaseDirectory;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

		public string ErrorMessage
		{
			get
			{
				return "Qlik integration failed login.";
			}
		}

		public bool LogIn()
		{

			if (String.IsNullOrEmpty(BiUserId))
			{
				BiUserId = "sqlviews\\jog";
			}

			if (String.IsNullOrEmpty(QlikServerUrl))
			{
				QlikServerUrl = "http://qlikview.112adams.local";
			}

			return true;
		}
		public bool LogOff()
		{
			//Code to implement LogOff
			return true;
		}
		public string GetURL()
		{
			byte[] ivValue = Encoding.ASCII.GetBytes(iv);
			byte[] sharedSecret = Encoding.ASCII.GetBytes(key);
			string finalURL = string.Format("{0}/qvajaxzfc/epicwebticket.aspx?token={1}", QlikServerUrl, HttpUtility.UrlEncode(Encryptor.EncryptString(BiUserId, sharedSecret, ivValue)));
			return finalURL;
		}
		public bool IsLoggedIn()
		{
			//Code to implement IsLoggedIn
			//BiUserId = BIConfiguration.get_BIUserName();
			//QlikViewServerUrl = BIConfiguration.get_BIServerName();
			return true;
		}

		public void set_Configuration(ref _IBIConfiguration value)
		{
			BIConfiguration = (EBIFoundation82.IBIConfiguration)value;
			QlikServerUrl = BIConfiguration.get_BIURL();
			BiUserId = BIConfiguration.get_BIUserName();

		}



	}
}

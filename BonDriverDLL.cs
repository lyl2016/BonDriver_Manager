using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// BonDriverDLL类
/// </summary>
namespace BonDriver_Manager
{
	/// <summary>
	/// BonDriver物理DLL类
	/// </summary>
	class BonDriverDLL
	{
		/// <summary>
		/// BonDriverDLL总量
		/// </summary>
		public static int BonDriverCount = 0;
		/// <summary>
		/// 构造函数
		/// </summary>
		public BonDriverDLL(string fileName, string region, ushort index, string tuner, ushort tunerIndex, bool sa, ushort saPort, string ip, string tunerPath, ChSet4 chSet4 = null)
		{
			this.fileName = fileName;
			this.region = region;
			this.index = index;
			this.tuner = tuner;
			this.tunerIndex = tunerIndex;
			this.sa = sa;
			this.saPort = saPort;
			this.ip = ip;
			this.tunerPath = tunerPath;
			List<Channel> channels = new List<Channel>();
			string saString;
			if (!sa)
			{
				saString = "T";
			}
			else
			{
				saString = "S";
			}
			string chSet4FileName = fileName.Replace(".dll", "(Spinel：") + this.tuner + "_" + this.tunerIndex + "／" + saString + this.saPort + ").ChSet4.txt";
			try
			{
				string line;
				StreamReader chSet4Reader = new StreamReader(@"./Setting/" + chSet4FileName, Encoding.GetEncoding("Shift-JIS"));
				while ((line = chSet4Reader.ReadLine()) != null)
				{
					string channel = line.Split('\t')[0];
					string channelName = line.Split('\t')[1];
					string networkName = line.Split('\t')[2];
					ushort tunerSpace = (ushort)Convert.ToInt32(line.Split('\t')[3]);
					ushort channelIndex = (ushort)Convert.ToInt32(line.Split('\t')[4]);
					ushort sid = (ushort)Convert.ToInt32(line.Split('\t')[7]);
					ushort onid = (ushort)Convert.ToInt32(line.Split('\t')[5]);
					ushort tsid = (ushort)Convert.ToInt32(line.Split('\t')[6]);
					ushort type = (ushort)Convert.ToInt32(line.Split('\t')[8]);
					bool show;
					if (Convert.ToInt32(line.Split('\t')[10]) == 1)
					{
						show = true;
					}
					else
					{
						show = false;
					}
					channels.Add(new Channel(channel, channelName, networkName, tunerSpace, channelIndex, sid, onid, tsid, type, show));
				}
				chSet4Reader.Close();
			}
			catch(Exception ex)
			{
#if DEBUG
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("BonDriver: " + this.fileName + "暂未搜台\r\n" + ex.Message);
#else
				Console.WriteLine("BonDriver: " + this.fileName + "\r\n暂未搜台");
#endif
				Console.ResetColor();
			}
			ChSet4 c = new ChSet4(chSet4FileName, channels);
			this.chSet4 = c;
		}
		/// <summary>
		/// 当前BonDriverDLL的文件名
		/// </summary>
		public string fileName;
		/// <summary>
		/// 地区
		/// </summary>
		public string region;
		/// <summary>
		/// 序号
		/// </summary>
		public ushort index;
		/// <summary>
		/// 卡型（例：PT3）
		/// </summary>
		public string tuner;
		/// <summary>
		/// 卡型序号
		/// </summary>
		public ushort tunerIndex;
		/// <summary>
		/// 卫星指示器（False=地面波，True=卫星）
		/// </summary>
		public bool sa;
		/// <summary>
		/// 卡型端口号
		/// </summary>
		public ushort saPort;
		/// <summary>
		/// 机主IP地址
		/// </summary>
		public string ip;
		/// <summary>
		/// TunerPath
		/// </summary>
		string tunerPath;
		/// <summary>
		/// BonDriverDLL关联的ChSet4.txt信息
		/// </summary>
		public ChSet4 chSet4;
		/// <summary>
		/// 重载ToString()方法，将输出格式化为BonDriverDLL配套ini的Address行以及TunerPath行
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string sa = null;
			if (this.sa)
			{
				sa = "S";
			}
			else
			{
				sa = "T";
			}
			return "Address = \"" + this.ip + "\"\r\n"
				+ "TunerPath = \"" + this.tuner + "/" + this.tunerIndex + "/" + sa + "/" + this.saPort + "\"";
		}
		/// <summary>
		/// 生成BonDriver物理DLL
		/// </summary>
		/// <param name="region">地区</param>
		/// <param name="index">序号</param>
		/// <param name="tuner">卡型</param>
		/// <param name="tunerIndex">卡型序号</param>
		/// <param name="sa">卫星指示器</param>
		/// <param name="ip">机主IP地址，包括端口号</param>
		/// <returns></returns>
		public static List<BonDriverDLL> GenBonDriver(string region, ushort index, string tuner, ushort tunerIndex, bool sa, string ip)
		{
			List<BonDriverDLL> returnValue = new List<BonDriverDLL>();
			int tCount = 0, sCount = 0;
			for (int i = 0; i < Program.tuner_counter; i++)
			{
				if (tuner.Equals(Program.tunerTypes[i, 0]))
				{
					tCount = Convert.ToInt32(Program.tunerTypes[i, 1]);
					sCount = Convert.ToInt32(Program.tunerTypes[i, 2]);
					break;
				}
			}
			string pLocalFilePath = "./BonDriver/BonDriver_Spinel_test.dll";//要复制的文件路径
			string pSaveFilePath;
			for (ushort t = 0; t < tCount; t++)
			{
				pSaveFilePath = "./BonDriver/BonDriver_" + region + "_" + tuner + "_" + index + "_T_" + t + ".dll";//指定存储的路径
				if (File.Exists(pLocalFilePath))//必须判断要复制的文件是否存在
				{
					File.Copy(pLocalFilePath, pSaveFilePath, true);//三个参数分别是源文件路径，存储路径，若存储路径有相同文件是否替换
				}
				string ini_path = pSaveFilePath + ".ini";
				using (StreamWriter sw = new StreamWriter(ini_path))
				{
					sw.WriteLine("[BonDriver_Spinel]\r\nIniVersion = 1");
					sw.WriteLine("Address = \"" + ip + ":48083\"");
					sw.WriteLine("TunerPath = \"" + tuner + "/" + tunerIndex.ToString() + "/T/" + t + "\"");
					sw.WriteLine("RequireExclusiveChannelControl = 0\r\n" +
									"ForceTCPDataLinkMode = 1\r\n" +
									"EnableHostProcessAliveCheck = 0\r\n" +
									"ConnectTimeoutSeconds = 10\r\n" +
									"DesiredDescrambleControl = 2");
				}
				BonDriverDLL b = new BonDriverDLL("BonDriver_" + region + "_" + tuner + "_" + index + "_T_" + t + ".dll", region, index, tuner, tunerIndex, false, t, ip + ":48083", tuner + "/" + tunerIndex + "/T/" + t);
				returnValue.Add(b);
				Program.bonDriverDLLs.Add(b);
			}
			if (sa)
			{
				for (ushort s = 0; s < sCount; s++)
				{
					pSaveFilePath = "./BonDriver/BonDriver_" + region + "_" + tuner + "_" + index + "_S_" + s + ".dll";//指定存储的路径
					if (File.Exists(pLocalFilePath))//必须判断要复制的文件是否存在
					{
						File.Copy(pLocalFilePath, pSaveFilePath, false);//三个参数分别是源文件路径，存储路径，若存储路径有相同文件是否替换
					}
					string ini_path = pSaveFilePath + ".ini";
					using (StreamWriter sw = new StreamWriter(ini_path))
					{
						sw.WriteLine("[BonDriver_Spinel]\r\nIniVersion = 1");
						sw.WriteLine("Address = \"" + ip + ":48083\"");
						sw.WriteLine("TunerPath = \"" + tuner + "/" + tunerIndex.ToString() + "/S/" + s + "\"");
						sw.WriteLine("RequireExclusiveChannelControl = 0\r\n" +
										"ForceTCPDataLinkMode = 1\r\n" +
										"EnableHostProcessAliveCheck = 0\r\n" +
										"ConnectTimeoutSeconds = 10\r\n" +
										"DesiredDescrambleControl = 2");
					}
					BonDriverDLL b = new BonDriverDLL("BonDriver_" + region + "_" + tuner + "_" + index + "_S_" + s + ".dll", region, index, tuner, tunerIndex, true, s, ip + ":48083", tuner + "/" + tunerIndex + "/T/" + s);
					returnValue.Add(b);
					Program.bonDriverDLLs.Add(b);
				}
			}
			return returnValue;
		}
		/// <summary>
		/// 生成当前对象的BonDriverDLL文件，存放于./BonDriver/目录中
		/// </summary>
		/// <returns></returns>
		public string GenBonDriver()
		{
			string pLocalFilePath = "./BonDriver/BonDriver_Spinel_test.dll";//要复制的文件路径
			string pSaveFilePath;
			if (!this.sa)
			{
				pSaveFilePath = "./BonDriver/BonDriver_" + this.region + "_" + this.tuner + "_" + this.index + "_T_" + this.saPort + ".dll";//指定存储的路径
			}
			else
			{
				pSaveFilePath = "./BonDriver/BonDriver_" + this.region + "_" + this.tuner + "_" + this.index + "_S_" + this.saPort + ".dll";//指定存储的路径
			}
			if (File.Exists(pLocalFilePath))//必须判断要复制的文件是否存在
			{
				File.Copy(pLocalFilePath, pSaveFilePath, false);//三个参数分别是源文件路径，存储路径，若存储路径有相同文件是否替换
			}
			string ini_path = pSaveFilePath + ".ini";
			using (StreamWriter sw = new StreamWriter(ini_path))
			{
				sw.WriteLine("[BonDriver_Spinel]\r\nIniVersion = 1");
				sw.WriteLine(this.ToString());
				sw.WriteLine("RequireExclusiveChannelControl = 0\r\n" +
								"ForceTCPDataLinkMode = 1\r\n" +
								"EnableHostProcessAliveCheck = 0\r\n" +
								"ConnectTimeoutSeconds = 10\r\n" +
								"DesiredDescrambleControl = 2");
			}
			return pSaveFilePath.Replace("./BonDriver/", "");
		}
	}
}

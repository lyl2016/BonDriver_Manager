using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace BonDriver_Manager
{
	class Program
	{
		/// <summary>
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			Console.Title = name + ' ' + ver;
			Console.WriteLine(name + "\t" + ver);
			//读取机型
			string line;
			StreamReader tunerReader = new StreamReader(@"tuner.txt");
			while ((line = tunerReader.ReadLine()) != null)
			{
				//参数解析
				string argu = line;
				string[] sArray = argu.Split(',');
				//System.Console.WriteLine(line);
				tunerTypes[tuner_counter, 0] = sArray[0];
				tunerTypes[tuner_counter, 1] = sArray[1];
				tunerTypes[tuner_counter, 2] = sArray[2];
				tuner_counter++;
			}
			tunerReader.Close();
			List<BonDriver_EpgTimerSrv> bonDriver_EpgTimerSrvs = new List<BonDriver_EpgTimerSrv>();
			List<BonDriverDLL> bonDriverDLLs = new List<BonDriverDLL>();
			StreamReader srvReader = new StreamReader(@"EpgTimerSrv.ini");
			while ((line = srvReader.ReadLine()) != null)
			{
				if (line.IndexOf("[BonDriver_") < 0)
				{
					continue;
				}
				else
				{
					BonDriver_EpgTimerSrv.BonDriverCount++;
					BonDriver_EpgTimerSrv b_Srv = new BonDriver_EpgTimerSrv();
					b_Srv.fileName = line.Replace("[", "").Replace("]", "");
					string count_string = srvReader.ReadLine();
					string getepg_string = srvReader.ReadLine();
					string epgcount_string = srvReader.ReadLine();
					string priority = srvReader.ReadLine();
					b_Srv.count = Convert.ToInt16(count_string.Split('=')[1]);
					if (Convert.ToInt16(getepg_string.Split('=')[1]) == 0)
					{
						b_Srv.epg = false;
					}
					else
					{
						b_Srv.epg = true;
					}
					b_Srv.priority = Convert.ToInt32(priority.Split('=')[1]);
					b_Srv.enabled = true;
					bonDriver_EpgTimerSrvs.Add(b_Srv);
				}
			}
			srvReader.Close();
			bonDriver_EpgTimerSrvs.Sort((l, r) => l.priority.CompareTo(r.priority));
			foreach (BonDriver_EpgTimerSrv b_Srv in bonDriver_EpgTimerSrvs)
			{
				//Console.Write(b_Srv.ToString());
				try
				{
					StreamReader bonDLLReader = new StreamReader("./BonDriver/" + b_Srv.fileName + ".ini");
					while ((line = bonDLLReader.ReadLine()) != null)
					{
						string ip_string = null;
						string tunerpath = null;
						if (line.IndexOf("Address") >= 0)
						{
							ip_string = line.Split('=')[1].Replace(" ", "").Replace("\"", "");
							tunerpath = bonDLLReader.ReadLine().Split('=')[1].Replace(" ", "").Replace("\"", "");
                        }
                        else
                        {
							continue;
                        }
						BonDriverDLL bonDriverDLL = new BonDriverDLL();
						bonDriverDLL.fileName = b_Srv.fileName;
						bonDriverDLL.ip = ip_string;
						bonDriverDLL.tuner = tunerpath.Split('/')[0];
						bonDriverDLL.tunerIndex = Convert.ToInt16(tunerpath.Split('/')[1]);
						if (tunerpath.Split('/')[2].Equals("S"))
						{
							bonDriverDLL.sa = true;
						}
						else
						{
							bonDriverDLL.sa = false;
						}
						bonDriverDLL.saPort = Convert.ToInt16(tunerpath.Split('/')[3]);
						bonDriverDLLs.Add(bonDriverDLL);
						BonDriverDLL.BonDriverCount++;
						//Console.WriteLine(bonDriverDLL.ToString());
					}
					bonDLLReader.Close();
				}
				catch
				{
					Console.BackgroundColor = ConsoleColor.Red;
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("./BonDriver/" + b_Srv.fileName + ".ini 文件不存在！");
					Console.ResetColor();
				}
			}
			Console.WriteLine("总BonDriverSrv数量：" + BonDriver_EpgTimerSrv.BonDriverCount + " 总BonDriverDLL数量：" + BonDriverDLL.BonDriverCount);
			if (BonDriver_EpgTimerSrv.BonDriverCount != BonDriverDLL.BonDriverCount)
            {
				Console.BackgroundColor = ConsoleColor.Red;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("总BonDriverSrv数量不等于BonDriverDLL数量，请检查配置异常。");
				Console.ResetColor();
			}
			Console.ReadLine();
		}

		/// <summary>
		/// 重新启动程序，旧窗口并不会关闭
		/// </summary>
		public static void Restart()
		{
			var rs = new Process();
			rs.StartInfo.FileName = "BonDriver_Manager.exe";
			rs.Start();
		}

#if DEBUG
		public static readonly string name = "[DEBUG]二枚目BonDriver管理程序";
#else
		public static readonly string name = "二枚目BonDriver管理程序";
#endif
		public static readonly Assembly assembly = Assembly.GetExecutingAssembly();
		public static readonly AssemblyName assemblyName = assembly.GetName();
		public static readonly Version version = assemblyName.Version;
#if DEBUG
		public static readonly string publishChannel = "测试版";
#else
		public static readonly string publishChannel = "正式版";
#endif
		public static readonly string ver =
			version.Major.ToString() + "." +
			version.Minor.ToString() + "." +
			version.Build.ToString() + "." +
			version.Revision.ToString() + " " +
			publishChannel;

		/// <summary>
		/// 日本行政区划，按ISO 3166-2:JP标记
		/// </summary>
		public static readonly string[,] jpregion =
		{
			{ "日本", "Japan"},
			{ "北海道", "Hokaido"},
			{ "青森", "Aomori"},
			{ "岩手", "Iwate"},
			{ "宫城", "Miyagi"},
			{ "秋田", "Akita"},
			{ "山形", "Yamagata"},
			{ "福岛", "Fukushima"},
			{ "茨城", "Ibaraki"},
			{ "栃木", "Tochigi"},
			{ "群马", "Gunma"},
			{ "埼玉", "Saitama"},
			{ "千叶", "Chiba"},
			{ "东京", "Tokyo"},
			{ "神奈川", "Kanakawa"},
			{ "新潟", "Niigata"},
			{ "富山", "Toyama"},
			{ "石川", "Ishikawa"},
			{ "福井", "Fukui"},
			{ "山梨", "Yamanashi"},
			{ "长野", "Nagano"},
			{ "岐阜", "Gifu"},
			{ "静冈", "Shizuoka"},
			{ "爱知", "Aichi"},
			{ "三重", "Mie"},
			{ "滋贺", "Shiga"},
			{ "京都", "Kyoto"},
			{ "大阪", "Osaka"},
			{ "兵库", "Hyogo"},
			{ "奈良", "Nara"},
			{ "和歌山", "Wakayama"},
			{ "鸟取", "Totori"},
			{ "岛根", "Shimane"},
			{ "冈山", "Okayama"},
			{ "广岛", "Hiroshima"},
			{ "山口", "Yamaguchi"},
			{ "德岛", "Tokushima"},
			{ "香川", "Kagawa"},
			{ "爱媛", "Eihime"},
			{ "高知", "Takachi"},
			{ "福冈", "Fukuoka"},
			{ "佐贺", "Saga"},
			{ "长崎", "Nagazaki"},
			{ "熊本", "Kumamoto"},
			{ "大分", "Ooita"},
			{ "宫崎", "Miyazaki"},
			{ "鹿儿岛", "Kagoshima"},
			{ "冲绳", "Okinawa"},
		};
		public static int tuner_counter = 0;
		const int num_tunerTypes = 128;
		public static string[,] tunerTypes = new string[num_tunerTypes, 3];//二维数组，每行保存机型名以及其支持的T/S数量
	}

	/// <summary>
	/// EpgTimerSrv的BonDriver调用类，用于控制顺序以及搜台等等
	/// </summary>
	class BonDriver_EpgTimerSrv
	{
		/// <summary>
		/// BonDriverSrv总量
		/// </summary>
		public static int BonDriverCount = 0;
		public BonDriver_EpgTimerSrv()
		{
			//构造BonDriver_EpgTimerSrv
		}
		/// <summary>
		/// BonDriver的物理文件名
		/// </summary>
		public string fileName;
		/// <summary>
		/// 录制调用的排序，从0起记
		/// </summary>
		public int priority;
		/// <summary>
		/// 是否在刷新EPG时使用该BonDriver
		/// </summary>
		public bool epg = false;
		/// <summary>
		/// BonDriver内含的Tuner数量，默认为1
		/// </summary>
		public short count = 1;
		/// <summary>
		/// 按大区划分的区域归属，关东=0，关西=1，名古屋=2，地方台=3
		/// </summary>
		public int regionArea;
		/// <summary>
		/// 按大区划分后BonDriver的排序
		/// </summary>
		public int indexArea;
		/// <summary>
		/// BonDriver启用状态
		/// </summary>
		public bool enabled;
		/// <summary>
		/// 重载ToString方法，用于后续生成ini文件
		/// </summary>
		/// <returns>适配了ini格式的字符串</returns>
		public override string ToString()
		{
			string getepg = null;
			if (this.epg)
			{
				getepg = "GetEPG=1";
			}
			else
			{
				getepg = "GetEPG=0";
			}
			return "[" + this.fileName + "]\r\n"
				+ "Count=" + this.count.ToString() + "\r\n"
				+ getepg + "\r\n"
				+ "EPGCount=0" + "\r\n"
				+ "Priority=" + this.priority.ToString() + "\r\n";
		}
	}
	/// <summary>
	/// BonDriverDLL类
	/// </summary>
	class BonDriverDLL
	{
		/// <summary>
		/// BonDriverDLL总量
		/// </summary>
		public static int BonDriverCount = 0;
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
		public short index;
		/// <summary>
		/// 卡型（例：PT3）
		/// </summary>
		public string tuner;
		/// <summary>
		/// 卡型序号
		/// </summary>
		public short tunerIndex;
		/// <summary>
		/// 卫星指示器（False=地面波，True=卫星）
		/// </summary>
		public bool sa;
		/// <summary>
		/// 卡型端口号
		/// </summary>
		public short saPort;
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
		public List<ChSet4> chSet4s;
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
		/// <param name="tuner">卡型</param>
		/// <param name="tunerIndex">卡型序号</param>
		/// <param name="sa">卫星指示器</param>
		/// <param name="index">序号</param>
		/// <param name="ip">机主IP地址</param>
		/// <returns></returns>
		public int[] GenBonDriver(string region, string tuner, int tunerIndex, bool sa, short index, string ip)
		{
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
			string pLocalFilePath = "./BonDriver_Spinel_test.dll";//要复制的文件路径
			string pSaveFilePath;
			for (int t = 0; t < tCount; t++)
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
			}
			if (sa)
			{
				for (int s = 0; s < sCount; s++)
				{
					pSaveFilePath = "./BonDriver/BonDriver_" + region + "_" + tuner + "_" + index + "_S_" + s + ".dll";//指定存储的路径
					if (File.Exists(pLocalFilePath))//必须判断要复制的文件是否存在
					{
						File.Copy(pLocalFilePath, pSaveFilePath, true);//三个参数分别是源文件路径，存储路径，若存储路径有相同文件是否替换
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
				}
			}
			int[] returnValue = new int[2] { tCount, sCount };
			return returnValue;
		}
	}

	/// <summary>
	/// 频道ChSet4信息
	/// </summary>
	class ChSet4
	{
		/// <summary>
		/// 频道名
		/// </summary>
		string channelName;
		/// <summary>
		/// 所属网络
		/// </summary>
		string networkName;
		short sid;
		short tsid;
		short onid;
	}
}

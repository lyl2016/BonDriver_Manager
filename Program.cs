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
			//读取机型
			string line;
			try
			{
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
			}
			catch
			{
				Console.WriteLine("tuner.txt读取失败，请检查文件状态");
			}
			
			List<BonDriverSrv> bonDriverSrvs = new List<BonDriverSrv>();
			List<BonDriverDLL> bonDriverDLLs = new List<BonDriverDLL>();
			try
			{
				StreamReader srvReader = new StreamReader(@"EpgTimerSrv.ini");
				while ((line = srvReader.ReadLine()) != null)
				{
					if (line.IndexOf("[BonDriver_") < 0)
					{
						continue;
					}
					else
					{
						BonDriverSrv.BonDriverCount++;
						BonDriverSrv b_Srv = new BonDriverSrv();
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
						bonDriverSrvs.Add(b_Srv);
					}
				}
				srvReader.Close();
			}
			catch
			{
				Console.WriteLine("EpgTimerSrv.ini读取失败，请检查文件状态");
			}
			
			bonDriverSrvs.Sort((l, r) => l.priority.CompareTo(r.priority));
			foreach (BonDriverSrv bonDiverSrv in bonDriverSrvs)
			{
				//Console.Write(bonDiverSrv.ToString());
				try
				{
					StreamReader bonDLLReader = new StreamReader("./BonDriver/" + bonDiverSrv.fileName + ".ini");
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
						bonDriverDLL.fileName = bonDiverSrv.fileName;
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
						bonDiverSrv.driverDLL = bonDriverDLL;
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
					Console.WriteLine("./BonDriver/" + bonDiverSrv.fileName + ".ini 文件不存在！");
					bonDiverSrv.fileName = "NULL";
					Console.ResetColor();
				}
			}
			Console.WriteLine("总BonDriverSrv数量：" + BonDriverSrv.BonDriverCount + " 总BonDriverDLL数量：" + BonDriverDLL.BonDriverCount);
			if (BonDriverSrv.BonDriverCount != BonDriverDLL.BonDriverCount)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("总BonDriverSrv数量不等于BonDriverDLL数量，请检查配置异常。");
				Console.ResetColor();
			}
			Console.ReadLine();
			Console.Clear();
			Console.WriteLine(name + "\t" + ver);
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

		public static void LoadMenu()
		{
			Console.WriteLine("01.\t遍历查看所有BonDriver信息");
			Console.WriteLine("02.\t添加新的BonDriver信息");
			Console.WriteLine("03.\t清理无效的BonDriverSrv");
		}

		/// <summary>
		/// 加载BonDriverDLL类相关操作菜单
		/// </summary>
		/// <param name="b">待操作的BonD river DLL</param>
		public static void LoadBonDriverControlMenu(BonDriverDLL b)
		{
			Console.WriteLine("00.\t新建BonDriver");
			Console.WriteLine("01.\t编辑BonDriver_*.ini（IP以及TunerPath）");
			Console.WriteLine("02.\t编辑EpgTimerSrv.ini（DLL排序以及EPG调用）");
			Console.WriteLine("03.\t移动BonDriver使用顺序");
			Console.WriteLine("04.\t删除BonDriver");
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
}

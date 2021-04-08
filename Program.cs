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
		/// 程序入口
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
			try
			{
				StreamReader srvReader = new StreamReader("EpgTimerSrv.ini");
				while ((line = srvReader.ReadLine()) != null)
				{
					if (line.IndexOf("[BonDriver_") < 0)
					{
						continue;
					}
					else
					{
						BonDriverSrv.BonDriverCount++;
						string fileName = line.Replace("[", "").Replace("]", "");
						string count_string = srvReader.ReadLine();
						string getepg_string = srvReader.ReadLine();
						string epgcount_string = srvReader.ReadLine();
						int priority = Convert.ToInt32(srvReader.ReadLine().Split('=')[1]);
						ushort count = (ushort)Convert.ToInt32(count_string.Split('=')[1]);
						bool enabled = true;
						bool epg;
						if (Convert.ToInt32(getepg_string.Split('=')[1]) == 0)
						{
							epg = false;
						}
						else
						{
							epg = true;
						}
						BonDriverSrv b_Srv = new BonDriverSrv(fileName, priority, epg, count, enabled, null);
						if (b_Srv.priority > BonDriverSrv.PriorityMax)
						{
							BonDriverSrv.PriorityMax++;
						}
						bonDriverSrvs.Add(b_Srv);
					}
				}
				srvReader.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("EpgTimerSrv.ini读取失败，请检查文件状态\r\n" + ex.ToString());
			}
			bonDriverSrvs.Sort((l, r) => l.priority.CompareTo(r.priority));
			foreach (BonDriverSrv bonDiverSrv in bonDriverSrvs)
			{
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
						bool sa = false;
						if (tunerpath.Split('/')[2].Equals("S"))
						{
							sa = true;
						}
						else
						{
							sa = false;
						}
						BonDriverDLL bonDriverDLL = new BonDriverDLL(bonDiverSrv.fileName, bonDiverSrv.fileName.Split('_')[1], (ushort)Convert.ToInt32(bonDiverSrv.fileName.Replace("Spinel_", "").Split('_')[3]), tunerpath.Split('/')[0], (ushort)Convert.ToInt32(tunerpath.Split('/')[1]), sa, (ushort)Convert.ToInt32(tunerpath.Split('/')[3]), ip_string, tunerpath);
						bonDiverSrv.driverDLL = bonDriverDLL;
						bonDriverDLLs.Add(bonDriverDLL);
						BonDriverDLL.BonDriverCount++;
					}
					bonDLLReader.Close();
				}
				catch
				{
					Console.BackgroundColor = ConsoleColor.Red;
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("./BonDriver/" + bonDiverSrv.fileName + ".ini 文件不存在！");
					Console.ResetColor();
				}
			}
			Console.WriteLine("总BonDriverSrv数量：" + BonDriverSrv.BonDriverCount + " 总BonDriverDLL数量：" + BonDriverDLL.BonDriverCount + "\r\n数据加载完毕。");
			if (BonDriverSrv.BonDriverCount != BonDriverDLL.BonDriverCount)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("总BonDriverSrv数量不等于BonDriverDLL数量，请检查配置异常。");
				Console.ResetColor();
			}
			Console.ReadLine();
			while (true)
			{
				Console.Clear();
				LoadMenu();
			}
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
		/// <summary>
		/// 加载主菜单
		/// </summary>
		public static void LoadMenu()
		{
			Console.WriteLine(name + ' ' + ver);
			Console.WriteLine("01.\t遍历查看所有BonDriver信息");
			Console.WriteLine("02.\t添加新的BonDriver信息（此操作将覆盖旧有同BonDriver，请谨慎使用）");
			Console.WriteLine("03.\t清理无效的BonDriverSrv");
			char key = Convert.ToChar(Console.ReadLine());
			switch (key)
			{
				case '1':
					{
						DumpBonDriverInfo_LoadMenu01();
						break;
					}

				case '2':
					{
						AddBonDriver_LoadMenu02();
						break;
					}

				case '3':
					{
						RemoveBonDriverSrv_LoadMenu03();
						break;
					}
			}
		}
		/// <summary>
		/// 主菜单第一项，遍历查看所有BonDriver信息
		/// </summary>
		public static void DumpBonDriverInfo_LoadMenu01()
		{
			int i = 0;
			foreach (BonDriverSrv b in bonDriverSrvs)
			{
				Console.WriteLine("========BonDriverDLL: " + i.ToString() + "========");
				Console.WriteLine(b.ToString());
				if (b.driverDLL == null)
				{
					Console.WriteLine("[BonDriverDLL实体不存在]");
				}
				else
				{
					Console.WriteLine("====IP信息====");
					Console.WriteLine(b.driverDLL.ToString());
                    if (b.driverDLL.chSet4 != null)
                    {
						Console.WriteLine("==BonDriverDLL实体频道列表==", b.driverDLL.chSet4.chSet4FileName);
						ushort channelCount = 0;
						foreach(Channel c in b.driverDLL.chSet4.channels)
                        {
                            if (c.show)
                            {
								Console.WriteLine(c.ToString());
							}
							channelCount++;
						}
						Console.WriteLine("BonDriver: " + b.fileName + "共有 " + channelCount + " 个频道");
                    }
                    else
                    {
						Console.WriteLine("BonDriverDLL未搜台");
					}
				}
				i++;
				Console.ReadLine();
			}
			Console.WriteLine("操作已完成01");
			Console.ReadLine();
			Console.Clear();
		}
		/// <summary>
		/// 主菜单第二项，添加新的BonDriver信息
		/// 该函数由控制台输入信息，完成DLL新建的动作，并在Srvs中写入数据
		/// TODO: 
		/// 1. 完成EpgTimerSrv.ini信息写入
		/// 2. 完成与EpgDataCap_Bon的联动，创建DLL动作完成后打开EDCB进行搜台操作
		/// 3. 完成ChSet4频道列表的导入
		/// </summary>
		public static void AddBonDriver_LoadMenu02()
		{
			Console.Write("请输入BonDriver所在地区：");
			string newRegion = Console.ReadLine();
			Console.Write("请输入BonDriver编号：");
			string newIndex = Console.ReadLine();
			Console.Write("请输入BonDriver卡型：");
			string newTuner = Console.ReadLine();
			Console.Write("请输入BonDriver同卡型的排序（PT3/2/T/1中的2）：");
			ushort newTunerIndex = (ushort)Convert.ToInt32(Console.ReadLine());
			Console.Write("请输入BonDriver的地址，需要带端口号（127.0.0.1:12121）：");
			string newIP = Console.ReadLine();
			foreach (BonDriverDLL b in BonDriverDLL.GenBonDriver(newRegion, (ushort)Convert.ToInt32(newIndex), newTuner, newTunerIndex, true, newIP))
			{
				BonDriverSrv b_Srv = new BonDriverSrv(b.fileName, BonDriverSrv.PriorityMax, false, 1, true, b);
				BonDriverSrv.PriorityMax++;
				bonDriverSrvs.Add(b_Srv);
			}
			Console.WriteLine("操作已完成02");
			Console.ReadLine();
			Console.Clear();
		}
		/// <summary>
		/// 主菜单第三项，清理无效的BonDriverSrv
		/// 此操作用于移除EpgTimerSrv.ini中无效的条目
		/// TODO: 
		/// 1. 将更改写入EpgTimerSrv.ini
		/// </summary>
		public static void RemoveBonDriverSrv_LoadMenu03()
		{
			ushort i = 0;
			int countB = bonDriverSrvs.Count;
            bool IsDLLNull(BonDriverSrv b) { return b.driverDLL == null; }
            bonDriverSrvs.RemoveAll(IsDLLNull);
			int countA = bonDriverSrvs.Count;
			foreach(BonDriverSrv b in bonDriverSrvs)
            {
				b.priority = i;
				i++;
            }
			Console.WriteLine("本次共计移除了 " + (countB - countA) + "个条目");
			Console.WriteLine("操作已完成03");
			Console.ReadLine();
			Console.Clear();
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
		public static List<BonDriverSrv> bonDriverSrvs = new List<BonDriverSrv>();
		public static List<BonDriverDLL> bonDriverDLLs = new List<BonDriverDLL>();
	}
}

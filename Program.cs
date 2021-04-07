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
		/// TODO: 
		/// 1. 加载Main方法时加载BonDriverSrv
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
			BonDriver_EpgTimerSrv bonDriver_EpgTimerSrv = new BonDriver_EpgTimerSrv("./EpgTimerSrv.ini");
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
		public static int BonDriverCount = 0;
		public BonDriver_EpgTimerSrv(string EpgTimerSrvFilePath)
		{
			//读取EpgtimerSrv.ini
		}
		~BonDriver_EpgTimerSrv()
        {

        }
		/// <summary>
		/// BonDriver的物理文件名
		/// </summary>
		string fileName;
		/// <summary>
		/// 录制调用的排序，从0起记
		/// </summary>
		int index;
		/// <summary>
		/// 是否在刷新EPG时使用该BonDriver
		/// </summary>
		bool epg = false;
		/// <summary>
		/// BonDriver内含的Tuner数量，默认为1
		/// </summary>
		short count = 1;
		/// <summary>
		/// 按大区划分的区域归属，关东=0，关西=1，名古屋=2，地方台=3
		/// </summary>
		int regionArea;
		/// <summary>
		/// 按大区划分后BonDriver的排序
		/// </summary>
		int indexArea;
		/// <summary>
		/// BonDriver启用状态
		/// </summary>
		bool enabled;
		/// <summary>
		/// 链表上一个元素指针域
		/// </summary>
		BonDriver_EpgTimerSrv previous;
		/// <summary>
		/// 链表下一个元素指针域
		/// </summary>
		BonDriver_EpgTimerSrv next;
		/// <summary>
		/// 当前BonDriverSrv所对应的BonDriverDLL
		/// </summary>
		BonDriverDLL driverDLL;
		/// <summary>
		/// 移动BonDriver次序
		/// TODO: 
		/// 1. 追加地区排序
		/// </summary>
		/// <param name="bonDriver_EpgTimerSrv">目标BonDriver</param>
		/// <param name="index">目标位置</param>
		/// <returns></returns>
		public static bool Move(BonDriver_EpgTimerSrv bonDriver_EpgTimerSrv, int index)
		{
			//将当前位置指针域清空
			if (bonDriver_EpgTimerSrv.next != null)
			{
				bonDriver_EpgTimerSrv.previous.next = bonDriver_EpgTimerSrv.next;
				bonDriver_EpgTimerSrv.next.previous = bonDriver_EpgTimerSrv.previous;
            }
            else
            {
				bonDriver_EpgTimerSrv.previous.next = null;
			}
			if (index > bonDriver_EpgTimerSrv.index)
            {
				BonDriver_EpgTimerSrv b = bonDriver_EpgTimerSrv.next;
				//将移动区间内BonDriver的index减一
				while (b.index <= index && b.next != null)
				{
					b.index--;
					b = b.next;
				}
				//当b.next不为空时，将bonDriver_EpgTimerSrv与链表后一个元素连接
				if (b.next != null)
				{
					b.next.previous = bonDriver_EpgTimerSrv;
					bonDriver_EpgTimerSrv.next = b.next;
				}
				//当b.next为空时，将bonDriver_EpgTimerSrv作为链表最后一个元素
				else
				{
					bonDriver_EpgTimerSrv.next = null;
				}
				//将bonDriver_EpgTimerSrv与其之前的元素连接
				b.next = bonDriver_EpgTimerSrv;
				bonDriver_EpgTimerSrv.previous = b;
				//bonDriver_EpgTimerSrv的index设置为前一个元素index+1
				bonDriver_EpgTimerSrv.index = b.index + 1;
            }
            else
            {
				BonDriver_EpgTimerSrv b = bonDriver_EpgTimerSrv.previous;
				//将移动区间内BonDriver的index减一
				while (b.index >= index && b.previous != null)
				{
					b.index++;
					b = b.previous;
				}
				//当b.previous不为空时，将bonDriver_EpgTimerSrv与链表后一个元素连接
				if (b.previous != null)
				{
					b.previous.next = bonDriver_EpgTimerSrv;
					bonDriver_EpgTimerSrv.previous = b.previous;
				}
				//当b.previous为空时，将bonDriver_EpgTimerSrv作为链表第一个元素
				else
				{
					bonDriver_EpgTimerSrv.previous = null;
				}
				//将bonDriver_EpgTimerSrv与其之后的元素连接
				b.previous = bonDriver_EpgTimerSrv;
				bonDriver_EpgTimerSrv.next = b;
				//bonDriver_EpgTimerSrv的index设置为后一个元素index-1
				bonDriver_EpgTimerSrv.index = b.index - 1;
			}
			return true;
		}
		public static bool Delete(BonDriver_EpgTimerSrv bonDriver_EpgTimerSrv)
        {
			//当前为最后一个元素
			if (bonDriver_EpgTimerSrv.next != null)
			{
				bonDriver_EpgTimerSrv.previous.next = bonDriver_EpgTimerSrv.next;
				bonDriver_EpgTimerSrv.next.previous = bonDriver_EpgTimerSrv.previous;
				bonDriver_EpgTimerSrv = null;
				return true;
			}
			//当前不为最后一个元素
			else
			{
				bonDriver_EpgTimerSrv.previous.next = null;
				return true;
			}
		}
	}
	/// <summary>
	/// BonDriverDLL类
	/// </summary>
	class BonDriverDLL
	{
		/// <summary>
		/// 当前BonDriverDLL的文件名
		/// </summary>
		string fileName;
		/// <summary>
		/// 地区
		/// </summary>
		string region;
		/// <summary>
		/// 序号
		/// </summary>
		short index;
		/// <summary>
		/// 卡型（例：PT3）
		/// </summary>
		string tuner;
		/// <summary>
		/// 卡型序号
		/// </summary>
		short tunerIndex;
		/// <summary>
		/// 卫星指示器（False=地面波，True=卫星）
		/// </summary>
		bool sa;
		/// <summary>
		/// 卡型端口号
		/// </summary>
		short saPort;
		/// <summary>
		/// 机主IP地址
		/// </summary>
		string ip;
		/// <summary>
		/// TunerPath
		/// </summary>
		string tunerPath;
		/// <summary>
		/// BonDriverDLL关联的ChSet4.txt信息
		/// </summary>
		List<ChSet4> chSet4s;
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
		int[] GenBonDriver(string region, string tuner, int tunerIndex, bool sa, short index, string ip)
		{
			int tCount = 0, sCount = 0;
			for(int i = 0; i< Program.tuner_counter; i++)
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
			for (int t = 0;t < tCount; t++)
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

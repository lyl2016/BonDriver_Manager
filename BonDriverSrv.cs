using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonDriver_Manager
{
	/// <summary>
	/// EpgTimerSrv的BonDriver调用类，用于控制顺序以及搜台等等
	/// </summary>
	class BonDriverSrv
	{
		/// <summary>
		/// BonDriverSrv总量
		/// </summary>
		public static int BonDriverCount = 0;
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="priority"></param>
		/// <param name="epg"></param>
		/// <param name="count"></param>
		/// <param name="enabled"></param>
		/// <param name="driverDLL">可留NULL</param>
		public BonDriverSrv(string fileName, int priority, bool epg, short count, bool enabled, BonDriverDLL driverDLL)
		{
			this.fileName = fileName;
			this.priority = priority;
			this.epg = epg;
			this.count = count;
			this.enabled = enabled;
			this.driverDLL = driverDLL;
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
		/// 排序当前最大值
		/// </summary>
		public static int PriorityMax;
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
		/// 当前BonDriverSrv对象对应的BonDriverDLL对象，可能为空
		/// </summary>
		public BonDriverDLL driverDLL;
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
}

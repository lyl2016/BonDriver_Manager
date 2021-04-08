using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonDriver_Manager
{
	/// <summary>
	/// 频道ChSet4信息
	/// </summary>
	class ChSet4
	{
		public ChSet4(string chSet4FileName, List<Channel> channels)
		{
			this.chSet4FileName = chSet4FileName;
			this.channels = channels;
		}
		/// <summary>
		/// ChSet4对应文件名
		/// </summary>
		public string chSet4FileName;
		public List<Channel> channels;
	}

	class Channel
	{
		public Channel(string channel, string channelName, string networkName, ushort tunerSpace, ushort channelIndex, ushort sid, ushort onid, ushort tsid, ushort type, bool show)
		{
			this.channel = channel;
			this.channelName = channelName;
			this.networkName = networkName;
			this.tunerSpace = tunerSpace;
			this.channelIndex = channelIndex;
			this.sid = sid;
			this.onid = onid;
			this.tsid = tsid;
			this.type = type;
			this.show = show;
		}
		/// <summary>
		/// 物理频道号（BS1/TS0）
		/// </summary>
		public string channel;
		/// <summary>
		/// 频道名（ＢＳ朝日１）
		/// </summary>
		public string channelName;
		/// <summary>
		/// 所属网络（BS Digital）
		/// </summary>
		public string networkName;
		/// <summary>
		/// Tuner空间序
		/// </summary>
		public ushort tunerSpace;
		/// <summary>
		/// 频道序号，按物理频道号排序
		/// </summary>
		public ushort channelIndex;
		/// <summary>
		/// SID（151）
		/// </summary>
		public ushort sid;
		/// <summary>
		/// ONID（4）
		/// </summary>
		public ushort onid;
		/// <summary>
		/// TSID（16400）
		/// </summary>
		public ushort tsid;
		/// <summary>
		/// 频道种类
		/// 1表示视频信号
		/// 2表示仅音频信号
		/// 162表示临时频道
		/// 192表示OneSegment信号
		/// </summary>
		public ushort type;
		/// <summary>
		/// 表示频道是否在EDCB中展示
		/// </summary>
		public bool show;
		/// <summary>
		/// 按照ChSet4格式化的频道信息
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string showString;
            if (show)
            {
				showString = "1";
            }
            else
            {
				showString = "0";
			}
			return channel + '\t' 
				+ channelName + '\t' 
				+ networkName + '\t'
				+ tunerSpace + '\t' 
				+ channelIndex + '\t' 
				+ onid + '\t' 
				+ tsid + '\t' 
				+ sid + '\t'
				+ type + "\t0\t"
				+ showString + "\t0";
		}
	}
}

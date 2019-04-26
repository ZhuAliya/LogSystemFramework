
/***
   *        Title: "LogSystemFramework" 项目开发
   *            核心层：核心层的参数列表
   *      Description:
   *                作用：
   *            
   *       Data:	[2019]
   *       Version: 0.1
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kernal
{
    public class KernalParameter
    {
#if UNITY_STANDALONE_WIN
         //系统配置信息_日志路径
        internal static readonly string SystemConfigInfo_LogPath ="file://"+Application.dataPath+"/StreamingAssets/SystemConfigInfo.xml";
        //系统配置信息_日志根节点名称
         internal static readonly string SystemConfigInfo_LogRootNodeName ="SystemConfigInfo";
#elif UNITY_ANDROID
         //系统配置信息_日志路径
        internal static readonly string SystemConfigInfo_LogPath =Application.dataPath+"!/Assets/SystemConfigInfo.xml";
        //系统配置信息_日志根节点名称
         internal static readonly string SystemConfigInfo_LogRootNodeName ="SystemConfigInfo";
#elif UNITY_IPHONE
        //系统配置信息_日志路径
        internal static readonly string SystemConfigInfo_LogPath =Application.dataPath+"/Raw/SystemConfigInfo.xml";
        //系统配置信息_日志根节点名称
         internal static readonly string SystemConfigInfo_LogRootNodeName ="SystemConfigInfo";
#endif

    }
}

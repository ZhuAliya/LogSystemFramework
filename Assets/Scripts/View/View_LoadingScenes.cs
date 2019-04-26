/***
   *        Title: "LogSystemFramework" 项目开发
   *            视图层：场景加载控制
   *      Description:
   *                作用：
   *            
   *       Data:	[2019]
   *       Version: 0.1
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kernal;
using UnityEngine.UI;

public class View_LoadingScenes : MonoBehaviour {
    public Slider SliLoadingProgress;   //进度条控制
    private float _FloProgressNumber;   //进度数值
    private AsyncOperation _AsyOper;

    void Start () {

        //测试Log 日志系统
        //面向“接口编程”
        //IConfigManager configMgr = new ConfigManager(KernalParameter.SystemConfigInfo_LogPath, KernalParameter.SystemConfigInfo_LogRootNodeName);
        //string strLogPath = configMgr.AppSetting["LogPath"]; 
        //string strLogState = configMgr.AppSetting["LogState"]; 
        //string strLogMaxCapacity = configMgr.AppSetting["LogMaxCapacity"];
        //string strLogBufferNumber = configMgr.AppSetting["LogBufferNumber"];
        //print("log path =" + strLogPath);
        //print("Log State =" + strLogState);
        //print("Log Max Capacity =" + strLogMaxCapacity);
        //print("Log Buffer Number =" + strLogBufferNumber);

        //测试Log.cs类
        // Log.Write("我的企业日志系统开始运行了，第一次测试");
        //Log.Write("低等级调试语句",Log.Level.Low);
        Log.Write("1低等级调试语句");
        Log.Write("1中等级别调试语句", Log.Level.Special);
        Log.Write("1高级与重要的调试语句", Log.Level.High);
        Log.Write("2低等级调试语句");
        Log.Write("2中等级别调试语句", Log.Level.Special);
        Log.Write("2高级与重要的调试语句", Log.Level.High);
        // Log.SyncLogArrayToFile(); //同步一下（然后就把所有的数据都存入缓存中了）
        Log.ClearLogFileAndBufferAllDate(); //清空Log日志
        Log.Write("------1-------");
        Log.Write("------2-------");
        Log.Write("------3-------");
        Log.Write("------4-------");
        Log.Write("------5-------");
        Log.Write("------6-------");
        print("Log日志缓存中的数量=" + Log.QueryAllDateFromLogBuffer().Count); //output:Log日志缓存中的数量=6

        //调试进入指定的关卡 （第1关卡）
        StartCoroutine("LoadingScenesProgress");
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadingScenesProgress()
    {
        _AsyOper = Application.LoadLevelAsync("2_LoginScene");
        
        _FloProgressNumber = _AsyOper.progress;
        yield return _AsyOper;
    }
 
    void Update () {
        if (_FloProgressNumber <= 1)
        {
            _FloProgressNumber += 0.01F;
        }
        SliLoadingProgress.value = _FloProgressNumber;
    }
}

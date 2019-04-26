/***
   *        Title: "LogSystemFramework" 项目开发
   *             核心层：日志调试系统（log日志）
   *      Description:
   *                作用：更方便于游戏开发人员，调试系统程序
   *                基本实现原理：
   *                    1.把开发人员在代码中定义的调试语句，写入本日志的“缓存”。
   *                    2.当缓存中数量超过定义的最大写入文件数值，则把缓存内容调试语句一次性写入文本文件。
   *            
   *       Data:	[2019]
   *       Version: 0.1
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;                                                          //C#的核心命名空间
using System.IO;                                                      //文件读写命名空间
using System.Threading;                                          //多线程命名空间


namespace Kernal
{
    //public  class Log: UnityEngine.MonoBehaviour
    public static class Log
    {
        /*核心字段*/
        private static List<string> _LiLogArray;             //Log日志缓存数据
        private static string _LogPath = null;                //Log日志文件路径
        private static State _LogState;                           //Log日志状态
        private static int _LogMaxCapacity;                   //Log日志最大容量
        private static int _LogBufferMaxNumber;           //Log日志缓存最大容量
        /*日志文件常量定义 */
        //XML 配置文件 ”标签常量“
        private const string XML_CONFIG_LOG_PATH = "LogPath";
        private const string XML_CONFIG_LOG_STATE = "LogState";
        private const string XML_CONFIG_LOG_MAX_CAPACITY = "LogMaxCapacity";
        private const string XML_CONFIG_LOG_BUFFER_NUMBER = "LogBufferNumber";
        //日志状态常量（部署模式）
        private const string XML_CONFIG_LOG_STATE_DEVELOP = "Develop";
        private const string XML_CONFIG_LOG_STATE_SPECIAL = "Special";
        private const string XML_CONFIG_LOG_STATE_DEPLOY = "Deploy";
        private const string XML_CONFIG_LOG_STATE_STOP = "Stop";
        //日志默认路径
        private static string XML_CONFIG_LOG_DEFAULT_PATH = "TestLog.txt";
        //日志默认最大容量
        private static int LOG_DEFAULT_MAX_CAPACITY_NUMBER = 2000;
        //日志缓存最大容量
        private static int LOG_DEFAULT_MAX_LOG_BUFFER_NUMBER = 1;
        //日志提示信息
        private static string LOG_TIPS = "@@@ Important !!!";
        /* 临时字段定义 */
        private static string strLogState = null;                        //日志状态（部署模式）
        private static string strLogMaxCapacity = null;            //日志最大容量
        private static string strLogBufferNumber = null;          //日志缓存最大容量


        /// <summary>
        /// 静态构造函数
        /// </summary>
        static Log()
        {
            //日志缓存数据
            _LiLogArray = new List<string>();

#if UNITY_STANDALONE_WIN
            //日志文件路径
            IConfigManager configMgr = new ConfigManager(KernalParameter.SystemConfigInfo_LogPath, KernalParameter.SystemConfigInfo_LogRootNodeName);
            _LogPath = configMgr.AppSetting[XML_CONFIG_LOG_PATH];
            //日志状态（部署模式）
            strLogState = configMgr.AppSetting[XML_CONFIG_LOG_STATE];
            //日志最大容量
            strLogMaxCapacity = configMgr.AppSetting[XML_CONFIG_LOG_MAX_CAPACITY];
            //日志缓存最大容量
            strLogBufferNumber = configMgr.AppSetting[XML_CONFIG_LOG_BUFFER_NUMBER];
#endif


            //日志文件路径
            if (string.IsNullOrEmpty(_LogPath))
            {
                _LogPath = UnityEngine.Application.persistentDataPath + "\\" + XML_CONFIG_LOG_DEFAULT_PATH;
            }
            // print("_LogPath=" + _LogPath);

            //日志状态（部署模式）
            if (!string.IsNullOrEmpty(strLogState))
            {
                switch (strLogState)
                {
                    case XML_CONFIG_LOG_STATE_DEVELOP:
                        _LogState = State.Develop;
                        break;
                    case XML_CONFIG_LOG_STATE_SPECIAL:
                        _LogState = State.Special;
                        break;
                    case XML_CONFIG_LOG_STATE_DEPLOY:
                        _LogState = State.Deploy;
                        break;
                    case XML_CONFIG_LOG_STATE_STOP:
                        _LogState = State.Stop;
                        break;
                    default:
                        _LogState = State.Stop;
                        break;
                }
            }
            else
            {
                _LogState = State.Stop;
            }
            // print("_LogState=" + _LogState);

            //日志最大容量

            if (!string.IsNullOrEmpty(strLogMaxCapacity))
            {
                _LogMaxCapacity = Convert.ToInt32(strLogMaxCapacity);
            }
            else
            {
                _LogMaxCapacity = LOG_DEFAULT_MAX_CAPACITY_NUMBER;
            }
            // print("_LogMaxCapacity=" + _LogMaxCapacity);

            //日志缓存最大容量

            if (!string.IsNullOrEmpty(strLogBufferNumber))
            {
                _LogBufferMaxNumber = Convert.ToInt32(strLogBufferNumber);
            }
            else
            {
                _LogBufferMaxNumber = LOG_DEFAULT_MAX_LOG_BUFFER_NUMBER; //默认缓存最大容量
            }
            //  print("_LogBufferMaxNumber=" + _LogBufferMaxNumber);

#if UNITY_STANDALONE_WIN
            //创建文件
            if (!File.Exists(_LogPath))                                //不存在指定路径的文件
            {
                File.Create(_LogPath);
                //关闭当前线程
                Thread.CurrentThread.Abort();                 //终止当前线程
            }

            //把日志文件中的数据同步到日志缓存中
            SyncFileDataToLogArray();
#endif
        }//Log_end(构造函数)

        //把日志文件中的数据同步到日志缓存中
        private static void SyncFileDataToLogArray()
        {
            if (!string.IsNullOrEmpty(_LogPath))
            {
                StreamReader sr = new StreamReader(_LogPath);
                while (sr.Peek() > 0)
                {
                    _LiLogArray.Add(sr.ReadLine());
                }
                sr.Close();
            }
        }

        /// <summary>
        /// 写数据到文件中
        /// </summary>
        /// <param name="writeFileData">写入的调试信息</param>
        /// <param name="level">重要等级级别</param>
        public static void Write(string writeFileDate, Level level)
        {
            //参数检查
            if (_LogState == State.Stop)
            {
                return;
            }
            //如果日志缓存数量超过指定容量，则清空
            if (_LiLogArray.Count >= _LogMaxCapacity)
            {
                _LiLogArray.Clear();                                  //清空缓存中的数据
            }

            if (!string.IsNullOrEmpty(writeFileDate))
            {
                //增加日期于时间
                writeFileDate = "Log State:" + _LogState.ToString() + "/" + DateTime.Now.ToString() + "/" + writeFileDate;

                //对于不同的“日志状态”，分特定情形写入文件
                if (level == Level.High)
                {
                    writeFileDate = LOG_TIPS + writeFileDate;
                }
                switch (_LogState)
                {
                    case State.Develop:                              //开发状态
                        //追加调试信息，写入文件
                        AppendDateToFile(writeFileDate);
                        break;
                    case State.Special:                               //“指定”状态
                        if (level == Level.High || level == Level.Special)
                        {
                            AppendDateToFile(writeFileDate);
                        }
                        break;
                    case State.Deploy:                               //部署状态
                        if (level == Level.High)
                        {
                            AppendDateToFile(writeFileDate);
                        }
                        break;
                    case State.Stop:                                   //停止输出
                        break;
                    default:
                        break;
                }
            }
        }//Write_end

        //方法重载
        public static void Write(string writeFileDate)
        {
            Write(writeFileDate, Level.Low);
        }

        /// <summary>
        /// 追加数据到文件
        /// </summary>
        /// <param name="writeFileDate">调试信息</param>
        private static void AppendDateToFile(string writeFileDate)
        {
            if (!string.IsNullOrEmpty(writeFileDate))
            {
                //调试信息数据追加到缓存集合中
                _LiLogArray.Add(writeFileDate);
            }

            //缓存集合数量超过一定指定数量（“_LogBufferMaxNumber”），则同步到实体文件中。
            if (_LiLogArray.Count % _LogBufferMaxNumber == 0)
            {
                //同步缓存数据信息到实体文件中。
                SyncLogArrayToFile();
            }


        }



        #region 重要管理方法

        /// <summary>
        /// 查询日志缓存中所有数据
        /// </summary>
        /// <returns></returns>
        public static List<string> QueryAllDateFromLogBuffer()
        {
            if (_LiLogArray != null)
            {
                return _LiLogArray;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 清除实体日志文件与日志缓存中所有数据
        /// </summary>
        public static void ClearLogFileAndBufferAllDate()
        {
            if (_LiLogArray != null)
            {
                //数据全部清空
                _LiLogArray.Clear();
            }
            //同步缓存数据信息到实体文件中。
            SyncLogArrayToFile();
        }



        //同步缓存数据信息到实体文件中。
        public static void SyncLogArrayToFile()
        {
            if (!string.IsNullOrEmpty(_LogPath))
            {
                StreamWriter sw = new StreamWriter(_LogPath);
                foreach (string item in _LiLogArray)
                {
                    sw.WriteLine(item);
                }
                sw.Close();
            }
        }

        #endregion

        #region 本类的枚举类型
        /// <summary>
        /// 日志状态（部署模式）
        /// </summary>
        public enum State
        {
            Develop,                                                      //开发模式（输出所有日志内容）
            Special,                                                        //指定输出模式（）
            Deploy,                                                        //部署模式（只输出最核心日志信息，例如严重错误信息，用户登陆账号等）
            Stop                                                            //停止输出模式（不输出任何日志信息）
        };

        /// <summary>
        /// 调试信息的等级（表示调试信息本身的重要程度）
        /// </summary>
        public enum Level
        {
            High,
            Special,
            Low
        }

        #endregion


    }
}


/***
   *        Title: "LogSystemFramework" 项目开发
   *            核心层：自定义异常：XML解析异常
   *      Description:
   *                作用：专门定位于XML解析的异常，如果出现本异常，说明XML格式定义错误。
   *            
   *       Data:	[2019]
   *       Version: 0.1
 * */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Kernal
{
    public class XMLAnalysisException : Exception
    {
        public XMLAnalysisException() : base() { }

        public XMLAnalysisException(string exceptionMessage) : base(exceptionMessage) { }



    }
}

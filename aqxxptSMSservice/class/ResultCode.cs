﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aqxxptSMSservice
{
	class ResultCode
	{
        private const int IMAPI_SUCC = 0;       //  操作成功 
        private const int IMAPI_CONN_ERR = -1;       //  连接数据库出错
        private const int IMAPI_CONN_CLOSE_ERR = -2;       //  数据库关闭失败
        private const int IMAPI_INS_ERR = -3;       //  数据库插入错误
        private const int IMAPI_DEL_ERR = -4;       //  数据库删除错误
        private const int IMAPI_QUERY_ERR = -5;       //  数据库查询错误
        private const int IMAPI_DATA_ERR = -6;       //  参数错误 
        private const int IMAPI_API_ERR = -7;    // API标识非法 
        private const int IMAPI_DATA_TOOLONG = -8;      //  消息内容太长 
        private const int IMAPI_INIT_ERR = -9;      //  没有初始化或初始化失败 
        private const int IMAPI_IFSTATUS_INVALID = -10;    // API接口处于暂停（失效）状态 
        private const int IMAPI_GATEWAY_CONN_ERR = -11;    // 短信网关未连接 

        public int Code { get; set; }
        public string Message { get; set; }

        public ResultCode(int code)
        {
            Code = code;
            switch (code)
            {
                case 0:
                    Message = "操作成功";
                    break;
                case -1:
                    Message = "连接数据库出错";
                    break;
                case -2:
                    Message = "数据库关闭失败";
                    break;
                case -3:
                    Message = "数据库插入错误";
                    break;
                case -4:
                    Message = "数据库删除错误";
                    break;
                case -5:
                    Message = "数据库查询错误";
                    break;
                case -6:
                    Message = "参数错误";
                    break;
                case -7:
                    Message = "API标识非法";
                    break;
                case -8:
                    Message = "消息内容太长";
                    break;
                case -9:
                    Message = "没有初始化或初始化失败";
                    break;
                case -10:
                    Message = "API接口处于暂停（失效）状态";
                    break;
                case -11:
                    Message = "短信网关未连接";
                    break;
                default:
                    Message = "未知错误";
                    code = -99;
                    break;
            }
        }

	}
}
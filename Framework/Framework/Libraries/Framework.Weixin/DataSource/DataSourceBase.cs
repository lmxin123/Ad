using Framework.Weixin.Command;
using Framework.Weixin.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Weixin.DataSource
{
    public abstract class DataSourceBase : IDataSource
    {
        OTTCommand _command;
        WeiXinContext _weiXin;
      //  IniParser _iniParser;

        public OTTCommand Command
        {
            get
            {
                return _command;
            }
        }

        public WeiXinContext WeiXin
        {
            get
            {
                return _weiXin;
            }
        }

        public DataSourceBase(OTTCommand command, WeiXinContext weiXin)
        {
            _command = command;
            _weiXin = weiXin;
        }

        public abstract WeiXinResponse GetResponse();

        public string GetData()
        {
            return Command.Data;
        }

        //public IniParser DataParser
        //{
        //    get
        //    {
        //        if (_iniParser == null)
        //        {
        //            _iniParser = new IniParser(Command.Data);
        //        }
        //        return _iniParser;
        //    }
        //}

        //public string GetSetting(string sectionName, string settingName)
        //{
        //    return DataParser.GetSetting(sectionName, settingName);
        //}

        //public string GetSetting(string settingName)
        //{
        //    return DataParser.GetSetting(settingName);
        //}
    }
}

using log4net;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Core.CrossCuttingConcerns.Logging.Log4Net
{
    public class LoggerServiceBase
    {
        private ILog _log;
        public LoggerServiceBase(string name)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(File.OpenRead("log4net.config"));

            ILoggerRepository loggerRepository = LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(loggerRepository, xmlDocument["log4net"]);

            _log = LogManager.GetLogger(loggerRepository.Name, name);


        }

        public bool IsInfoEnabled => _log.IsInfoEnabled; //bilgi loglamasının etkin olup olmadığını kontrol etme
        public bool IsDebugEnabled => _log.IsDebugEnabled; //hata ayıklama (debug) loglamasının etkin olup olmadığını kontrol etme
        public bool IsWarnEnabled => _log.IsWarnEnabled; //uyarı (warning) seviyesinde loglama yapılıp yapılmadığını kontrol etme
        public bool IsFatalEnabled => _log.IsFatalEnabled; //kritik (fatal) seviyesinde loglama yapılıp yapılmadığını kontrol etmek
        public bool IsErrorEnabled => _log.IsErrorEnabled; //hata (error) seviyesinde loglama yapılıp yapılmadığını kontrol etmek 

        public void Info(object logMessage)
        {
            if (IsInfoEnabled)
                _log.Info(logMessage);
        }

        public void Debug(object logMessage)
        {
            if (IsDebugEnabled)
                _log.Debug(logMessage);
        }

        public void Warn(object logMessage)
        {
            if (IsWarnEnabled)
                _log.Warn(logMessage);
        }

        public void Fatal(object logMessage)
        {
            if (IsFatalEnabled)
                _log.Fatal(logMessage);
        }

        public void Error(object logMessage)
        {
            if (IsErrorEnabled)
                _log.Error(logMessage);
        }


    }
}

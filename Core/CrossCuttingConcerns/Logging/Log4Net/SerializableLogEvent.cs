﻿using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Logging.Log4Net
{
    [Serializable]
    public class SerializableLogEvent
    {
        private LoggingEvent _loggingEvent;

        public SerializableLogEvent(LoggingEvent loggingEvent)
        {
            _loggingEvent = loggingEvent;
        }

        public object Message => _loggingEvent.MessageObject;  //Log mesajı (hangi metot hangi parametrelerle çalıştırıldı bilgisi)
        public string UserName => _loggingEvent.UserName;  //Bu log işlemine sebep olan kişi kimdir bilgisi tutar
    }
}

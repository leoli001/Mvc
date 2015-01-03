﻿using System;
using System.Collections.Generic;

namespace LoggingWebSite.Models
{
    public class ScopeNodeDto
    {
        public ScopeNodeDto Parent { get; set; }

        public List<ScopeNodeDto> Children { get; private set; } = new List<ScopeNodeDto>();

        public List<LogInfoDto> Messages { get; private set; } = new List<LogInfoDto>();

        public object State { get; set; }

        public Type StateType { get; set; }

        public string LoggerName { get; set; }
    }
}
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Automation.JS
{
    public abstract class AbsJS
    {
        public abstract string Path { get; }
        public abstract string GetSuccess { get; }
        public abstract string GetFail { get; }
        public abstract Page Page { get; set; }
    }
}

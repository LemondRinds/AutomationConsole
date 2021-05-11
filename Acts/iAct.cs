using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Acts
{
    public interface IAct
    {
        public Task<bool> Perform(Page p);
    }
}

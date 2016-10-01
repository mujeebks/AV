using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common
{
    public abstract class EngineBase
    {
        /// <summary>
        /// If set, these are used by the engine to change its behaviour from the default.
        /// </summary>
        public Dictionary<String, String> Arguments { get; set; }

        public bool IsDryRun { get; set; }

        public bool DryRunSupported { get; protected set; }

        protected EngineBase()
        {
            DryRunSupported = false;
            Arguments = new Dictionary<string, string>();
        }

        /// <summary>
        /// Set this to insert into the Details field when engine has concluded its running
        /// </summary>
        public Object CompletionDetailsEntry { get; set; }

        public bool Execute()
        {
            IsDryRun = this.Arguments.ContainsKey("DryRun");

            if (IsDryRun && !DryRunSupported)
            {
                Console.WriteLine("Dry runs are not supported for this engine type");
                return false;
            }

            return DoWork();
        }

        public abstract bool DoWork();
    }
}

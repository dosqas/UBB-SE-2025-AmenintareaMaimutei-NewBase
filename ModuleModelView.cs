using System;
using System.Collections.Generic;


namespace Duo
{ 
    public class ModuleModelView
    {
        public Module Module { get; private set; }
        public string Status { get; private set; }
        private readonly ModuleService _moduleService;

        public ModuleModelView(Module module, ModuleService moduleService)
        {
            Module = module ?? throw new ArgumentNullException(nameof(module));
            _moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
            Status = "Not Started";
        }

        public bool IsCompleted()
        {
            return Status.Equals("Completed", StringComparison.OrdinalIgnoreCase);
        }

        public void UpdateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status cannot be empty", nameof(status));

            Status = status;
        }
    }
}
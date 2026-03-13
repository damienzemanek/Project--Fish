using System;

namespace EMILtools.Systems
{
    [Serializable]
    public readonly struct ResolveContainer
    {
        public readonly IResolvable[] beforeExecution;
        public readonly IResolvable[] afterExecution;
        public readonly IResolvable[] failedExecution;
        
        public ResolveContainer(IResolvable[] beforeExecution = null, IResolvable[] afterExecution = null, IResolvable[] failedExecution = null)
        {
            this.beforeExecution = beforeExecution ?? Array.Empty<IResolvable>();
            this.afterExecution = afterExecution ?? Array.Empty<IResolvable>();
            this.failedExecution = failedExecution ?? Array.Empty<IResolvable>();
        }
    }
}
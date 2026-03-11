using System;

namespace EMILtools.Systems
{
    [Serializable]
    public readonly struct ResolveContainer<TResolveType>
        where TResolveType : IResolvable
    {
        public readonly TResolveType[] beforeExecution;
        public readonly TResolveType[] afterExecution;
        public readonly TResolveType[] failedExecution;
        
        public ResolveContainer(TResolveType[] beforeExecution = null, TResolveType[] afterExecution = null, TResolveType[] failedExecution = null)
        {
            this.beforeExecution = beforeExecution ?? Array.Empty<TResolveType>();
            this.afterExecution = afterExecution ?? Array.Empty<TResolveType>();
            this.failedExecution = failedExecution ?? Array.Empty<TResolveType>();
        }
    }
}
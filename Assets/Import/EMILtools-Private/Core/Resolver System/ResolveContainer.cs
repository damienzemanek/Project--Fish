using System;

namespace EMILtools.Core
{
    [Serializable]
    public readonly struct ResolveContainer<TResolveType>
        where TResolveType : IResolvable
    {
        public readonly TResolveType[] beforeExecution;
        public readonly TResolveType[] afterExecution;
        public readonly TResolveType[] failedExecution;

        public ResolveContainer(bool autoInit = false)
        {
            if (autoInit)
            {
                beforeExecution = Array.Empty<TResolveType>();
                afterExecution = Array.Empty<TResolveType>();
                failedExecution = Array.Empty<TResolveType>();
            }
            else
            {
                beforeExecution = null;
                afterExecution = null;
                failedExecution = null;
            }
        }
        
        public ResolveContainer(bool autoInit = true, TResolveType[] beforeExecution = null, TResolveType[] afterExecution = null, TResolveType[] failedExecution = null)
        {
            if (autoInit)
            {
                this.beforeExecution = beforeExecution ?? Array.Empty<TResolveType>();
                this.afterExecution = afterExecution ?? Array.Empty<TResolveType>();
                this.failedExecution = failedExecution ?? Array.Empty<TResolveType>();
            }
            else
            {
                this.beforeExecution = beforeExecution;
                this.afterExecution = afterExecution;
                this.failedExecution = failedExecution;
            }
        }
    }
}
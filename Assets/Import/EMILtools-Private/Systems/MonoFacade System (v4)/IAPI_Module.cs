using EMILtools.Systems;

public interface IAPI_Module { }





public interface IAPI_SignalReceiver : IAPI_Module
{
    public void SendSignal() => ReceiveSignal();
    protected void ReceiveSignal();
}



public interface IMsgReceiver<TClass> : IAPI_Module
    where TClass : class
{
    public void SendSignal(TClass owner);
}


public interface IAPI_Dependant<T> : IAPI_Module
{
    public void SendDependencies(T dependencies) => GrabDependancies(dependencies);
    protected void GrabDependancies(T injectedContext);
}

public interface IAPI_Spawn : IAPI_Module
{
    public void Spawn();
}
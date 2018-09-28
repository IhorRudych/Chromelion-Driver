// Copyright 2018 Thermo Fisher Scientific Inc.
using System;

namespace MyCompany
{
    [Serializable()]
    public class ExceptionAbort : Exception { public ExceptionAbort(string message) : base(message) { } }

    [Serializable()]
    public class ExceptionBusy : Exception { public ExceptionBusy(string message) : base(message) { } }

    [Serializable()]
    public class ExceptionTaskAlreadyRunning : Exception { public ExceptionTaskAlreadyRunning(string message) : base(message) { } }
}

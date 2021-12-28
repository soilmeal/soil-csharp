using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soil.Net.Event;

public interface IEventSourceObserver
{
    public void OnRegistered();

    public void OnbUnregistered();
}

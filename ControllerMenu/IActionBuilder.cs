using System;

namespace ControllerMenu
{
    public interface IActionBuilder
    {
        string Type { get; }
        Action Build(IApplicationContext context, object options);
    }
}
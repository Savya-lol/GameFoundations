using Darkmatter.Core.Services.CommandFactory.Interfaces;
using UnityEngine;
using VContainer;

namespace Darkmatter.Core.Services.CommandFactory
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IObjectResolver _objectResolver;
        public CommandFactory(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }
        public TCommand CreateCommandAsync<TCommand>() where TCommand : ICommandAsync, new()
        {
            var command = new TCommand();
            command.SetResolver(_objectResolver);
            command.ResolveDependencies();
            return command;
        }

        public TCommand CreateCommandAsyncWithResult<TCommand, TReturn>() where TCommand : ICommandAsyncWithResult<TReturn>, new()
        {
            var command = new TCommand();
            command.SetResolver(_objectResolver);
            command.ResolveDependencies();
            return command;
        }

        public TCommand CreateCommandVoid<TCommand>() where TCommand : ICommandVoid, new()
        {
            var command = new TCommand();
            command.SetResolver(_objectResolver);
            command.ResolveDependencies();
            return command;
        }

        public TCommand CreateCommandWithResult<TCommand, TReturn>() where TCommand : ICommandWithResult<TReturn>, new()
        {
            var command = new TCommand();
            command.SetResolver(_objectResolver);
            command.ResolveDependencies();
            return command;
        }
    }
}

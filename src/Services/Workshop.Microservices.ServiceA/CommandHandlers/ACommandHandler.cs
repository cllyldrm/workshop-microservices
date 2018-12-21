using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Workshop.Microservices.ServiceA.Commands;

namespace Workshop.Microservices.ServiceA.CommandHandlers
{
    public class ACommandHandler : IRequestHandler<ACommand, bool>
    {
        public async Task<bool> Handle(ACommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"ACommandHandler Information: {request.Information} ");

            return true;
        }
    }
}